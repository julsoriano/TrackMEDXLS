using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class LocationsController : MVCControllerWithHub<Location>
    {
        public LocationsController(IEntityService<Location> entityService, 
                                   IEntityService<Component> componentService,
                                   IEntityService<SystemTab> systemtabService) 
            : base(entityService, componentService, systemtabService)
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

        public async Task<IEnumerable<Component>> LoadComponents(string descId)
        {
            List<Component> compRecords = await _componentService.GetSelectedEntitiesAsync("Location", descId);
            var items = compRecords
                          .OrderBy(x => x.imte)
                          .ToList();

            return items;
        }

        [HttpGet]
        public async Task<IEnumerable<SystemTab>> LoadSystems(string descId)
        {
            List<SystemTab> systemRecords = await _systemtabService.GetSelectedEntitiesAsync("Location", descId);

            var items = systemRecords
                          .OrderBy(x => x.imte)
                          .ToList();

            return items;
        }
    }
}