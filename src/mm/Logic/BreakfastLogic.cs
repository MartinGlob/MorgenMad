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
        int _teamId;

        List<Person> _persons;
        List<Participant> _participants;

        public BreakfastLogic(IDataStore db, int teamId)
        {
            _ds = db;
            _teamId = teamId;
        }

        public Person NextGiver(List<Person> notParticipating)
        {
            var list = NextGiverList();

            foreach (var g in list)
            {
                if (notParticipating.Exists(p => p.Id == g.Id))
                    continue;

                return g;
            }

            return null;
        }

        public List<Person> NextGiverList()
        {
            var givers = _persons.FindAll(x => x.Deleted == null);

            Person oldest = new Person { LastGave = DateTime.MaxValue };

            foreach (var p in _participants.FindAll(p => p.Participating == Participation.Buying))
            {
                var giver = givers.Find(g => g.Id == p.PersonId);
                if (giver == null)
                    continue;
                giver.LastGave = p.When > giver.LastGave ? p.When : giver.LastGave;

                if (giver.LastGave < oldest.LastGave)
                    oldest = giver;
            }

            givers = givers.OrderBy(g => g.LastGave).ToList();

            var seedDate = oldest?.LastGave ?? DateTime.Now.Date;

            foreach (var p in givers.Where(x => x.LastGave == null))
            {
                p.LastGave = seedDate;
                seedDate = seedDate.AddSeconds(1);
            }

            return givers.ToList();
        }

        public BreakfastsView CreateEventList(int teamId, DateTime fromDate)
        {
            _persons = _ds.GetPersons(_teamId);
            _participants = _ds.GetParticipants(_teamId);

            var view = new BreakfastsView();

            DateTime nextDate = DateTime.Now;
            while (nextDate.DayOfWeek != DayOfWeek.Friday) { nextDate = nextDate.AddDays(1); } //todo using team settings instead of fixed day

            var oldEventDates = (from p in _participants where p.When >= fromDate && p.When < nextDate select p.When).Distinct().ToList();

            foreach (var breakfastDate in oldEventDates)
            {
                var be = new Breakfast { When = breakfastDate };

                var participantGiving = _participants.FirstOrDefault(p => p.When == breakfastDate && p.Participating == Participation.Buying);

                if (participantGiving == null)
                {
                    be.Skipped = true;
                    continue;
                }

                be.Buying = _persons.FirstOrDefault(p => p.Id == participantGiving.PersonId);

                be.NotParticipating = (from np in _participants
                                       where np.When == breakfastDate && np.Participating == Participation.NotParticipating
                                       join p in _persons on np.PersonId equals p.Id
                                       select p).OrderBy(p => p.UserId).ToList();

                be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Buying.Id && !be.NotParticipating.Any(np => np.Id == p.Id)).OrderBy(p => p.UserId).ToList();

                view.Breakfasts.Add(be);
            }

            for (var i = 1; i < 12; i++)
            {
                var be = new Breakfast { When = nextDate };

                be.NotParticipating = (from np in _participants
                                       where np.When.Date == nextDate.Date && np.Participating == Participation.NotParticipating
                                       join p in _persons on np.PersonId equals p.Id
                                       select p).OrderBy(p => p.UserId).ToList();

                be.Buying = (from b in _participants
                             where b.When.Date == nextDate.Date &&( b.Participating == Participation.Buying || b.Participating == Participation.Override)
                             join p in _persons on b.PersonId equals p.Id
                             select p).FirstOrDefault();

                if (be.Buying == null)
                {
                    be.Buying = NextGiver(be.NotParticipating);
                }

                if (be.Buying != null)
                {
                    be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Buying.Id && !be.NotParticipating.Any(np => np.Id == p.Id)).OrderBy(p => p.UserId).ToList();
                    be.Buying.LastGave = be.When;

                    var idx = _participants.FindIndex(p => p.Participating == Participation.Buying && p.When == be.When);
                    if (idx >= 0)
                        _participants.RemoveAt(idx);

                    _participants.Add(new Participant { Participating = Participation.Buying, PersonId = be.Buying.Id, TeamId = _teamId, When = be.When });
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
            _ds.RemoveParticipation(p.When, p.PersonId);
            switch (p.Participating)
            {
                case Participation.Override:
                    _ds.RemoveSpecificParticipation(p.When, _teamId, Participation.Buying);
                    _ds.AddParticipant(p.When, _teamId, p.PersonId, Participation.Buying);
                    break;
                case Participation.Buying:     // was buying
                case Participation.Participating:     // was participating
                    _ds.AddParticipant(p.When, _teamId, p.PersonId, Participation.NotParticipating);
                    break;
                case Participation.NotParticipating:     // was not participating
                    break;
            }
        }


        public void DailyWork()
        {

        }
    }
}
