using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using mm.Models;
using mm.DataStore;

namespace Tests
{
    public class MongoTest
    {
        [Fact]
        public async void AddTeam()
        {
            var m = new MongoStore();

            var tId = await m.CreateTeam(new Team { Name = "34AP", EventDay = DayOfWeek.Friday });

            var t = await m.GetTeam("34AP");

            Assert.Equal("34AP", t.Name);
            Assert.Equal<Guid>(tId, t.Id);

            var idMGL = await m.AddPerson(t.Id, new Person { Name = "MGL" });
            var idIO = await m.AddPerson(t.Id, new Person { Name = "IO" });
            await m.AddPerson(t.Id, new Person { Name = "KEPET" });
            await m.AddPerson(t.Id, new Person { Name = "LEDY" });

            t = await m.GetTeam(tId);

            Assert.Equal(4, t.Persons.Count);

            await m.AddParticipant(new Participant { TeamId = tId, PersonId = idMGL, When = new DateTime(2017, 06, 01), Participating = Participation.Buying });
            await m.AddParticipant(new Participant { TeamId = tId, PersonId = idIO, When = new DateTime(2017, 06, 01), Participating = Participation.NotParticipating });
        }
    }
}
