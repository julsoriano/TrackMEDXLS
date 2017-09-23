using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackMEDXLS.Models;
using TrackMEDXLS.Services;
using WilderBlog.Services;
using Excel = Microsoft.Office.Interop.Excel;
using Serilog;
using System.Globalization;
// using FluentAssertions;

namespace TrackMEDXLS.Controllers
{
    //[Route("systems")]
    public class SystemTabController : Controller
    {
        private readonly IEntityService<SystemTab> _entityService;
        private readonly IEntityService<ActivityType> _activitytypeService;
        private readonly IEntityService<Component> _componentService;
        private readonly IEntityService<Deployment> _deploymentService;
        private readonly IEntityService<Description> _descriptionService;
        private readonly IEntityService<EquipmentActivity> _equipmentactivityService;
        private readonly IEntityService<Location> _locationService;
        private readonly IEntityService<Model_Manufacturer> _modelmanufacturerService;
        private readonly IEntityService<Owner> _ownerService;
        private readonly IEntityService<ProviderOfService> _serviceproviderService;
        private readonly IEntityService<Status> _statusService;
        private readonly IEntityService<SystemsDescription> _systemsdescriptionService;
        private readonly IEntityService<SystemTab> _systemtabService;

        private readonly ILogger<SystemTabController> _logger;
        private IEmailSender _mailService;
        //private IMailService _mailService;
        private readonly Settings _settings;

        public SystemTabController(IEntityService<SystemTab> entityService,
                                 IEntityService<ActivityType> activitytypeService,
                                 IEntityService<Component> componentService,
                                 IEntityService<Deployment> deploymentService,
                                 IEntityService<Description> descriptionService,
                                 IEntityService<EquipmentActivity> equipmentactivityService,
                                 IEntityService<Location> locationService,
                                 IEntityService<Model_Manufacturer> modelmanufacturerService,
                                 IEntityService<Owner> ownerService,
                                 IEntityService<ProviderOfService> serviceproviderService,
                                 IEntityService<Status> statusService,
                                 IEntityService<SystemsDescription> systemsdescriptionService,
                                 IEntityService<SystemTab> systemtabService,
                                 IOptions<Settings> optionsAccessor,
                                 ILogger<SystemTabController> logger,
                                 IEmailSender mailService)
        {
            _entityService = entityService;
            _activitytypeService = activitytypeService;
            _componentService = componentService;
            _deploymentService = deploymentService;
            _descriptionService = descriptionService;
            _equipmentactivityService = equipmentactivityService;
            _locationService = locationService;
            _modelmanufacturerService = modelmanufacturerService;
            _ownerService = ownerService;
            _serviceproviderService = serviceproviderService;
            _statusService = statusService;
            _systemsdescriptionService = systemsdescriptionService;
            _systemtabService = systemtabService;

            _settings = optionsAccessor.Value; // reads appsettings.json
            _logger = logger;
            _mailService = mailService;
        }

        
        // GET: Entities
        public async Task<ActionResult> Index(string id = null)
        {
            if (id != null) CleanUp(id);

            if (_settings.MergeOnly)
            {
                MergeTables();
            }
            else
                {
                    if (_settings.Initialize)
                    {
                        //await _entityService.DropDatabaseAsync();
                        InitializeMongoDB();
                    }
                }
            
            //await _mailService.SendEmailAsync("jul_soriano@yahoo.com", "Error Creating Components Table", "Please look into this");
            return View(await _entityService.GetEntitiesAsync());
        }


        [HttpPost]
        //[Route("{id}")]  // WARNING! Do NOT resurrect this line. jQuery POST is laid astray by this
        public async void CleanUp(string id)
        {
            if (id == null) return;           

            var recordToUpdate = await _entityService.GetEntityAsync(id);
            if (id == null) return;

            // Remove "PENDING" from imteModule
            if (recordToUpdate.leftComponents != null) CleanUpComponents(recordToUpdate.leftComponents);
            if (recordToUpdate.rightComponents != null) CleanUpComponents(recordToUpdate.rightComponents);

            return;
        }

