using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mm.DataStore;
using mm.Logic;
using mm.Models;

namespace mm.Controllers
{
    public class HomeController : Controller
    {
        IDataStore _ds;

        public HomeController(IDataStore dataStore)
        {
            _ds = dataStore;
            
        }

        public IActionResult Index(string submit)
        {
            var b = new BreakfastLogic(_ds,2);

            if (submit != null)
            {
                var p = Breakfast.DecodeChangeId(submit);

                b.ChangeParticipation(p);
            }

            return View(b.CreateEventList(2, DateTime.Now.AddDays(-21)));
        }

        public IActionResult ChangeStatus(string id)
        {
            var b = new BreakfastLogic(_ds, 2);

            if (id != null)
            {
                var p = Breakfast.DecodeChangeId(id);

                b.ChangeParticipation(p);
            }

            return View("Index",b.CreateEventList(2, DateTime.Now.AddDays(-21)));
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
