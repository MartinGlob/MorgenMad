using mm.DataStore;
using mm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class TestData
    {
        public static void AddPersons(IDataStore ds, int number, bool noLastGaveDate = false)
        {
            var l = new List<Person>();

            var nextDate = new DateTime(2000, 1, 1);
            for (var i = 1; i <= number; i++)
            {
                ds.AddPerson(new Person
                {
                    Id = i,
                    TeamId = 1,
                    UserId = $"User{i}",
                    LastGave = noLastGaveDate ? null : (DateTime?)nextDate
                });

                nextDate = nextDate.AddDays(7);
            }
        }

        public static void Add34AP(IDataStore ds)
        {
            ds.AddPerson(new Person { TeamId = 1, UserId = "CAJRG", Deleted = new DateTime(2016, 10, 14) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "JEHE", });
            ds.AddPerson(new Person { TeamId = 1, UserId = "RCHI", });
            ds.AddPerson(new Person { TeamId = 1, UserId = "VAAL", });
            ds.AddPerson(new Person { TeamId = 1, UserId = "KEPET" });
            ds.AddPerson(new Person { TeamId = 1, UserId = "TBER", Deleted = new DateTime(2017, 01, 30) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "IO",   });
            ds.AddPerson(new Person { TeamId = 1, UserId = "YLI",  });
            ds.AddPerson(new Person { TeamId = 1, UserId = "LEDY", });
            ds.AddPerson(new Person { TeamId = 1, UserId = "ALLLA" });
            ds.AddPerson(new Person { TeamId = 1, UserId = "MGL",  });
        }

    }
}

