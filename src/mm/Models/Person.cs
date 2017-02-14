using System;
using System.Collections.Generic;
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
            Created = DateTime.Now;
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

    public enum Participation { Buying, Participating, NotParticipating }

    public class Participant
    {
        public int TeamId { get; set; }
        public DateTime When { get; set; }
        public int PersonId { get; set; }
        public Participation Participating { get; set; }
    }

    public class EventListItem
    {
        public bool Skipped { get; set; }
        public string WhenDisplay { get; set; }
        public string WhenId { get; set; }
        public Person Giving { get; set; }
        public List<Person> Participating { get; set; }
        public List<Person> NotParticipating { get; set; }
    }

    public class EventsView
    {
        public EventsView()
        {
            //Persons = new List<Person>();
            Events = new List<EventListItem>();
        }

        //public List<Person> Persons { get; set; }
        public List<EventListItem> Events { get; set; }

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