        #region Private Helper Methods
        private async Task<List<Component>> PopulateLeftRightComponents(List<string> lrComponents)
        {
            List<Component> lrEntities = new List<Component>();

            foreach (var imte in lrComponents)
            {
                var imteToAdd = await _componentService.GetEntityAsync(imte);

                if (imteToAdd != null)
                {
                    imteToAdd.Description = !String.IsNullOrEmpty(imteToAdd.DescriptionID)?_descriptionService.GetEntityAsync(imteToAdd.DescriptionID).Result:null;
                    imteToAdd.Owner = !String.IsNullOrEmpty(imteToAdd.OwnerID)?_ownerService.GetEntityAsync(imteToAdd.OwnerID).Result:null;
                    //imteToAdd.Model = !String.IsNullOrEmpty(imteToAdd.ModelID)?_modelService.GetEntityAsync(imteToAdd.ModelID).Result:null;
                    //imteToAdd.Classification = !String.IsNullOrEmpty(imteToAdd.ClassificationID)?_classificationService.GetEntityAsync(imteToAdd.ClassificationID).Result:null;
                    //imteToAdd.Manufacturer = !String.IsNullOrEmpty(imteToAdd.ManufacturerID)?_manufacturerService.GetEntityAsync(imteToAdd.ManufacturerID).Result:null;            
                    imteToAdd.Model_Manufacturer = !String.IsNullOrEmpty(imteToAdd.Model_ManufacturerID) ? _modelmanufacturerService.GetEntityAsync(imteToAdd.Model_ManufacturerID).Result : null;

                    lrEntities.Add(imteToAdd);
                }
            }
            return lrEntities;
        }
        
        private async void CleanUpComponents(List<string> lrComponents)
        {
            foreach (String s in lrComponents)
            {
                Component comp = await _componentService.GetEntityAsync(s);
                if (comp.imteModule != null)
                {
                    if ((comp.imteModule).Contains("PENDING"))
                    {
                        int pendingStart = (comp.imteModule).IndexOf("PENDING");
                        comp.imteModule = comp.imteModule.Substring(0, pendingStart - 1).Trim();
                        await _componentService.EditEntityAsync(comp);
                    }
                }
            }
        }

        private bool FindImte(String sToFind, List<string> sToFindFrom)
        {
            int cnt = sToFindFrom.Count();
            for(int i=0; i <= cnt; i++)
            {
                if(sToFind == sToFindFrom[i].Substring(1))
                {
                    return true;
                }
            }
            return false;
        }

        private void InitializeMongoDB()
        //public static void InitializeMongoDB(IServiceProvider serviceProvider)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            int rCnt = 0;
            string currentSheet = null;
            string path = String.Empty;

