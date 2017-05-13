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

        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            if (!await b.AuthenticateUser(User.Identity.Name))
                return RedirectToAction("NewUser");

            return View(b.CreateEventList(10, 40));
        }

        [HttpGet("ChangeStatus/{when}/{id}/{status}")]
        public async Task<IActionResult> ChangeStatus(string when, string id, string status)
        {
            if (!await b.AuthenticateUser(User.Identity.Name))
                return RedirectToAction("NewUser");

            try
            {
                var p = new Participant(when, id, status);

                var person = await _ds.GetPerson(p.PersonId);

                if (p.When < DateTime.UtcNow.Date || p.When.ToLocalTime().DayOfWeek != b.Team.EventDay)
                    throw new Exception($"{p.When} is in the past or wrong day");

                if (person == null || person.TeamId != b.User.TeamId)
                    throw new Exception($"{id} not found or is in another team");

                b.ChangeParticipation(p);

                await _ds.Log($"{b.User.Id} changed {p.PersonId} {p.When.ToString("yyyy-MM-dd")} {p.Participating}");

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                await _ds.Log($"{b.User.Id} send bad data in a ChangeStatus request [{when}/{id}/{status}: {ex.Message}]");
                return View("Error", new ErrorModel("Something went wrong...\nThe error is loggged and someone might look into it..."));
            }
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
        public IActionResult NewUser()
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
                    model.Teams = GetTeams();
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

        [HttpGet("Housekeeping")]
        public async Task<IActionResult> Housekeeping()
        {
            var teams = _ds.GetTeams().Result;
            foreach (var t in teams)
            {
                b.User = new Person { TeamId = t.Id };
                var evt = b.CreateEventList(0, 1, reloadTeam: true);
            }

            return Ok();
        }

        public IActionResult Error(string errorMessage)
        {
            return View();
        }
    }
}
