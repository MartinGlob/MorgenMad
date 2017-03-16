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
        List<Person> _persons;
        List<Participant> _participants;

        public Person User { get; set; }

        public BreakfastLogic(IMongoStore db)
        {
            _ds = db;
        }

        //todo Cache user
        public bool AuthenticateUser(string WindowsUser)
        {
            if (!string.IsNullOrWhiteSpace(WindowsUser))
            {
                // split domain/userid
                var parts = WindowsUser.Split('\\');
                User = _ds.GetPerson(parts[1]).Result;
            }
            return User != null;
        }

        public void LoadPersons()
        {
            _persons = _ds.GetPersons(User.TeamId).Result;
        }

        public void LoadParticipants()
        {
            _participants = _ds.GetParticipants(User.TeamId).Result;
        }

        public Person NextGiver(List<Person> notParticipating)
        {
            var list = WhoIsNextList();

            foreach (var g in list)
            {
                if (notParticipating.Exists(p => p.Id == g.Id))
                    continue;

                return g;
            }

            return null;
        }

        public List<Person> WhoIsNextList()
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

            return who;
        }

        public BreakfastsView CreateEventList(DateTime fromDate)
        {
            var view = new BreakfastsView();

            DateTime nextDate = DateTime.Today.ToUniversalTime();

            //todo this is the place to fix so any day can be breakfast day ;-)
            while (nextDate.ToLocalTime().DayOfWeek != DayOfWeek.Friday) { nextDate = nextDate.AddDays(1); }

            if (_participants == null)
                throw new Exception("PART IS NULL");

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

            for (var i = 1; i < 12; i++)
            {
                var be = new Breakfast { When = nextDate };

                be.NotParticipating = (from np in _participants
                                       where np.Participating == Participation.NotParticipating && IsSameDate(np.When, nextDate)
                                       join p in _persons on np.PersonId equals p.Id
                                       select p).OrderBy(p => p.Id).ToList();

                be.Buying = (from b in _participants
                             where (b.Participating == Participation.Buying || b.Participating == Participation.Override) && IsSameDate(b.When, nextDate)
                             join p in _persons on b.PersonId equals p.Id
                             select p).FirstOrDefault();

                if (be.Buying == null)
                {
                    be.Buying = NextGiver(be.NotParticipating);
                    be.BuyerStatus = Participation.Buying;
                }
                else
                {
                    be.BuyerStatus = Participation.Override;
                }

                if (be.Buying != null)
                {
                    be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Buying.Id && be.NotParticipating.All(np => np.Id != p.Id)).OrderBy(p => p.Name).ToList();

                    _participants.RemoveAll(p => p.Participating == Participation.Buying && IsSameDate(p.When, be.When));

                    _participants.Add(new Participant { Participating = Participation.Buying, PersonId = be.Buying.Id, TeamId = User.TeamId, When = be.When });
                }
                else
                {
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

