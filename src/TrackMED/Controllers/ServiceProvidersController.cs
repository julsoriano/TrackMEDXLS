using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class ServiceProvidersController : MVCControllerWithHub<ProviderOfService>
    {
        public ServiceProvidersController(IEntityService<ProviderOfService> entityService, 
                                          IEntityService<Component> componentService) 
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

            var model = new DescComponentModel<ServiceProvider>();
            model.descRecords = allRecords
                        .OrderBy(x => x.Desc)
                        .ToList();

            if (id != null)
            {
                model.Id = id;
                var compRecords = await _componentService.GetEntitiesAsync();
                model.linkedComponents = compRecords
                      .OrderBy(x => x.imte)
                      .Where(x => x.ServiceProviderID == id)
                      .ToList();
            }
            return View(model);
        }
        */

        public async Task<IEnumerable<Component>> LoadComponents(string descId)
        {
            List<Component> compRecords = await _componentService.GetSelectedEntitiesAsync("ServiceProvider", descId);
            var items = compRecords
                          .OrderBy(x => x.imte)
                          //.Where(x => x.ServiceProviderID == descId)
                          .ToList();

            return items;
        }
    }
}