            try
            {
                xlApp = new Excel.Application
                {
                    Visible = false,
                    UserControl = true  //??
                };

                /* Exporting Access table to Excel
                 1. Select EXTERNAL DATA > Excel (Export panel)
                 2. Select Destination: C:\CIT\VS\Workspaces\RESMED\TrackMED_NetCore_461Alt\src\TrackMED\bin\Debug\net461\win7-x64\TrackMEDDataAug2016.xlsx
                 */

                string excelFileName = "\\TrackMEDDataAug2016.xlsx"; // \\TrackMEDData.xlsx

                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + excelFileName;

                // open workbook as read-only, See http://csharp.net-informations.com/excel/csharp-read-excel.htm
                //xlWorkBook = xlApp.Workbooks.Open(@"C:\CedarITT\VisualStudio\Workspaces\RESMED\TrackMED\TrackMED Data.xlsx", 0, true, 5, "", "",
                //    true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                int wsCnt = xlWorkBook.Worksheets.Count;
                int recCnt = 0;

                for (int iSheet = 9; iSheet <= wsCnt; iSheet++)
                {
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(iSheet);
                    range = xlWorkSheet.UsedRange;
                    int nRows = range.Rows.Count;
                    int nColumns = range.Columns.Count;
                    

                    for (rCnt = 2; rCnt <= nRows; rCnt++)
                    {
                        /* Use 'Range' property in lieu of the 'get_range' method: Gets as Microsoft.Office.Interop.Excel.Range object that represents a cell or a range of cells. 
                           See https://msdn.microsoft.com/en-us/library/microsoft.office.tools.excel.worksheet.get_range.aspx */
                        Excel.Range rng = (Excel.Range)xlWorkSheet.Range[range.Cells[rCnt, 1], range.Cells[rCnt, nColumns]];

                        // Adapted from: http://forums.asp.net/t/1719856.aspx?Three+Methods+to+Read+Excel+with+ASP+NET
                        // The only difference between Value2 property and the Value property is that Value2 is not a parameterized property. https://msdn.microsoft.com/en-us/library/microsoft.office.tools.excel.namedrange.ToString()2(v=vs.90).aspx

                        dynamic myvalues;
                        myvalues = nColumns > 1? (object[,])rng.Value2:(object)rng.Value2;
                        /* {object[1..1, 1..3]}
                            [1, 1]: 1.0
                            [1, 2]: "Flow Analyser"
                            [1, 3]: ""
                        */

                        dynamic classInstance;
                        switch (xlWorkSheet.Name)
                        {
                            case "EquipmentDescription":
                                if (rCnt == 2)
                                {
                                    currentSheet = "EquipmentDescription";

                                    /*
                                    List<Description> listDesc = new List<Description>();
                                    // http://www.asp.net/web-api/overview/web-api-routing-and-actions/create-a-rest-api-with-attribute-routing               
                                    listDesc.AddRange(new Description[] {
                                        new Description() { Desc = "Fixture", CreatedAtUtc = DateTime.Now },
                                        new Description() { Desc = "Module", CreatedAtUtc = DateTime.Now }
                                        });
                                    _descriptionService.PostEntitiesAsync(listDesc);
                                    */
                                }

                                classInstance = CreateRecord<Description>(myvalues, nColumns);
                                _descriptionService.PostEntityAsync(classInstance);

                                break;

                            case "Model_Manufacturer":
                                if (rCnt == 2)
                                {
                                    //recCnt.Should().Be(40); // # Description records
                                    recCnt = 0;
                                    currentSheet = "Model_Manufacturer";
                                }

                                classInstance = CreateRecord<Model_Manufacturer>(myvalues, nColumns);
                                _modelmanufacturerService.PostEntityAsync(classInstance);
                                
                                break;

                            case "Owner":
                                if (rCnt == 2) currentSheet = "Owner";

                                classInstance = CreateRecord<Owner>(myvalues, nColumns);
                                _ownerService.PostEntityAsync(classInstance);
                                
                                break;

                            case "Status":
                                if (rCnt == 2) currentSheet = "Status";
                                
                                classInstance = CreateRecord<Status>(myvalues, nColumns);
                                _statusService.PostEntityAsync(classInstance);

                                break;

                            case "Location":
                                if (rCnt == 2) currentSheet = "Location";
                                
                                classInstance = CreateRecord<Location>(myvalues, nColumns);
                                _locationService.PostEntityAsync(classInstance);
                                
                                break;

                            case "ProviderOfService":
                                if (rCnt == 2) currentSheet = "ProviderOfService";
                                
                                classInstance = CreateRecord<ProviderOfService>(myvalues, nColumns);
                                _serviceproviderService.PostEntityAsync(classInstance);
                                
                                break;

                            case "SystemsDescription":
                                if (rCnt == 2) currentSheet = "SystemsDescription";
                                
                                classInstance = CreateRecord<SystemsDescription>(myvalues, nColumns);
                                _systemsdescriptionService.PostEntityAsync(classInstance);
                                
                                break;

                            case "ActivityType":
                                if (rCnt == 2) currentSheet = "ActivityType";

                                classInstance = CreateRecord<ActivityType>(myvalues, nColumns);
                                _activitytypeService.PostEntityAsync(classInstance);

                                break;

                            case "Component": // TrackMEDDataAug2016
                                if (rCnt == 2) currentSheet = "Component";
                                

                                // http://stackoverflow.com/questions/18993735/how-to-read-single-excel-cell-value
                                string objDescId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 4] as Excel.Range).Value2))
                                {
                                    string sDesc = (string)(range.Cells[rCnt, 4] as Excel.Range).Value2;
                                    Description objDesc = _descriptionService.GetEntityAsyncByDescription(sDesc).Result;
                                    //Description objDesc = _descriptionService.GetEntityAsyncByFieldID("Desc", sDesc).Result;
                                    objDescId = objDesc != null ? (objDesc).Id : null;
                                }

