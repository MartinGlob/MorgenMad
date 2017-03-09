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
        IMongoStore _ds;

        public HomeController(IMongoStore dataStore)
        {
            _ds = dataStore;
            
        }

        public IActionResult Index(string submit)
        {
            var b = new BreakfastLogic(_ds);
            b.LoadPersons();
            b.LoadParticipants();
            //if (submit != null)
            //{
            //    var p = Breakfast.DecodeChangeId(submit);

            //    b.ChangeParticipation(p);
            //}

            return View(b.CreateEventList(DateTime.Now.AddDays(-21)).Result);
        }

        public IActionResult ChangeStatus(string id)
        {
            var b = new BreakfastLogic(_ds);
            b.LoadPersons();

            if (id != null)
            {
                var p = Breakfast.DecodeChangeId(id);
                b.ChangeParticipation(p);
            }

            b.LoadParticipants();

            return View("Index",b.CreateEventList(DateTime.Now.AddDays(-21)).Result);
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
