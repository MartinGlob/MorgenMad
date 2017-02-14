using mm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class TestData
    {
        public static List<Person> CreatePersons(int number, bool noLastGaveDate = false)
        {
            var l = new List<Person>();

            var nextDate = new DateTime(2000, 1, 1);
            for (var i = 1; i <= number; i++)
            {
                l.Add(new Person
                {
                    Id = i,
                    UserId = $"User{i}",
                    LastGave = noLastGaveDate ? null : (DateTime?) nextDate
                });

                nextDate = nextDate.AddDays(7);
            }
            return l;
        }
    }
}
