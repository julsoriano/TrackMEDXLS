using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class DescriptionsController : MVCControllerWithHub<Description>
    {
        public DescriptionsController(IEntityService<Description> entityService, IEntityService<Component> componentService) 
            : base(entityService, componentService)
        {
        }

        // GET: Entities
        public async Task<ActionResult> Index()
        {
            var allRecords = await _entityService.GetEntitiesAsync();
            var items = allRecords
                        .OrderBy(x => x.Desc);
            return View(items);
        }

        /*
        public async Task<ActionResult> Index(String id = null)
        {
            var allRecords = await _entityService.GetEntitiesAsync();

            var model = new DescComponentModel<Description>();
            model.descRecords = allRecords
                        .OrderBy(x => x.Desc)
                        .ToList();

            if (id != null)
            {
                model.Id = id;
                var compRecords = await _componentService.GetEntitiesAsync();
                model.linkedComponents = compRecords
                      .OrderBy(x => x.imte)
                      .Where(x => x.DescriptionID == id)
                      .ToList();
            }
            return View(model);
        }
               
        // GET: Entities/Details/5
        public async Task<ActionResult> Details(string id)
        {
            return View(await _entityService.GetEntityAsync(id));
        }

        // GET: Entities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Entities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Description collection)
        {
            try
            {
                await _entityService.PostEntityAsync(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
            return View(Entity);
        }
        
        // POST: Entities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id, Desc, Tag")] Description Entity)
        {
            if (id != Entity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var res = await _entityService.EditEntityAsync(Entity.Id, Entity);
                    var res = await _entityService.EditEntityAsync(Entity);
                }
                catch (Exception)
                {
                    if (!EntityExists(Entity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(Entity);
        }
        */
        /*
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
            var rectodelete = await _entityService.GetEntityAsync(id);

            // See http://stackoverflow.com/questions/2378023/how-to-return-error-from-asp-net-mvc-action to format Json response
            if (rectodelete == null) { return Json(new { Success = false, Status = "Record non-existent" }); }

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
       
        private bool EntityExists(string id)
        {
            return _entityService.VerifyEntityAsync(id).IsCompleted;
        }
        */
        [HttpGet]
        public async Task<IEnumerable<Component>> LoadComponents(string descId)
        {
            List<Component> compRecords = await _componentService.GetSelectedEntitiesAsync("Description", descId);
            var items = compRecords
                          .OrderBy(x => x.imte)
                          //.Where(x => x.DescriptionID == descId)
                          .ToList();

            return items;
        }
    }
}