using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mm.Models;
using mm.DataStore;
using System.Collections;

namespace mm.Logic
{
    public class Breakfast
    {
        IDataStore _db;


        public Breakfast(IDataStore db)
        {
            _db = db;
        }

        public List<Person> WhoIsNextList(List<Person> persons)
        {
            var oldestGiver = persons.FindAll(p => p.Deleted == null && p.LastGave != null).Min(p => p.LastGave);

            var list = persons.FindAll(p => p.Deleted == null).OrderBy(p => p.Created);

            var seedDateTime = oldestGiver ?? DateTime.Now;
            foreach(var person in list)
            {
                if (person.LastGave != null)
                    continue;
                seedDateTime = seedDateTime.AddSeconds(1);
                person.LastGave = seedDateTime;
            }

            return list.OrderBy(p => p.LastGave).ToList();
        }

        public EventsView CreateEventList(int teamId)
        {

            var nextGivers = WhoIsNextList(_db.GetPersons(teamId));

            //Dictionary<int, DateTime?> _lastGave = new Dictionary<int, DateTime?>();

            //var events = _db.GetEvents(teamId, DateTime.MinValue);

            //var deltagere = _db.GetParticipations(teamId);

            var view = new EventsView();
            //view.Persons = _db.GetPersons(teamId);


            //foreach (var evt in events)
            //{

            //    var e = new EventListItem();
            //    e.WhenDisplay = evt.When.ToString("dd.MM");
            //    e.WhenId = evt.When.ToString("yyyyMMdd");
            //    e.Skipped = evt.Status == EventStatus.Skipped;

            //    if (e.Skipped)
            //    {
            //        view.Events.Add(e);
            //        continue;
            //    }

            //    e.Giving = (from d in deltagere
            //                where d.When == evt.When && d.Participating == Participation.Buying
            //                join p in view.Persons on d.PersonId equals p.Id
            //                select p).Single();

            //    if (!e.Giving.Deleted)
            //    {
            //        _lastGave[e.Giving.Id] = evt.When;
            //    }

            //    if (evt.When < DateTime.Now.AddDays(-7))
            //        continue;

            //    e.NotParticipating = (from d in deltagere
            //                          where d.When == evt.When && d.Participating == Participation.NotParticipating
            //                          join p in view.Persons on d.PersonId equals p.Id
            //                          select p).ToList();

            //    e.Participating = view.Persons.Where(x => !e.NotParticipating.Any(np => np.Id == x.Id)).ToList();

                   

            //    view.Events.Add(e);
            //}

            ////todo add fake lastBought dates to those who haven't given yet

            ////todo check if empty
            //var nextEventDate = events.Last().When;

            //for (int i = 1; i < 12; i++)
            //{
            //    nextEventDate = nextEventDate.AddDays(7);

            //    var e = new EventListItem();
            //    e.WhenDisplay = nextEventDate.ToString("dd.MM");
            //    e.WhenId = nextEventDate.ToString("yyyyMMdd");
            //    e.Skipped = false;

            //    e.NotParticipating = (from d in deltagere
            //                          where d.When == nextEventDate && d.Participating == Participation.NotParticipating
            //                          select d.PersonId).ToList();

            //    var nextGiver = -1;

            //    foreach (var next in _lastGave.OrderBy(x => x.Value))
            //    {
            //        if (!e.NotParticipating.Contains(next.Key))
            //        {
            //            nextGiver = next.Key;
            //            break;
            //        }
            //    }

            //    if (nextGiver > 0)
            //    {
            //        e.Giving = nextGiver;
            //        _lastGave[e.Giving] = nextEventDate;
            //    }
            //    else
            //    {
            //        e.Giving = 0;
            //    }

            //    e.Participating = (from p in view.Persons
            //                       orderby p.UserId
            //                       where !p.Deleted && p.Id != e.Giving && !e.NotParticipating.Contains(p.Id)
            //                       select p.Id).ToList();

            //    view.Events.Add(e);
            //}

            return view;
        }

        internal void ChangeParticipation(DateTime when, int who, int oldState)
        {
            int teamId = 1;

            switch (oldState)
            {
                case 0:     // was buying
                case 1:     // was participating
                    _db.AddParticipation(when, teamId, who, Participation.NotParticipating);
                    break;
                case 2:     // was not participating
                    _db.RemoveParticipation(when, who);
                    break;
            }
        }

        
        public void DailyWork()
        {

        }
    }
}
