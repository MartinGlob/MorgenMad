using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using mm.Models;
using mm.DataStore;
using MongoDB.Bson;

namespace Tests
{
    public class MongoTest
    {
        [Fact]
        public async void BasicCRUDTests()
        {
            var m = new MongoStore();
            m.ClearAll();

            var p1 = new Person { Name = "MGL" };
            var id = await m.UpdatePerson(p1);

            Assert.NotEqual<ObjectId>(ObjectId.Empty, id);

            var p2 = await m.GetPerson(id);
            var p3 = await m.GetPerson("MGL");

            Assert.Equal(p1.Name, p2.Name);


            //var dt1 = t.Persons[0].Created.ToLocalTime();
            //var dt2 = t.Persons[0].Created.ToString();

            //await m.AddParticipant(new Participant { TeamId = tId, PersonId = idMGL, When = new DateTime(2017, 06, 01), Participating = Participation.Buying });
            //await m.AddParticipant(new Participant { TeamId = tId, PersonId = idIO, When = new DateTime(2017, 06, 01), Participating = Participation.NotParticipating });
            //await m.AddParticipant(new Participant { TeamId = tId, PersonId = idMGL, When = new DateTime(2017, 06, 01), Participating = Participation.NotParticipating });
        }

        [Fact]
        public async void SeedDataBase()
        {
            var m = new MongoStore();
            m.ClearAll();

            var teamId = await m.UpdateTeam(new Team { Name = "34AP", EventDay = DayOfWeek.Friday });

            var idCAJRG = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "CAJRG", Deleted = new DateTime(2016, 10, 14) });
            var idJEHE = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "JEHE" });
            var idRCHI = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "RCHI" });
            var idKEPET = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "KEPET" });
            var idTBER = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "TBER", Deleted = new DateTime(2017, 01, 30) });
            var idIO = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "IO" });
            var idYLI = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "YLI" });
            var idLEDY = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "LEDY" });
            var idALLLA = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "ALLLA" });
            var idMGL = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "MGL" });
            var idVAAL = await m.UpdatePerson(new Person { Created = new DateTime(2016, 08, 01), TeamId = teamId, Name = "VAAL" });

            await m.SetParticipant(new Participant(new DateTime(2016, 10, 14), teamId, idCAJRG, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2016, 11, 25), teamId, idJEHE, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2016, 12, 09), teamId, idRCHI, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2016, 12, 16), teamId, idVAAL, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2016, 12, 23), teamId, idKEPET, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 01, 06), teamId, idTBER, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 01, 13), teamId, idIO, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 01, 20), teamId, idYLI, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 01, 27), teamId, idLEDY, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 02, 03), teamId, idALLLA, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 02, 10), teamId, idMGL, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 02, 10), teamId, idJEHE, Participation.NotParticipating));
            await m.SetParticipant(new Participant(new DateTime(2017, 02, 10), teamId, idRCHI, Participation.NotParticipating));
            await m.SetParticipant(new Participant(new DateTime(2017, 02, 17), teamId, idVAAL, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 02, 24), teamId, idRCHI, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 03, 03), teamId, idJEHE, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 03, 10), teamId, idKEPET, Participation.Buying));
            await m.SetParticipant(new Participant(new DateTime(2017, 03, 17), teamId, idMGL, Participation.NotParticipating));





        }
    }
}
