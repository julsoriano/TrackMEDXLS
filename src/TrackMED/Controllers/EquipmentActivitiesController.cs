using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class EquipmentActivitiesController : MVCControllerWithHub<EquipmentActivity>
    {
        public EquipmentActivitiesController(IEntityService<EquipmentActivity> entityService, IEntityService<Component> componentService) 
            : base(entityService, componentService)
        {
        }

        // GET: Entities
        public async Task<ActionResult> Index()
        {
            var allRecords = await _entityService.GetEntitiesAsync();
            var items = allRecords
                        .OrderBy(x => x.DeploymentID);
            return View(items);
        }
    }
}