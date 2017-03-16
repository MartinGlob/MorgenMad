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
        //public List<Person> Persons { get; set; }

        public Team()
        {
            //Persons = new List<Person>();
        }
    }

    public class Person
    {
        [BsonId]
        public string Id { get; set; } // B number
        public ObjectId TeamId { get; set; }
        public string Name { get; set; } // Short name - first part of email
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
        public string EMail { get; set; }

        public Person()
        {
            Created = DateTime.UtcNow;
        }

        public Person(string id, string email, string teamId)
        {
            Created = DateTime.Now;
            Id = id;
            TeamId = ObjectId.Parse(teamId);
            Name = email.Split('@')[0].ToUpper();
            EMail = email.ToLower();
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

    public class EditTeamPerson
    {
        public string Message { get; set; }

        public List<Team> Teams { get; set; }
        public string TeamName { get; set; }
        public DayOfWeek Day { get; set; }
        public string TeamId { get; set; }

        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
   
    public enum EventStatus { Normal = 0, Skipped = 9, SeedEvent = 2 }

   
    public enum Participation { Buying=1, Participating=2, NotParticipating=3,Override=4 }

    public class Participant
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId TeamId { get; set; }
        public DateTime When { get; set; }
        public string PersonId { get; set; }
        public Participation Participating { get; set; }

        public Participant() {}

        public Participant(DateTime when, ObjectId teamId, string personId, Participation participating)
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
            return $"/Home/ChangeStatus/{When.ToLocalTime():yyyyMMdd}:{person.Id}:{status.ToString("D")}";
        }
         
        public static Participant DecodeChangeId(string id)
        {
            var r = new Participant();

            var a = id.Split(':');
            r.When =  DateTime.ParseExact(a[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.AssumeLocal).ToUniversalTime();
            
            r.PersonId = a[1];
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
