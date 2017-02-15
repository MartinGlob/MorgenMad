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
            ds.AddPerson(new Person { TeamId = 1, UserId = "CAJRG", LastGave = new DateTime(2016, 10, 14), Deleted = new DateTime(2016, 10, 14) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "JEHE", LastGave = new DateTime(2016, 11, 25) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "RCHI", LastGave = new DateTime(2016, 12, 09) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "VAAL", LastGave = new DateTime(2016, 12, 16) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "KEPET", LastGave = new DateTime(2016, 12, 23) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "TBER", LastGave = new DateTime(2017, 01, 06), Deleted = new DateTime(2017, 01, 30) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "IO", LastGave = new DateTime(2017, 01, 13) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "YLI", LastGave = new DateTime(2017, 01, 20) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "LEDY", LastGave = new DateTime(2017, 01, 27) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "ALLLA", LastGave = new DateTime(2017, 02, 03) });
            ds.AddPerson(new Person { TeamId = 1, UserId = "MGL", LastGave = new DateTime(2017, 02, 10) });
        }

    }
}

