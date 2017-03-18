using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mm.DataStore;
using mm.Logic;
using mm.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mm.Controllers
{
    public class HomeController : Controller
    {
        IMongoStore _ds;
        BreakfastLogic b;

        public HomeController(IMongoStore dataStore)
        {
            _ds = dataStore;
            b = new BreakfastLogic(_ds);
        }

        public async Task<IActionResult> Index(string submit)
        {
            if (!b.AuthenticateUser(User.Identity.Name))
                return RedirectToAction("NewUser");

            b.LoadPersons();
            b.LoadParticipants();
            return View(b.CreateEventList(DateTime.Now.AddDays(-21)));
        }

        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (!b.AuthenticateUser(User.Identity.Name))
                return await NewUser();

            b.LoadPersons();

            if (id != null)
            {
                var p = Breakfast.DecodeChangeId(id);
                b.ChangeParticipation(p);
            }

            b.LoadParticipants();

            return RedirectToAction("Index");
        }

        private IList<SelectListItem> GetTeams()
        {
            var l = new List<SelectListItem>();

            foreach (var t in _ds.GetTeams().Result)
            {
                l.Add(new SelectListItem() { Text = t.Name, Value = t.Id.ToString() });
            }
            return l;
        }

        [HttpGet]
        public async Task<IActionResult> NewUser()
        {
            var model = new EditTeamPerson() { Teams = GetTeams() };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewUser(EditTeamPerson model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Teams =GetTeams();
                    return View(model);
                }

                var parts = User.Identity.Name.Split('\\');
                var p = new Person(parts[1], model.Email, model.TeamId);
                await _ds.AddPerson(p);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                model.Message = ex.Message;
                model.Teams = GetTeams();
                return View("NewUser", model);
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
