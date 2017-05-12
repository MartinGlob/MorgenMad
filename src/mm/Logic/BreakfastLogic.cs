using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mm.Models;
using mm.DataStore;
using System.Collections;
using MongoDB.Bson;

namespace mm.Logic
{
    public class BreakfastLogic
    {
        IMongoStore _ds;
        List<Person> _personsf;
        List<Participant> _participantsf;

        public Person User { get; set; }
        public Team Team { get; set; }
        public Calendar Calendar{ get; set; }

        public BreakfastLogic(IMongoStore db)
        {
            _ds = db;
        }

        private List<Person> _persons { get
            {
                if (_personsf == null)
                {
                    if (User == null)
                        throw new Exception("User has not been authenticated before use of _persons");
                    _personsf = _ds.GetPersons(User.TeamId).Result;
                }
                return _personsf;
            }
        }

        private List<Participant> _participants
        {
            get
            {
                if (_participantsf == null)
                {
                    if (User == null)
                        throw new Exception("User has not been authenticated before use of _participants");
                    _participantsf = _ds.GetParticipants(User.TeamId).Result;
                }
                return _participantsf;
            }
        }

        //todo Cache user
        public async Task<bool> AuthenticateUser(string WindowsUser)
        {
            if (!string.IsNullOrWhiteSpace(WindowsUser))
            {
                // split domain/userid
                var parts = WindowsUser.Split('\\');
                User = _ds.GetPerson(parts[1]).Result;
                if (User != null)
                {
                    Team = await _ds.GetTeam(User.TeamId);
                    Calendar = await _ds.GetCalendar(Team.CalendarId);
                }
            }
            return User != null;
        }

        public NextBuyerClass WhoIsNextList()
        {
            // first create a list of those who actually gave breakfast
            var dates = from n in _participants
                        where n.Participating == Participation.Buying || n.Participating == Participation.Override
                        group n by n.PersonId into g
                        select new { Id = g.Key, When = g.Max(t => t.When) };

            // convert that to a list of persons
            var who = (from d in dates
                       orderby d.When
                       join p in _persons on d.Id equals p.Id
                       where p.Deleted == null
                       select p).ToList();

            // now find persons who never gave breakfast
            var never = _persons.FindAll(p => (p.Deleted == null) && !who.Any(w => w.Id == p.Id)).OrderBy(n => n.Created);

            // those who never gave, should give after the next in line 
            // todo implement  a selectable way of determining when new persons should buy breakfast

            if (who.Any())
                who.InsertRange(1, never);
            else
                who.AddRange(never);

            return new NextBuyerClass(who);
        }

        public BreakfastsView CreateEventList(int numberOfPreviousWeeks, int numberOfWeeksToShow, bool reloadTeam = false, string errorMessage = null)
        {
            if (reloadTeam)
            {
                _participantsf = null;
                _personsf = null;

            }

            var view = new BreakfastsView();

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                view.ErrorMessage = errorMessage;
            }

            DateTime nextDate = DateTime.Today.ToUniversalTime();

            //todo this is the place to fix so any day can be breakfast day ;-)
            while (nextDate.ToLocalTime().DayOfWeek != DayOfWeek.Friday) { nextDate = nextDate.AddDays(1); }

            if (numberOfPreviousWeeks > 0)
            {
                var fromDate = DateTime.Now.AddDays(-7 * numberOfPreviousWeeks);
                var oldEventDates = (from p in _participants where p.When >= fromDate && p.When < nextDate select p.When).Distinct().ToList();

                foreach (var breakfastDate in oldEventDates)
                {
                    var be = new Breakfast { When = breakfastDate };

                    var participantGiving = _participants.FirstOrDefault(p => (p.Participating == Participation.Buying || p.Participating == Participation.Override) && IsSameDate(p.When, breakfastDate));

                    if (participantGiving == null)
                    {
                        be.Skipped = true;
                        continue;
                    }

                    be.BuyerStatus = Participation.Buying;

                    be.Buying = _persons.FirstOrDefault(p => p.Id == participantGiving.PersonId);

                    be.NotParticipating = (from np in _participants
                                           where np.Participating == Participation.NotParticipating && IsSameDate(np.When, breakfastDate)
                                           join p in _persons on np.PersonId equals p.Id
                                           select p).OrderBy(p => p.Id).ToList();

                    be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Buying.Id && !be.NotParticipating.Any(np => np.Id == p.Id)).OrderBy(p => p.Name).ToList();

                    view.Breakfasts.Add(be);
                }
            }

            var next = WhoIsNextList();

            var isNext = true;

            for (var i = 1; i <= numberOfWeeksToShow; i++)
            {
                var be = new Breakfast { When = nextDate };

                be.IsNext = isNext;
                isNext = false;

                // create a list of those listed as not participating 
                be.NotParticipating = (from np in _participants
                                       where np.Participating == Participation.NotParticipating && IsSameDate(np.When, nextDate)
                                       join p in _persons on np.PersonId equals p.Id
                                       select p).OrderBy(p => p.Id).ToList();

                // check if some one is listed as the buyer (todo Override is probably the only status to check for)
                be.Buying = (from b in _participants
                             where (b.Participating == Participation.Buying || b.Participating == Participation.Override) && IsSameDate(b.When, nextDate)
                             join p in _persons on b.PersonId equals p.Id
                             select p).FirstOrDefault();

                if (be.Buying != null)
                {
                    // found one..
                    be.BuyerStatus = Participation.Override;
                }
                else
                {
                    // FInd next buyer that is not listed as not participating
                    if (be.NotParticipating.Count < next.Count())
                    {
                        int idx = 0;
                        while (be.NotParticipating.Exists(x => x.Id == next.Peek(idx).Id))
                        {
                            idx++;
                        }
                        be.Buying = next.Peek(idx);
                        next.MoveToEnd(be.Buying);
                        be.BuyerStatus = Participation.Buying;
                    }
                }

                if (be.Buying != null)
                {
                    // next buyer was found!
                    be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Buying.Id && be.NotParticipating.All(np => np.Id != p.Id)).OrderBy(p => p.Name).ToList();
                    next.MoveToEnd(be.Buying);
                }
                else
                {
                    // next buyer was not found - none one was participating on that date
                    be.Participating = new List<Person>();
                }

                view.Breakfasts.Add(be);

                nextDate = nextDate.AddDays(7);
            }

            return view;
        }

        private bool IsSameDate(DateTime a, DateTime b)
        {
            return a.ToUniversalTime().Date == b.ToUniversalTime().Date;
        }

        internal void ChangeParticipation(Participant p)
        {
            p.TeamId = _persons.Find(x => x.Id == p.PersonId).TeamId;
            _ds.RemoveAndInsert(p);
        }


        public void DailyWork()
        {

        }
    }
}
