using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mm.DataStore;
using mm.Logic;

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
            var b = new Breakfast(_ds);

            if (submit != null)
            {
                var a = submit.Split(':');
                var when = DateTime.ParseExact(a[0], "yyyyMMdd", null);
                var who = int.Parse(a[1]);
                var oldState = int.Parse(a[2]);

                b.ChangeParticipation(when, who, oldState);
            }

            return View(b.CreateEventList(1));
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
