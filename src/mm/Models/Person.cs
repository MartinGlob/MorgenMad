using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mm.Models
{
    public class Team
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DayOfWeek EventDay { get; set; }
        public List<Person> Persons { get; set; }

        public Team()
        {
            Persons = new List<Person>();
        }
    }

    public class Person
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
        public string EMail { get; set; }

        public Person()
        {
        }

        public bool WasActive(DateTime when)
        {
            return true;
        }
    }

   
    public enum EventStatus { Normal = 0, Skipped = 9, SeedEvent = 2 }

    public class Event
    {
        public Guid Id { get; set; }
        public string TeamId { get; set; }
        public DateTime When { get; set; }
        public EventStatus Status { get; set; }
    }

    public enum Participation { Buying=1, Participating=2, NotParticipating=3,Override=4 }

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

        public Guid TeamId { get; set; }
        public DateTime When { get; set; }
        public Guid PersonId { get; set; }
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

        public string GenChangeLink(Person person, Participation status)
        {
            // / Home / ChangeStatus / 123
            return $"/Home/ChangeStatus/{When:yyyyMMdd}:{person.Id}:{status.ToString("D")}";
        }
         
        public static Participant DecodeChangeId(string id)
        {
            var r = new Participant();

            var a = id.Split(':');
            r.When = DateTime.ParseExact(a[0], "yyyyMMdd", null);
            //r.PersonId = a[1];
            r.Participating =(Participation) Enum.Parse(typeof(Participation), a[2]);

            return r;
        }

    }

    public class BreakfastsView
    {
        public BreakfastsView()
        {
            Breakfasts = new List<Breakfast>();
        }

        public List<Breakfast> Breakfasts { get; set; }
    }
}
