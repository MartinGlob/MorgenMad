using System;
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

            _persons = _ds.GetPersons(_teamId);
            _participants = _ds.GetParticipants(_teamId);
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
            var lastGave = (from n in _persons
                            where n.Deleted == null
                            group n by n.Id into g
                            select g.OrderByDescending(t => t.LastGave).FirstOrDefault()).ToList().OrderBy(p => p.LastGave);


            var oldest = lastGave.FirstOrDefault(p => p.LastGave != null);
            var seedDate = oldest?.LastGave ?? DateTime.Now.Date;

            foreach (var p in lastGave.Where(x => x.LastGave == null))
            {
                p.LastGave = seedDate;
                seedDate = seedDate.AddSeconds(1);
            }

            return lastGave.ToList();
        }

        public BreakfastsView CreateEventList(int teamId, DateTime fromDate)
        {

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

                be.Giving = _persons.FirstOrDefault(p => p.Id == participantGiving.PersonId);

                be.NotParticipating = (from np in _participants
                                       where np.When == breakfastDate && np.Participating == Participation.NotParticipating
                                       join p in _persons on np.PersonId equals p.Id
                                       select p).OrderBy(p => p.UserId).ToList();

                be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Giving.Id && !be.NotParticipating.Any(np => np.Id == p.Id)).OrderBy(p => p.UserId).ToList();

                view.Breakfasts.Add(be);
            }

            for (var i = 1; i < 12; i++)
            {
                var be = new Breakfast { When = nextDate };

                be.NotParticipating = (from np in _participants
                                       where np.When.Date == nextDate.Date && np.Participating == Participation.NotParticipating
                                       join p in _persons on np.PersonId equals p.Id
                                       select p).OrderBy(p => p.UserId).ToList();

                be.Giving = NextGiver(be.NotParticipating);

                if (be.Giving != null)
                {
                    be.Participating = _persons.Where(p => p.WasActive(be.When) && p.Id != be.Giving.Id && !be.NotParticipating.Any(np => np.Id == p.Id)).OrderBy(p => p.UserId).ToList();
                    be.Giving.LastGave = be.When;
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



        //private List<Participant> ParticipatingIn(DateTime breakfastDate, Participation participatesAs)
        //{
        //    var l = _participants.FindAll(p => p.When == breakfastDate && p.Participating == participatesAs).ToList();
        //    return l;
        //}

        internal void ChangeParticipation(Participant p)
        {
            switch (p.Participating)
            {
                case Participation.Buying:     // was buying
                case Participation.Participating:     // was participating
                    _ds.AddParticipant(p.When, _teamId, p.PersonId, Participation.NotParticipating);
                    _participants.Add(new Participant(p) { Participating = Participation.NotParticipating });
                    break;
                case Participation.NotParticipating:     // was not participating
                    _ds.RemoveParticipation(p.When, p.PersonId);
                    var idx = _participants.FindIndex(x => x.When.Date == p.When.Date && x.PersonId == p.PersonId);
                    _participants.RemoveAt(idx);
                    break;
            }
        }


        public void DailyWork()
        {

        }
    }
}

