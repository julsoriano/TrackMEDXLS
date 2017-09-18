using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class ActivityTypesController : MVCControllerWithHub<ActivityType>
    {
        public ActivityTypesController(IEntityService<ActivityType> entityService, IEntityService<Component> componentService) 
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

        public async Task<IEnumerable<Component>> LoadComponents(string descId)
        {
            List<Component> compRecords = await _componentService.GetSelectedEntitiesAsync("ActivityType", descId);

            var items = compRecords
                          .OrderBy(x => x.imte)
                          .ToList();

            return items;
        }
    }
}