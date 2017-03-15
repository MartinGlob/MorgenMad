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
        BreakfastLogic _b;

        public HomeController(IMongoStore dataStore)
        {
            _ds = dataStore;
            var b = new BreakfastLogic(_ds);
        }

        public async Task<IActionResult> Index(string submit)
        {
            var b = new BreakfastLogic(_ds);
            //todo show this if User.Identity is unknown -
            //return View("NewUser");
            return await NewUser(null);

            b.LoadPersons();
            b.LoadParticipants();
            return View(b.CreateEventList(DateTime.Now.AddDays(-21)));
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

            return View("Index", b.CreateEventList(DateTime.Now.AddDays(-21)));
        }

        public async Task<IActionResult> NewUser(EditTeamPerson model)
        {
            switch (Request.Method)
            {
                case "GET":
                    model = new EditTeamPerson()
                    {
                        Teams = await _ds.GetTeams()
                    };
                    return View("NewUser", model);
                case "POST":
                    return View("NewUser", model);
                default:
                    return Error();
            }

        }

        //public IActionResult About()
        //{
        //    ViewData["Message"] = "Your application description page.";

        //    return View();
        //}

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        public IActionResult Error()
        {
            return View();
        }
    }
}
