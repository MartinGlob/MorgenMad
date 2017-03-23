﻿using System;
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
            return $"ChangeStatus/{When.ToLocalTime():yyyyMMdd}/{person.Id}/{status.ToString("D")}";
        }
    }
}
