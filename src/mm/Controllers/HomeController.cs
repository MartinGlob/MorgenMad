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

            //todo determine team based upon user
            //var x = User.Identity.Name;
        }

        public IActionResult Index(string submit)
        {
            var b = new BreakfastLogic(_ds);
            return View("NewUser");
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

        [HttpPost]
        public async Task<IActionResult> NewUser(string id, string email, string teamId)
        {
            var model = new EditTeamPerson();

            model.Teams = await _ds.GetTeams();

            return View("NewUser", model);
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