                                string objOwnerId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 5] as Excel.Range).Value2))
                                {
                                    string sOwner = (string)(range.Cells[rCnt, 5] as Excel.Range).Value2;
                                    Owner objOwner = _ownerService.GetEntityAsyncByDescription(sOwner).Result;
                                    objOwnerId = objOwner != null? (objOwner).Id : null;
                                }

                                string objModelManufacturerId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 7] as Excel.Range).Value2))
                                {
                                    string sModelManufacturer = (string)(range.Cells[rCnt, 7] as Excel.Range).Value2;
                                    Model_Manufacturer objModelManufacturer = _modelmanufacturerService.GetEntityAsyncByDescription(sModelManufacturer).Result;
                                    objModelManufacturerId = objModelManufacturer != null ? (objModelManufacturer).Id : null;
                                }

                                Component component = new Component
                                {
                                    imte = (string)(range.Cells[rCnt, 1] as Excel.Range).Value2,
                                    serialnumber = !String.IsNullOrEmpty((string)(range.Cells[rCnt, 2] as Excel.Range).Value2)? (string)(range.Cells[rCnt, 2] as Excel.Range).Value2:String.Empty,
                                    assetnumber = (string)(range.Cells[rCnt, 3] as Excel.Range).Value2,
                                    OwnerID = objOwnerId,
                                    DescriptionID = objDescId,
                                    Model_ManufacturerID = objModelManufacturerId,
                                    Notes = String.Empty,
                                    imteModule = null,

                                    CreatedAtUtc = DateTime.Now
                                };

                                _componentService.PostEntityAsync(component);

                                break;

                           /*
                            case "Events":
                                try
                                {
                                    string simte = (string)(range.Cells[rCnt, 3] as Excel.Range).Value2;
                                    string sStatus = (string)(range.Cells[rCnt, 4] as Excel.Range).Value2;
                                    string sCat = (string)(range.Cells[rCnt, 5] as Excel.Range).Value2;

                                    // http://stackoverflow.com/questions/22169936/casting-double-to-string
                                    string sVal = Convert.ToString((double?)(range.Cells[rCnt, 6] as Excel.Range).Value2);

                                    string sLoc = (string)(range.Cells[rCnt, 7] as Excel.Range).Value2;

                                    Event eventRecord = new Event
                                    {
                                        imte = (string)(range.Cells[rCnt, 3] as Excel.Range).Value2,
                                        EventDateTime = DateTime.FromOADate((double)(range.Cells[rCnt, 2] as Excel.Range).Value2),
                                        EquipmentID = null,
                                        StatusID = sStatus,
                                        CategoryID = sCat,
                                        Validity = sVal,
                                        LocationID = sLoc
                                    };
                                    _eventService.PostEntityAsync(eventRecord);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("{0} Unable to fully create Events file", e);
                                }
                                break;


                                */

                            case "System":
                                if (rCnt == 2) currentSheet = "System";

                                string objSysDescId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 2] as Excel.Range).Value2))
                                {
                                    string sSysDesc = (string)(range.Cells[rCnt, 2] as Excel.Range).Value2;
                                    SystemsDescription objSysDesc = _systemsdescriptionService.GetEntityAsyncByDescription(sSysDesc).Result;
                                    objSysDescId = objSysDesc != null ? (objSysDesc).Id : null;
                                }

                                SystemTab sysdesc = new SystemTab
                                {
                                    imte = (string)(range.Cells[rCnt, 1] as Excel.Range).Value2,
                                    serialnumber = String.Empty,
                                    SystemsDescriptionID = objSysDescId,
                                    OwnerID = null,
                                    LocationID = null,
                                    SystemsDescription = null,
                                    Owner = null,
                                    Location = null,
                                    Notes = String.Empty,
                                    leftComponents = new List<string>(),
                                    rightComponents = new List<string>(),

                                    CreatedAtUtc = DateTime.Now
                                };

                                _systemtabService.PostEntityAsync(sysdesc);

                                break;

                            case "Deployment":
                                if (rCnt == 2) currentSheet = "Deployment";

                                string objSystemTabId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 3] as Excel.Range).Value2))
                                {
                                    string sSystemTab = (string)(range.Cells[rCnt, 3] as Excel.Range).Value2;
                                    SystemTab objSystemTab = _entityService.GetEntityAsyncByDescription(sSystemTab).Result;
                                    objSystemTabId = objSystemTab != null ? (objSystemTab).Id : null;
                                }

                                string objLocationId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 4] as Excel.Range).Value2))
                                {
                                    string sLocation = (string)(range.Cells[rCnt, 4] as Excel.Range).Value2;
                                    Location objLocation = _locationService.GetEntityAsyncByDescription(sLocation).Result;
                                    objLocationId = objLocation != null ? (objLocation).Id : null;
                                }

                                Deployment deployment = new Deployment
                                {
                                    DeploymentID = Convert.ToString((double)(range.Cells[rCnt, 1] as Excel.Range).Value2),

                                    // http://stackoverflow.com/questions/4538321/reading-datetime-value-from-excel-sheet
                                    DeploymentDate = DateTime.FromOADate((double)(range.Cells[rCnt, 2] as Excel.Range).Value2),
                                    SystemTabID = objSystemTabId,
                                    LocationID = objLocationId,
                                    ReferenceNo = !String.IsNullOrEmpty((string)(range.Cells[rCnt, 5] as Excel.Range).Value2) ? (string)(range.Cells[rCnt, 5] as Excel.Range).Value2:String.Empty,

                                    //CreatedAtUtc = DateTime.Now
                                };

                                _deploymentService.PostEntityAsync(deployment);

                                break;
                            
                            case "EquipmentActivity":

                                if (rCnt == 2) currentSheet = "EquipmentActivity";
                                

                                string objEquipmentId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 2] as Excel.Range).Value2))
                                {
                                    string sEquipment = (string)(range.Cells[rCnt, 2] as Excel.Range).Value2;
                                    Component objEquipment = _componentService.GetEntityAsyncByDescription(sEquipment).Result;
                                    objEquipmentId = objEquipment != null ? (objEquipment).Id : null;
                                }

                                string objActivityTypeId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 8] as Excel.Range).Value2))
                                {
                                    string sActivityType = (string)(range.Cells[rCnt, 8] as Excel.Range).Value2;
                                    ActivityType objActivityType = _activitytypeService.GetEntityAsyncByDescription(sActivityType).Result;
                                    objActivityTypeId = objActivityType != null ? (objActivityType).Id : null;
                                }

                                string objServiceProviderId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 9] as Excel.Range).Value2))
                                {
                                    string sServiceProvider = (string)(range.Cells[rCnt, 9] as Excel.Range).Value2;
                                    ProviderOfService objServiceProvider = _serviceproviderService.GetEntityAsyncByDescription(sServiceProvider).Result;
                                    objServiceProviderId = objServiceProvider != null ? (objServiceProvider).Id : null;
                                }

                                string objStatusId = null;
                                if (!String.IsNullOrEmpty((string)(range.Cells[rCnt, 12] as Excel.Range).Value2))
                                {
                                    string sStatus = (string)(range.Cells[rCnt, 12] as Excel.Range).Value2;
                                    Status objStatus = _statusService.GetEntityAsyncByDescription(sStatus).Result;
                                    objStatusId = objStatus != null ? (objStatus).Id : null;
                                }

                                DateTime? wo_sd = null;
                                if (((double?)(range.Cells[rCnt, 4] as Excel.Range).Value2) != null) {
                                    wo_sd = DateTime.FromOADate((double)(range.Cells[rCnt, 4] as Excel.Range).Value2);
                                }

                                DateTime? wo_dd = null;
                                if (((double?)(range.Cells[rCnt, 5] as Excel.Range).Value2) != null)
                                {
                                    wo_dd = DateTime.FromOADate((double)(range.Cells[rCnt, 5] as Excel.Range).Value2);
                                }

                                DateTime? wo_cdd = null;
                                if (((double?)(range.Cells[rCnt, 7] as Excel.Range).Value2) != null)
                                {
                                    wo_cdd = DateTime.FromOADate((double)(range.Cells[rCnt, 7] as Excel.Range).Value2);
                                }

                                EquipmentActivity equipmentactivity = new EquipmentActivity
                                {
                                    imte = objEquipmentId,
                                    Work_Order = !String.IsNullOrEmpty((string)(range.Cells[rCnt, 3] as Excel.Range).Value2) ? (string)(range.Cells[rCnt, 3] as Excel.Range).Value2 : String.Empty,
                                    WO_Scheduled_Due = wo_sd,
                                    WO_Done_Date = wo_dd,
                                    Schedule = ((double?)(range.Cells[rCnt, 6] as Excel.Range).Value2) != null ? Convert.ToString((double)(range.Cells[rCnt, 6] as Excel.Range).Value2) : null,
                                    WO_Calculated_Due_Date = wo_cdd,
                                    ActivityTypeID = objActivityTypeId,
                                    ProviderOfServiceID = objServiceProviderId,
                                    eRecord = !String.IsNullOrEmpty((string)(range.Cells[rCnt, 10] as Excel.Range).Value2) ? (string)(range.Cells[rCnt, 10] as Excel.Range).Value2 : String.Empty,
                                    DeploymentID = ((double?)(range.Cells[rCnt, 11] as Excel.Range).Value2) != null ? Convert.ToString((double)(range.Cells[rCnt, 11] as Excel.Range).Value2) : null,
                                    StatusID = objStatusId
                                };

                                _equipmentactivityService.PostEntityAsync(equipmentactivity);
                                recCnt++;

                                break; 

                            default:
                                break;
                        }
                    }
                }

                MergeTables();

                xlWorkBook.Close(false, null, null);
                //xlApp.Quit();

                releaseObject(xlWorkBook);
                //releaseObject(xlApp);
            }
            catch (System.IO.FileNotFoundException fileEx)
            {
                // Handle the specific FileNotFoundException here.
                Console.WriteLine("File not found" + fileEx.ToString());
                _logger.LogWarning("File not found {0} " + fileEx.TargetSite.ToString(), path);

            }
            catch (System.IO.IOException ioEx)
            {
                //Handle other non-specific IO Exceptions here.
                Console.WriteLine("An IO exception has occurred" + ioEx.ToString());
                _logger.LogWarning("IO exception in creating {0} table at row position = {1}: " + ioEx.TargetSite.ToString(), rCnt, currentSheet);
            }
            catch (Exception e)
            {
                //' Handle any other non-IO Exception here.
                Console.WriteLine("{0} Error in creating table", e);
                _logger.LogWarning("Error in creating {0} table at record count = {1}: " + e.TargetSite.ToString(), currentSheet, rCnt );
                //Console.WriteLine("An unspecified exception has occurred" + ex.ToString());
            }
            finally
            {
                // See http://csharp.net-informations.com/excel/csharp-read-excel.htm                     
            }

        }

        private void MergeTables()
        {
            // update system table with data from deployment table
            int cntStab = 0;
            int cntStabDesired = 0;
            string[] vsvsiiiline = { "RPMX09384_Yellow", "RPMX09384_Blue", "RPMX09384_Yellow", "RPMX09384_Blue" };

            List<SystemTab> recSystemTab = _entityService.GetEntitiesAsync().Result;
            foreach (SystemTab stab in recSystemTab)
            {
                // initialize fields that will be read from deployments
                stab.leftComponents = new List<string>();
                stab.DeploymentDate = null;
                stab.LocationID = null;
                stab.Location = null;
                stab.ReferenceNo = String.Empty;

                cntStab++;

                // See http://stackoverflow.com/questions/2912476/using-c-sharp-to-check-if-string-contains-a-string-in-string-array
                if (vsvsiiiline.Any(s => stab.imte.Contains(s))) // or: if( vsvsiiiline.Any(stab.imte.Contains) )
                {
                    //_logger.LogWarning("imte {0}\t @row {1}: \rLocationID = {2} ", stab.imte + "\t", cntStab);
                    cntStabDesired++;
                }

                // Or Alternately: See https://msdn.microsoft.com/en-us/library/yw84x8be(v=vs.110).aspx and https://msdn.microsoft.com/en-us/library/bb384015.aspx
                // if (Array.Exists(vsvsiiiline, element => element == stab.imte))

                // read all deployments
                List<Deployment> recDeployment = _deploymentService.GetSelectedEntitiesAsync("Deployment", stab.Id).Result;
                foreach (Deployment d in recDeployment)
                {

                    // update system tab fields
                    if (stab.DeploymentDate != null)
                    {
                        if (stab.DeploymentDate < d.DeploymentDate)
                        {
                            stab.DeploymentDate = d.DeploymentDate;
                            stab.ReferenceNo = d.ReferenceNo;
                            stab.LocationID = !String.IsNullOrEmpty(d.LocationID) ? d.LocationID : null;
                            stab.ReferenceNo = !String.IsNullOrEmpty(d.ReferenceNo) ? d.ReferenceNo : null;
                        }
                    }
                    else
                    {
                        stab.DeploymentDate = d.DeploymentDate;
                        stab.ReferenceNo = d.ReferenceNo;
                        stab.LocationID = !String.IsNullOrEmpty(d.LocationID) ? d.LocationID : null;
                        stab.ReferenceNo = !String.IsNullOrEmpty(d.ReferenceNo) ? d.ReferenceNo : null;
                    }
                    // Start
                    // read all equipment activity records
                    List<EquipmentActivity> recActivity = _equipmentactivityService.GetEntitiesAsync(d.DeploymentID).Result;

                    // update component table with data from equipment activity table
                    Status recStatus = _statusService.GetEntityAsyncByDescription("Active_PROD").Result;
                    if (recActivity != null)
                    {
                        foreach (EquipmentActivity ea in recActivity)
                        {
                            if (ea.imte == null)
                            {
                                continue;
                            }

                            Component c = _componentService.GetEntityAsync(ea.imte).Result;
                            if (c != null)
                            {
                                if (!stab.leftComponents.Contains(ea.imte)) stab.leftComponents.Add(ea.imte);

                                // intialize fields that will be read from equipment activity records
                                c.CalibrationDate = null;
                                c.MaintenanceDate = null;
                                c.StatusID = null;
                                c.Status = null;
                                c.ProviderOfServiceID = null;
                                c.imteModule = null;

                                if (ea.ActivityTypeID != null)
                                {
                                    ActivityType at = _activitytypeService.GetEntityAsync(ea.ActivityTypeID).Result;
                                    switch (at.Desc)
                                    {
                                        case "Calibration":
                                            if (c.CalibrationDate != null)
                                            {
                                                if (c.CalibrationDate < ea.WO_Calculated_Due_Date)
                                                {
                                                    // always get the latest
                                                    c.CalibrationDate = ea.WO_Calculated_Due_Date;
                                                    c.StatusID = c != null ? recStatus.Id : null;
                                                    c.ProviderOfServiceID = ea.ProviderOfServiceID;
                                                    c.imteModule = stab.imte;
                                                }
                                            }
                                            else
                                            {
                                                c.CalibrationDate = ea.WO_Calculated_Due_Date;
                                                c.StatusID = c != null ? recStatus.Id : null;
                                                c.ProviderOfServiceID = ea.ProviderOfServiceID;
                                                c.imteModule = stab.imte;
                                            }

                                            break;

                                        case "Maintenance":
                                            if (c.MaintenanceDate != null)
                                            {
                                                // always get the latest
                                                if (c.MaintenanceDate < ea.WO_Calculated_Due_Date)
                                                {
                                                    c.MaintenanceDate = ea.WO_Calculated_Due_Date;
                                                    c.StatusID = c != null ? recStatus.Id : null;
                                                    c.ProviderOfServiceID = ea.ProviderOfServiceID;
                                                    c.imteModule = stab.imte;
                                                }
                                            }
                                            else
                                            {
                                                c.MaintenanceDate = ea.WO_Calculated_Due_Date;
                                                c.StatusID = c != null ? recStatus.Id : null;
                                                c.ProviderOfServiceID = ea.ProviderOfServiceID;
                                                c.imteModule = stab.imte;
                                            }

                                            break;

                                        default:
                                            break;
                                    } // End: switch (at.Desc)											
                                } // End: if (ea.ActivityTypeID != null)
                                _componentService.EditEntityAsync(c);
                            } // End: if(c!= null)
                        } // End: foreach (EquipmentActivity ea in recActivity)
                    } // End: if (recActivity != null)
                } // End: foreach (Deployment d in recDeployment)
                _entityService.EditEntityAsync(stab);
            } // foreach (SystemTab stab in recSystemTab)
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                // http://forums.asp.net/t/1719856.aspx?Three+Methods+to+Read+Excel+with+ASP+NET
                Process[] procs = Process.GetProcessesByName("excel");
                foreach (Process pro in procs)
                {
                    pro.Kill();//Kill process.
                }
                GC.Collect();
            }
        }

        private static T CreateRecord<T>(dynamic myvalues, int nColumns)
        {

            Type t = typeof(T);
            //var properties = t.GetProperties();

            PropertyInfo[] pi = t.GetProperties();
            int pCnt = pi.Count();

            // in lieu of the non-generic: T classInstance = new Description(); see https://msdn.microsoft.com/en-us/library/a89hcwhh(v=vs.110).aspx
            // ConstructorInfo magicConstructor = t.GetConstructor(Type.EmptyTypes);
            // T classInstance = (T) magicConstructor.Invoke(new object[] { });

            T classInstance = (T)Activator.CreateInstance(typeof(T), new object[] { }); // http://stackoverflow.com/questions/731452/create-instance-of-generic-type

            for (int i = 1; i < pCnt; i++) // leaving out Id which will be filled up my MongoDB
            {
                if (pi[i].PropertyType != null && pi[i].CanWrite)
                {
                    //if (pi[i].Name == "CreatedAtUtc") pi[i].SetValue(classInstance, DateTime.Now);
                    if (pi[i].PropertyType == typeof(DateTime))
                    {
                        pi[i].SetValue(classInstance, DateTime.Now);
                    }
                    else
                        if (nColumns > 1)
                        {
                            pi[i].SetValue(classInstance, Convert.ChangeType(myvalues[1, i], pi[i].PropertyType), null); // http://stackoverflow.com/questions/1089123/setting-a-property-by-reflection-with-a-string-value
                            //pi[i].SetValue(classInstance, myvalues[1, i+1]);     Error: Object of type 'System.Double' cannot be converted to type 'System.Int32'.
                        }
                        else
                        {
                            pi[i].SetValue(classInstance, Convert.ChangeType(myvalues, pi[i].PropertyType), null);
                        }                               
                }
            }

            return classInstance;
        }

        private int ValidateIMTE(String imte)
        {
            // no special characters allowed
            char[] chars = { '+', '&', '|', '!', '(', ')', '{', '}', '[', ']', '^', '~', '*', '?', ':', '\\', ';', '/', '%', '$' };
            int indexSpecial = imte.IndexOfAny(chars);
            if (indexSpecial >= 0)
            {
                return (int)ErrorCodes.SpecialCharacters;
            }

            // get the portion before the _
            Regex r = new Regex(@"^(?<imtenew>[A-Za-z0-9]+)_?.*$", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            Match m = r.Match(imte);

            if (!m.Success)
            {
                return (int)ErrorCodes.InvalidFormat;
            }

            string[] suffixesIMTE = { String.Empty, "_Yellow", "_Blue", "_Left", "_Right", "_L", "_R" };
            int cnt = suffixesIMTE.Count();
            for (int i = 0; i < cnt; i++)
            {
                string imteToCreate = imte + suffixesIMTE[i];
                SystemTab systabToCreate = _entityService.GetEntityAsyncByDescription(imteToCreate).Result;
                if (systabToCreate != null)
                {
                    return (int)ErrorCodes.DuplicateIMTESystem;
                }
                else
                {
                    Component compToCreate = _componentService.GetEntityAsyncByDescription(imteToCreate).Result;
                    if (compToCreate != null)
                    {
                        return (int)ErrorCodes.DuplicateIMTEComponent;
                    }
                }
            }

            return (int)ErrorCodes.Valid;
        }

        private void CreateCommon()
        {
            List<SystemsDescription> descRecords =  _systemsdescriptionService.GetEntitiesAsync().Result;
            ViewBag.SystemsDescriptionID = new SelectList(descRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Owner> ownerRecords =  _ownerService.GetEntitiesAsync().Result;
            ViewBag.OwnerID = new SelectList(ownerRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Location> locationRecords =_locationService.GetEntitiesAsync().Result;
            ViewBag.LocationID = new SelectList(locationRecords.OrderBy(x => x.Desc), "Id", "Desc");

            // select only components that are not yet commissioned
            List<Component> compRecords = _componentService.GetEntitiesAsync().Result;
            var items = compRecords
                        .OrderBy(x => x.Description.Desc)
                        .Where(x => String.IsNullOrEmpty(x.imteModule));

            // create lookup table with key = Description and value = IEnumerable<Component>
            ILookup<string, Component> lookup = items
                    .ToLookup(p => p.Description.Desc.Trim(), // + (!String.IsNullOrEmpty(p.Description.Tag) ? " (" + p.Description.Tag + ")" : null),
                              p => p);                        // will be used to create a select list of p.IMTE + " " + p.MaintenanceDateTime);

            ViewBag.LookUp = lookup;

            // from the lookup table: create a select list of description + tag only 
            var listDesc = new List<SelectListItem>();
            foreach (IGrouping<string, Component> g in lookup)
            {
                listDesc.Add
                (
                    new SelectListItem { Value = g.Key, Text = g.Key }
                );
            }
            ViewBag.ComponentDescID = new SelectList(listDesc, "Value", "Text");
        }

        enum ErrorCodes { Valid, SpecialCharacters, InvalidFormat, DuplicateIMTESystem, DuplicateIMTEComponent };

        #endregion
    }

}