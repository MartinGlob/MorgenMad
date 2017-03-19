using System;
using System.Collections.Generic;

namespace mm.Models
{
    public class Breakfast
    {
        public DateTime When { get; set; }
        public bool Skipped { get; set; }
        public string WhenDisplay { get; set; }
        public Person Buying { get; set; }
        public Participation BuyerStatus { get; set; }
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
}
