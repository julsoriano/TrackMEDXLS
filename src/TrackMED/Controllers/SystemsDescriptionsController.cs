using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class SystemsDescriptionsController: Controller
    {
        private readonly IEntityService<SystemsDescription> _entityService;
        private readonly IEntityService<SystemTab> _systemtabService;

        public SystemsDescriptionsController(IEntityService<SystemsDescription> entityService, 
                                            IEntityService<SystemTab> systemtabService) 
        {
            _entityService = entityService;
            _systemtabService = systemtabService;
        }

        // GET: Entities
        public async Task<ActionResult> Index()
        {
            var allRecords = await _entityService.GetEntitiesAsync();
            var items = allRecords
                        .OrderBy(x => x.Desc);
            return View(items);
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
        public async Task<ActionResult> Create(SystemsDescription collection)
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
        //public async Task<IActionResult> Edit(string id, T Entity)
        public async Task<IActionResult> Edit(string id, [Bind("Id, Desc, Tag")] SystemsDescription Entity)
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
                    //var res = await _entityService.EditEntityAsync(Entity.Id, Entity);
                    var res = await _entityService.EditEntityAsync(Entity);
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(Entity);
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

        [HttpGet]
        public async Task<IEnumerable<SystemTab>> LoadSystems(string descId)
        {
            List<SystemTab> systemRecords = await _systemtabService.GetEntitiesAsync();

            var items = systemRecords
                          .OrderBy(x => x.imte)
                          .Where(x => x.SystemsDescriptionID == descId)
                          .ToList();

            return items;
        }
    }
}