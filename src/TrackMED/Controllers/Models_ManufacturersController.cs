using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class Models_ManufacturersController : MVCControllerWithHub<Model_Manufacturer>
    {
        public Models_ManufacturersController(IEntityService<Model_Manufacturer> entityService, IEntityService<Component> componentService) 
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

            var model = new DescComponentModel<Model>();
            model.descRecords = allRecords
                        .OrderBy(x => x.Desc)
                        .ToList();

            if (id != null)
            {
                model.Id = id;
                var compRecords = await _componentService.GetEntitiesAsync();
                model.linkedComponents = compRecords
                      .OrderBy(x => x.imte)
                      .Where(x => x.ModelID == id)
                      .ToList();
            }
            return View(model);
        }
        */
        public async Task<IEnumerable<Component>> LoadComponents(string descId)
        {
            List<Component> compRecords = await _componentService.GetSelectedEntitiesAsync("Model_Manufacturer", descId);
            var items = compRecords
                          .OrderBy(x => x.imte)
                          //.Where(x => x.Model_ManufacturerID == descId)
                          .ToList();

            return items;
        }
    }
}