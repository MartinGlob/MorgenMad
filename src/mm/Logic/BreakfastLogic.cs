﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mm.Models;
using mm.DataStore;
using System.Collections;

namespace mm.Logic
{
    public class BreakfastLogic
    {
        IDataStore _ds;
        string _teamId;

        List<Person> _persons;
        List<Participant> _participants;

        public BreakfastLogic(IDataStore db, string teamId)
        {
            _ds = db;
            _teamId = teamId;
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
            var dates = from n in _participants
                    where n.Participating == Participation.Buying || n.Participating == Participation.Override
                    group n by n.PersonId into g
                    select new { Id = g.Key, When = g.Max(t => t.When) };

            var who = from d in dates
                      orderby d.When
                      join p in _persons on d.Id equals p.Id
                      where p.Deleted == null
                      select p;

            return who.ToList();
        }

        public BreakfastsView CreateEventList(int teamId, DateTime fromDate)
        {
            //todo fix id's
            _persons = new List<Person>(); // _ds.GetPersons("");
            _participants = new List<Participant>(); // _ds.GetParticipants("");

            var view = new BreakfastsView();

            DateTime nextDate = DateTime.Now;

            //todo this is the place to fix so any day can be breakfast day ;-)
            while (nextDate.DayOfWeek != DayOfWeek.Friday) { nextDate = nextDate.AddDays(1); } 

            var oldEventDates = (from p in _participants where p.When >= fromDate && p.When < nextDate select p.When).Distinct().ToList();

            foreach (var breakfastDate in oldEventDates)
            {
                var be = new Breakfast { When = breakfastDate };

                var participantGiving = _participants.FirstOrDefault(p => p.When == breakfastDate && (p.Participating == Participation.Buying || p.Participating == Participation.Override));

                if (participantGiving == null)
                {
                    be.Skipped = true;
                    continue;
                }

                be.Buying = _persons.FirstOrDefault(p => p.Id == participantGiving.PersonId);

                be.NotParticipating = (from np in _participants
                                       where np.When == breakfastDate && np.Participating == Participation.NotParticipating
                                       join p in _persons on np.PersonId equals p.Id
                                       select p).OrderBy(p => p.Id).ToList();

                be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Buying.Id && !be.NotParticipating.Any(np => np.Id == p.Id)).OrderBy(p => p.Name).ToList();

                view.Breakfasts.Add(be);
            }

            for (var i = 1; i < 12; i++)
            {
                var be = new Breakfast { When = nextDate };

                be.NotParticipating = (from np in _participants
                                       where np.When.Date == nextDate.Date && np.Participating == Participation.NotParticipating
                                       join p in _persons on np.PersonId equals p.Id
                                       select p).OrderBy(p => p.Id).ToList();

                be.Buying = (from b in _participants
                             where b.When.Date == nextDate.Date && (b.Participating == Participation.Buying || b.Participating == Participation.Override)
                             join p in _persons on b.PersonId equals p.Id
                             select p).FirstOrDefault();

                if (be.Buying == null)
                {
                    be.Buying = NextGiver(be.NotParticipating);
                }

                if (be.Buying != null)
                {
                    be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Buying.Id && !be.NotParticipating.Any(np => np.Id == p.Id)).OrderBy(p => p.Name).ToList();
                    //be.Buying.LastGave = be.When;

                    var idx = _participants.FindIndex(p => p.Participating == Participation.Buying && p.When == be.When);
                    if (idx >= 0)
                        _participants.RemoveAt(idx);

                 //   _participants.Add(new Participant { Participating = Participation.Buying, PersonId = be.Buying.Id, TeamId = _teamId, When = be.When });
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

        internal void ChangeParticipation(Participant p)
        {
            //_ds.RemoveParticipation(p.When, p.PersonId);
            //switch (p.Participating)
            //{
            //    case Participation.Override:
            //        _ds.RemoveSpecificParticipation(p.When, _teamId, Participation.Buying);
            //        _ds.RemoveSpecificParticipation(p.When, _teamId, Participation.Override);
            //        _ds.AddParticipant(p.When, _teamId, p.PersonId, Participation.Override);
            //        break;
            //    case Participation.Buying:     // was buying
            //    case Participation.Participating:     // was participating
            //        _ds.RemoveSpecificParticipation(p.When, _teamId, Participation.Override);
            //        _ds.AddParticipant(p.When, _teamId, p.PersonId, Participation.NotParticipating);
            //        break;
            //    case Participation.NotParticipating:     // was not participating
            //        break;
            //}
        }


        public void DailyWork()
        {

        }
    }
}

