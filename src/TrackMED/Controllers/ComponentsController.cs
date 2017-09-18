using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.DTOs;
using TrackMED.Services;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace TrackMED.Controllers
{
    //public class ComponentsController : MVCControllerWithHub<Component>
    public class ComponentsController : Controller
    {
        private readonly IEntityService<Component> _entityService;
        private readonly IEntityService<ActivityType> _activitytypeService;
        private readonly IEntityService<Category> _categoryService;
        private readonly IEntityService<Classification> _classificationService;
        private readonly IEntityService<Deployment> _deploymentService;
        private readonly IEntityService<Description> _descriptionService;
        private readonly IEntityService<EquipmentActivity> _equipmentactivityService;
        private readonly IEntityService<Event> _eventService;
        private readonly IEntityService<Location> _locationService;
        private readonly IEntityService<Manufacturer> _manufacturerService;
        private readonly IEntityService<Model_Manufacturer> _modelmanufacturerService;
        private readonly IEntityService<Model> _modelService;
        private readonly IEntityService<Owner> _ownerService;
        private readonly IEntityService<Status> _statusService;
        private readonly IEntityService<ProviderOfService> _serviceproviderService;
        private readonly IEntityService<SystemTab> _systemtabService;

        private readonly ILogger<ComponentsController> _logger;

        /* superceded by procedures Fill and CreateMapping
        internal static readonly Expression<Func<EquipmentActivity, EquipmentActivityDTO>> AsEquipmentActivityDTO =
            x => new EquipmentActivityDTO
            {
                Id = x.Id,
                DeploymentID = x.DeploymentID,
                Work_Order = x.Work_Order,
                WO_Scheduled_Due = x.WO_Scheduled_Due,
                WO_Done_Date = x.WO_Done_Date,
                WO_Calculated_Due_Date = x.WO_Calculated_Due_Date,
                Schedule = x.Schedule,
                eRecord = x.eRecord,
                ActivityTypeID = x.ActivityTypeID,
                ServiceProviderID = x.ServiceProviderID,
                StatusID = x.StatusID,
                ActivityType = x. ActivityType,
                ServiceProvider = x.ServiceProvider,
                Status = x.Status       
            };        
        */

        public ComponentsController(IEntityService<Component> entityService,
                                 IEntityService<ActivityType> activitytypeService,
                                 IEntityService<Category> categoryService,
                                 IEntityService<Classification> classificationService,
                                 IEntityService<Deployment> deploymentService,
                                 IEntityService<Description> descriptionService,
                                 IEntityService<EquipmentActivity> equipmentactivityService,
                                 IEntityService<Event> eventService,
                                 IEntityService<Location> locationService,
                                 IEntityService<Manufacturer> manufacturerService,
                                 IEntityService<Model_Manufacturer> modelmanufacturerService,
                                 IEntityService<Model> modelService,
                                 IEntityService<Owner> ownerService,
                                 IEntityService<ProviderOfService> serviceproviderService,
                                 IEntityService<Status> statusService,
                                 IEntityService<SystemTab> systemtabService,
                                 ILogger<ComponentsController> logger)
        {
            _entityService = entityService;
            _activitytypeService = activitytypeService;
            _categoryService = categoryService;
            _classificationService = classificationService;
            _deploymentService = deploymentService;
            _descriptionService = descriptionService;
            _equipmentactivityService = equipmentactivityService;
            _eventService = eventService;
            _locationService = locationService;
            _manufacturerService = manufacturerService;
            _modelmanufacturerService = modelmanufacturerService;
            _modelService = modelService;
            _ownerService = ownerService;
            _serviceproviderService = serviceproviderService;
            _statusService = statusService;
            _systemtabService = systemtabService;

            _logger = logger;
        }

        // GET: Components
        // http://www.asp.net/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
        [HttpGet]
        public async Task<ActionResult> Index(string selectcomponents)
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "All", Value = "0"},
                new SelectListItem{Text = "Overdue for Maintenance or Calibration", Value = "1"},
                new SelectListItem{Text = "Due for Maintenance or Calibration", Value = "2"},
                new SelectListItem{Text = "Not Due in 30 days", Value = "9"},

            };
            ViewBag.SelectComponents = new SelectList(items, "Value", "Text", "1");

            if (selectcomponents != null)
            {
                ViewBag.CurrentOptions = selectcomponents;
            }

            // Base records
            List<Component> compRecords = await _entityService.GetEntitiesAsync();
            
            var Components = from s in compRecords
                             select s;

            switch (selectcomponents)
            {
                case "0": // all records
                    break;

                case "1": // overdue for maintenance or calibration
                          //Components = Components.Where(s => ((DateTime.Now).Subtract((DateTime)s.MaintenanceDateTime)).Days > 30 || ((DateTime.Now).Subtract((DateTime)s.CalibrationDateTime)).Days > 30);
                    Components = Components.Where(s => (s.MaintenanceDate != null && ((DateTime.Now).Subtract((DateTime)s.MaintenanceDate)).Days > 30 ||
                                                        s.CalibrationDate != null && ((DateTime.Now).Subtract((DateTime)s.CalibrationDate)).Days > 30));
                    break;

                case "2": // due for maintenance or calibration
                    Components = Components.Where(s => (s.CalibrationDate != null && ((DateTime)s.CalibrationDate).Subtract(DateTime.Now).Days <= 30 ||
                                                        s.MaintenanceDate != null && ((DateTime)s.MaintenanceDate).Subtract(DateTime.Now).Days <= 30));
                    break;

                case "3": // due for calibration
                    //Components = from s in compRecords
                    //             where ((DateTime.Now).Subtract(s.CalibrationDateTime).Days <= 30)
                    //             select s;
                    Components = Components.Where(s => s.CalibrationDate != null && ((DateTime.Now).Subtract((DateTime)s.CalibrationDate).Days <= 30));
                    break;

                case "4": // overdue for maintenance
                    Components = Components.Where(s => s.MaintenanceDate != null && ((DateTime.Now).Subtract((DateTime)s.MaintenanceDate).Days > 30));
                    break;

                case "5": // overdue for calibration
                    Components = Components.Where(s => s.CalibrationDate != null && ((DateTime.Now).Subtract((DateTime)s.CalibrationDate).Days > 30));
                    break;

                case "6": // not due for maintenance
                    Components = Components.Where(s => s.MaintenanceDate != null && (((DateTime)s.MaintenanceDate).Subtract(DateTime.Now).Days > 30));
                    break;

                case "7": // not due for calibration
                    Components = Components.Where(s => s.CalibrationDate != null && (((DateTime)s.CalibrationDate).Subtract(DateTime.Now).Days > 30));
                    break;
                     
                case "9": // not due for calibration and maintenance
                    Components = Components.Where(s => (s.CalibrationDate != null && ((DateTime)s.CalibrationDate).Subtract(DateTime.Now).Days > 30) && 
                                                       (s.MaintenanceDate != null && ((DateTime)s.MaintenanceDate).Subtract(DateTime.Now).Days > 30));
                    break;

                default: // all               
                    //Components = Components.Where(s => ((DateTime.Now).Subtract((DateTime)s.MaintenanceDateTime)).Days > 30 || ((DateTime.Now).Subtract((DateTime)s.CalibrationDateTime)).Days > 30);
                    Components =Components.Where(s => (s.MaintenanceDate != null && ((DateTime.Now).Subtract((DateTime)s.MaintenanceDate)).Days > 30 || 
                                                       s.CalibrationDate != null && ((DateTime.Now).Subtract((DateTime)s.CalibrationDate)).Days > 30));
                    break;
            }

            return View(Components);
        }

        /*
        // GET: Components
        // http://www.asp.net/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
        [HttpGet]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, string currentOption, string selectcomponents, IFormCollection collection)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IMTESortParm = String.IsNullOrEmpty(sortOrder) ? "IMTE" : "";
            ViewBag.SerialSortParm = "Serial";
            ViewBag.DescSortParm = "Description";
            ViewBag.ModelSortParm = "Model";
            ViewBag.ManSortParm = "Manufacturer";
            ViewBag.ClassSortParm = "Classification";
            ViewBag.OwnerSortParm = "Owner";
            ViewBag.CalSortParm = "CalibrationDateTime";
            ViewBag.MaiSortParm = "MaintenanceDateTime";

            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "All", Value = "0"},
                new SelectListItem{Text = "Overdue for Maintenance or Calibration", Value = "1"},
                new SelectListItem{Text = "Due for Maintenance or Calibration", Value = "2"},
                //new SelectListItem{Text = "Due for Calibration in one month", Value = "3"},
                //new SelectListItem{Text = "Overdue for Maintenance", Value = "4"},
                //new SelectListItem{Text = "Overdue for Calibration", Value = "5"},
                //new SelectListItem{Text = "Not Due for Maintenance", Value = "6"},
                //new SelectListItem{Text = "Not Due for Calibration", Value = "7"},
                new SelectListItem{Text = "Not Due in 30 days", Value = "9"},

            };
            ViewBag.SelectComponents = new SelectList(items, "Value", "Text", "1");

            if (selectcomponents != null)
            {
                ViewBag.CurrentOptions = selectcomponents;
            }

            ViewBag.CurrentFilter = searchString;

            // Base records
            List<Component> compRecords = await _entityService.GetEntitiesAsync();
            
            var Components = from s in compRecords
                             select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                Components = Components.Where(s => s.imte.Contains(searchString));
                ViewBag.CurrentFilter = null;
            }
            else
            {
                searchString = currentFilter;

                switch (selectcomponents)
                {
                    case "0": // all records
                        break;

                    case "1": // overdue for maintenance or calibration
                        Components = Components.Where(s => ((DateTime.Now).Subtract(s.MaintenanceDateTime)).Days > 30 || ((DateTime.Now).Subtract(s.CalibrationDateTime)).Days > 30);
                        break;

                    case "2": // due for maintenance or calibration
                        Components = Components.Where(s => ((s.CalibrationDateTime).Subtract(DateTime.Now).Days <= 30) || (s.MaintenanceDateTime).Subtract(DateTime.Now).Days <= 30);
                        break;

                    case "3": // due for calibration
                        //Components = from s in compRecords
                        //             where ((DateTime.Now).Subtract(s.CalibrationDateTime).Days <= 30)
                        //             select s;
                        Components = Components.Where(s => ((DateTime.Now).Subtract(s.CalibrationDateTime).Days <= 30));
                        break;

                    case "4": // overdue for maintenance
                        //Components = from s in compRecords
                        //             where ((DateTime.Now).Subtract(s.MaintenanceDateTime).Days > 30)
                        //             select s;
                        Components = Components.Where(s => ((DateTime.Now).Subtract(s.MaintenanceDateTime).Days > 30));
                        break;

                    case "5": // overdue for calibration
                        //Components = from s in compRecords
                        //             where ((DateTime.Now).Subtract(s.CalibrationDateTime).Days > 30)
                        //             select s;
                        Components = Components.Where(s => ((DateTime.Now).Subtract(s.CalibrationDateTime).Days > 30));
                        break;

                    case "6": // not due for maintenance
                        //Components = from s in compRecords
                        //             where ((DateTime.Now).Subtract(s.CalibrationDateTime).Days <= 0)
                        //             select s;
                        Components = Components.Where(s => ((s.MaintenanceDateTime).Subtract(DateTime.Now).Days > 30));
                        break;

                    case "7": // not due for calibration
                        //Components = from s in compRecords
                        //             where ((DateTime.Now).Subtract(s.CalibrationDateTime).Days <= 0)
                        //             select s;
                        Components = Components.Where(s => ((s.CalibrationDateTime).Subtract(DateTime.Now).Days > 30));
                        break;

                    case "9": // not due for calibration and maintenance
                        //Components = from s in compRecords
                        //             where ((DateTime.Now).Subtract(s.CalibrationDateTime).Days <= 0)
                        //             select s;
                        Components = Components.Where(s => ((s.CalibrationDateTime).Subtract(DateTime.Now).Days > 30) && (s.MaintenanceDateTime).Subtract(DateTime.Now).Days > 30);
                        break;

                    default: // all
                        Components = Components.Where(s => ((DateTime.Now).Subtract(s.MaintenanceDateTime)).Days > 30 || ((DateTime.Now).Subtract(s.CalibrationDateTime)).Days > 30);
                        break;
                    }

                    searchString = currentFilter;
                    switch (sortOrder)
                    {
                        case "IMTE":
                            Components = Components.OrderBy(s => s.imte);
                            break;
                        case "Serial":
                            Components = Components.OrderBy(s => s.serialnumber).ThenBy(s => s.imte);
                            break;
                        case "Description":
                            Components = Components.OrderBy(s => s.Description.Desc).ThenBy(s => s.imte);
                            break;
                        case "Model":
                            Components = Components.OrderBy(s => s.Model.Desc).ThenBy(s => s.imte);
                            break;
                        case "Manufacturer":
                            Components = Components.OrderBy(s => s.Manufacturer.Desc).ThenBy(s => s.imte);
                            break;
                        case "Classification":
                            Components = Components.OrderBy(s => s.Classification.Desc).ThenBy(s => s.imte);
                            break;
                        case "Owner":
                            Components = Components.OrderBy(s => s.Owner.Desc).ThenBy(s => s.imte);
                            break;
                        case "CalibrationtionDateTime":
                            Components = Components.OrderBy(s => s.CalibrationDateTime).ThenBy(s => s.imte);
                            break;
                        case "MaintenanceDateTime":
                            Components = Components.OrderBy(s => s.MaintenanceDateTime).ThenBy(s => s.imte);
                            break;
                        default:
                            Components = Components.OrderBy(s => s.imte);
                            break;
                    }
            }

            return View(Components);
        }         
        */

        // GET: Entities/Details/5
        public async Task<ActionResult> Details(string id)
        {
            return View(await _entityService.GetEntityAsync(id));
        }

        public async Task<ActionResult> Create()
        {
            List<Description> descRecords = await _descriptionService.GetEntitiesAsync();
            ViewBag.DescriptionID = new SelectList(descRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Owner> ownerRecords = await _ownerService.GetEntitiesAsync();
            ViewBag.OwnerID = new SelectList(ownerRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<ProviderOfService> serviceproviderRecords = await _serviceproviderService.GetEntitiesAsync();
            ViewBag.ServiceProviderID = new SelectList(serviceproviderRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Model_Manufacturer> modelmanuRecords = await _modelmanufacturerService.GetEntitiesAsync();
              ViewBag.Model_ManufacturerID = new SelectList(modelmanuRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Status> statusRecords = await _statusService.GetEntitiesAsync();
            ViewBag.StatusID = new SelectList(statusRecords.OrderBy(x => x.Desc), "Id", "Desc");

            return View();
        }

        // POST: Systems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("assetnumber, imte, serialnumber, DescriptionID, OwnerID, ServiceProviderID, Model_ManufacturerID, StatusID, Notes, imteModule, CalibrationDate, CalibrationInterval, MaintenanceDate, MaintenanceInterval")]
                                        Component compRecord)
        {
            try
            {
                // no duplicate system IMTE or throw exception
                var recordToCreate = await _entityService.GetEntityAsyncByDescription(compRecord.imte);
                // var recordToCreate = await _entityService.GetEntityAsyncByFieldID("imte", compRecord.imte);

                if (recordToCreate != null)
                {
                    ModelState.AddModelError("imte", "Can not create component record: Duplicate IMTE found");
                }

                if (ModelState.IsValid)
                {
                    compRecord.CreatedAtUtc = DateTime.UtcNow;
                    await _entityService.PostEntityAsync(compRecord);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            List<Description> descRecords = await _descriptionService.GetEntitiesAsync();
            ViewBag.DescriptionID = new SelectList(descRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Owner> ownerRecords = await _ownerService.GetEntitiesAsync();
            ViewBag.OwnerID = new SelectList(ownerRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<ProviderOfService> serviceproviderRecords = await _serviceproviderService.GetEntitiesAsync();
            ViewBag.ServiceProviderID = new SelectList(serviceproviderRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Model_Manufacturer> modelmanuRecords = await _modelmanufacturerService.GetEntitiesAsync();
            ViewBag.Model_ManufacturerID = new SelectList(modelmanuRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Status> statusRecords = await _statusService.GetEntitiesAsync();
            ViewBag.StatusID = new SelectList(statusRecords.OrderBy(x => x.Desc), "Id", "Desc");

            return View(compRecord);
        }

        // GET: Entities/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Entity = await _entityService.GetEntityAsync(id);

            if (Entity == null)
            {
                return NotFound();
            }

            List<Description> descRecords = await _descriptionService.GetEntitiesAsync();
            List<SelectListItem> sl = new SelectList(descRecords.OrderBy(x => x.Desc), "Id", "Desc", Entity.DescriptionID).ToList();
            if (Entity.DescriptionID == null)
            {
                sl.Insert(0, new SelectListItem { Value = "0", Text = "Please Select", Selected = true });
            }
            ViewBag.DescriptionID = sl;

            List<Owner> ownerRecords = await _ownerService.GetEntitiesAsync();
            sl = new SelectList(ownerRecords.OrderBy(x => x.Desc), "Id", "Desc", Entity.OwnerID).ToList();
            if (Entity.OwnerID == null)
            {
                sl.Insert(0, new SelectListItem { Value = "0", Text = "Please Select", Selected = true });
            }
            ViewBag.OwnerID = sl;

            List<ProviderOfService> serviceproviderRecords = await _serviceproviderService.GetEntitiesAsync();
            sl = new SelectList(serviceproviderRecords.OrderBy(x => x.Desc), "Id", "Desc", Entity.ServiceProviderID).ToList();
            if (Entity.ServiceProviderID == null)
            {
                sl.Insert(0, new SelectListItem { Value = "0", Text = "Please Select", Selected = true });
            }
            ViewBag.ServiceProviderID = sl;

            List<Model_Manufacturer> modelmanuRecords = await _modelmanufacturerService.GetEntitiesAsync();
            sl = new SelectList(modelmanuRecords.OrderBy(x => x.Desc), "Id", "Desc", Entity.Model_ManufacturerID).ToList();
            if (Entity.Model_ManufacturerID == null)
            {
                sl.Insert(0, new SelectListItem { Value = "0", Text = "Please Select", Selected = true });
            }
            ViewBag.Model_ManufacturerID = sl;

            List<Status> statusRecords = await _statusService.GetEntitiesAsync();
            sl = new SelectList(statusRecords.OrderBy(x => x.Desc), "Id", "Desc", Entity.StatusID).ToList();
            if (Entity.StatusID == null)
            {
                sl.Insert(0, new SelectListItem { Value = "0", Text = "Please Select", Selected = true });
            }
            ViewBag.StatusID = sl;

            return View(Entity);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id, assetnumber, imte, serialnumber, DescriptionID, OwnerID, ServiceProviderID, Model_ManufacturerID, StatusID, Notes, imteModule, CalibrationDate, CalibrationInterval, MaintenanceDate, MaintenanceInterval")]
                                         Component component)
        {
            var findRecord = await _entityService.GetEntityAsync(id);

            if (findRecord == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    component.ActivityTypeID = component.ActivityTypeID != "0"? component.ActivityTypeID: null;
                    component.DescriptionID = component.DescriptionID != "0" ? component.DescriptionID : null;
                    component.Model_ManufacturerID = component.Model_ManufacturerID != "0" ? component.Model_ManufacturerID : null;
                    component.OwnerID = component.OwnerID != "0" ? component.OwnerID : null;
                    component.ServiceProviderID = component.ServiceProviderID != "0" ? component.ServiceProviderID : null;
                    component.StatusID = component.StatusID != "0" ? component.StatusID : null;

                    var res = await _entityService.EditEntityAsync(component);
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(component);
        }

        // GET: Entities/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            return View(await _entityService.GetEntityAsync(id));
        }

        // POST: Entities/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Remove(string id)
        {
            string errorMessage = null;
            var rectodelete = await _entityService.GetEntityAsync(id);

            // See http://stackoverflow.com/questions/2378023/how-to-return-error-from-asp-net-mvc-action to format Json response
            if (rectodelete == null) { return Json(new { Success = false, Status = "Record non-existent" }); }
            if (rectodelete.imteModule != null)
            {
                errorMessage = "Can't delete component: " + rectodelete.imte + " because Test System " + rectodelete.imteModule 
                               + " uses it. Edit Test System " + rectodelete.imteModule 
                               + " record first and eliminate this component and then retry.";
                _logger.LogWarning(errorMessage);
                return Json(new { Success = false, Status = errorMessage });
            }

            try
            {
                await _entityService.DeleteEntityAsync(id);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Status = ex.Message });
            }

            return Json(new { Success = true, Status = "Completed Successfully" });
        }

        public async Task<Component> LoadComponents(string descId)
        {
            Component item = await _entityService.GetEntityAsync(descId);

            return item;
        }

        [HttpGet]
        public async Task<IEnumerable<EquipmentActivityDTO>> LoadActivities(string descId)
        {
            List<EquipmentActivity> eqactRecords = await _equipmentactivityService.GetSelectedEntitiesAsync("EquipmentActivityID", descId);

            List<EquipmentActivityDTO> eadtoRecords = new List<EquipmentActivityDTO>();
            Fill<EquipmentActivity, EquipmentActivityDTO>(eqactRecords, eadtoRecords);

            foreach (EquipmentActivityDTO ea in eadtoRecords)
            {
                Deployment d = !String.IsNullOrEmpty(ea.DeploymentID) ? _deploymentService.GetEntityAsyncByFieldID("DeploymentID", ea.DeploymentID, "Deployment").Result : null;
                //Deployment d = !String.IsNullOrEmpty(ea.DeploymentID) ? _deploymentService.GetEntityAsync(ea.DeploymentID).Result : null;
                SystemTab st = d != null ? _systemtabService.GetEntityAsync(d.SystemTabID).Result : null;
                ea.DeploymentDate = d.DeploymentDate;
                ea.SystemID = st.imte;
            }

            var items = eadtoRecords
                          .OrderBy(x => x.imte)
                          .ToList();

            return items;
        }

        #region Private Helper Methods
        // http://stackoverflow.com/questions/8846992/copy-the-content-of-one-collection-to-another-using-a-generic-function
        public static Func<T1, T2> CreateMapping<T1, T2>() where T2 : new() 
        {
            var typeOfSource = typeof(T1);
            var typeOfDestination = typeof(T2);

            // use reflection to get a list of the properties on the source and destination types
            var sourceProperties = typeOfSource.GetProperties();
            var destinationProperties = typeOfDestination.GetProperties();

            // join the source properties with the destination properties based on name
            var properties = from sourceProperty in sourceProperties
                             join destinationProperty in destinationProperties
                             on sourceProperty.Name equals destinationProperty.Name
                             select new { SourceProperty = sourceProperty, DestinationProperty = destinationProperty };

            return (x) =>
            {
                var y = new T2();

                foreach (var property in properties)
                {
                    var value = property.SourceProperty.GetValue(x, null);
                    property.DestinationProperty.SetValue(y, value, null);
                }

                return y;
            };
        }

        public static void Fill<T1, T2>(List<T1> Source, List<T2> Destination) where T2 : new()
        {
            Destination.AddRange(Source.Select(CreateMapping<T1, T2>()));
        }

        #endregion
    }
}