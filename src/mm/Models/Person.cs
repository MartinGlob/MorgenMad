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
        public ObjectId Id { get; set; }
        public DateTime Created { get; set; }
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
        public ObjectId Id { get; set; }
        public ObjectId TeamId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
        public string EMail { get; set; }

        public Person()
        {
        }

        public bool WasActive(DateTime when)
        {
            if (Created.Date > when.Date)
                return false;
            if (Deleted != null && Deleted.Value.Date <= when.Date)
                return false;
            return true;
        }
    }

   
    public enum EventStatus { Normal = 0, Skipped = 9, SeedEvent = 2 }

   
    public enum Participation { Buying=1, Participating=2, NotParticipating=3,Override=4 }

    public class Participant
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId TeamId { get; set; }
        public DateTime When { get; set; }
        public ObjectId PersonId { get; set; }
        public Participation Participating { get; set; }

        public Participant() {}

        public Participant(DateTime when, ObjectId teamId, ObjectId personId, Participation participating)
        {
            When = when;
            TeamId = teamId;
            PersonId = personId;
            Participating = participating;
        }
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
            return $"/Home/ChangeStatus/{When:yyyyMMdd}:{person.Id}:{status.ToString("D")}";
        }
         
        public static Participant DecodeChangeId(string id)
        {
            var r = new Participant();

            var a = id.Split(':');
            r.When = DateTime.ParseExact(a[0], "yyyyMMdd", null);
            r.PersonId = ObjectId.Parse(a[1]);
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
