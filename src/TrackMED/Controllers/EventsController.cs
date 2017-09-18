using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEntityService<Event> _entityService;

        public EventsController(IEntityService<Event> entityService)
        {
            _entityService = entityService;
        }

        // GET: Entities
        public async Task<ActionResult> Index()
        {
            return View(await _entityService.GetEntitiesAsync());
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
        public async Task<ActionResult> Create(Event collection)
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

        /*
        public async Task<ActionResult> Create([Bind(Include = "imte,serialnumber,Notes,RowVersion")] Entity Entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entityservice = new EntityService<Event>();
                    var res = await entityservice.PostEntityAsync(Entity);
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(Entity);
        }        
        */

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
        public async Task<IActionResult> Edit(string id, [Bind("Id, Desc, Tag")] Event Entity)
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

        /*
        // POST: Entities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        */

        // GET: Entities/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            return View(await _entityService.GetEntityAsync(id));
        }

        /*
        // POST: Entities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                var res = await _entityService.DeleteEntityAsync(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        */

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
    }
}