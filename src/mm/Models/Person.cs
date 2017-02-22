using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mm.Models
{
    public class Person
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
        public string UserId { get; set; }
        public string EMail { get; set; }
        public DateTime? LastGave { get; set; }

        public Person()
        {
            Created = DateTime.MinValue;
        }

        public Person(Person p)
        {
            Id = p.Id;
            TeamId = p.TeamId;
            Created = p.Created;
            Deleted = p.Deleted;
            UserId = p.UserId;
            EMail = p.EMail;
            LastGave = p.LastGave;
        }

        internal bool WasActive(DateTime when)
        {
            if (Created > when)
                return false;
            if (Deleted.HasValue && Deleted.Value <= when)
                return false;
            return true;
        }
    }

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DayOfWeek EventDay { get; set; }
    }

    public enum EventStatus { Normal = 0, Skipped = 9, SeedEvent = 2 }

    public class Event
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public DateTime When { get; set; }
        public EventStatus Status { get; set; }
    }

    public enum Participation { Buying=1, Participating=2, NotParticipating=3 }

    public class Participant
    {
        public Participant()
        {

        }

        public Participant(Participant p)
        {
            TeamId = p.TeamId;
            When = p.When;
            PersonId = p.PersonId;
            Participating = p.Participating;
        }

        public int TeamId { get; set; }
        public DateTime When { get; set; }
        public int PersonId { get; set; }
        public Participation Participating { get; set; }
    }

    public class Breakfast
    {
        public DateTime When { get; set; }
        public bool Skipped { get; set; }
        public string WhenDisplay { get; set; }
        public Person Buying { get; set; }
        public List<Person> Participating { get; set; }
        public List<Person> NotParticipating { get; set; }

        public string CreateClickId(Person person, Participation status)
        {
            return $"{When:yyyyMMdd}:{person.Id}:{status.ToString("D")}";
        }

        public static Participant DecodeClickId(string id)
        {
            var r = new Participant();

            var a = id.Split(':');
            r.When = DateTime.ParseExact(a[0], "yyyyMMdd", null);
            r.PersonId = int.Parse(a[1]);
            r.Participating =(Participation) Enum.Parse(typeof(Participation), a[2]);

            return r;
        }

    }

    public class BreakfastsView
    {
        public BreakfastsView()
        {
            //Persons = new List<Person>();
            Breakfasts = new List<Breakfast>();
        }

        //public List<Person> Persons { get; set; }
        public List<Breakfast> Breakfasts { get; set; }

        //public Person GetPerson(int id)
        //{
        //    return Persons.Find(p => p.Id == id);
        //}

        //public string GetName(int id)
        //{
        //    var p = Persons.Find(x => x.Id == id);
        //    return p == null ? "" : p.UserId;
        //}
    }
}
