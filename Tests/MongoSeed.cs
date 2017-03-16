using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mm.Models;
using mm.DataStore;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class MongoSeed
    {
        [TestMethod]
        public async Task InitializeMongo()
        {
            var m = new MongoStore();
            m.ClearAll();

            await m.UpdateTeam(new Team { Name = "C&I GLS", EventDay = DayOfWeek.Friday });
            await m.UpdateTeam(new Team { Name = "C&I KWANT", EventDay = DayOfWeek.Friday });

            var teamId = await m.UpdateTeam(new Team { Name = "34AP", EventDay = DayOfWeek.Friday });

            var idCAJRG = await m.UpdatePerson(new Person("BB001", "cajrg@danskebank.dk", teamId.ToString()) { Deleted = new DateTime(2016, 10, 14) });
            var idJEHE = await m.UpdatePerson(new Person("BB002", "jehe@danskebank.dk", teamId.ToString()));
            var idRCHI = await m.UpdatePerson(new Person("BB003", "rchi@danskebank.dk", teamId.ToString()));
            var idKEPET = await m.UpdatePerson(new Person("BB004", "kepet@danskebank.dk", teamId.ToString()));
            var idTBER = await m.UpdatePerson(new Person("BB005", "tber@danskebank.dk", teamId.ToString()) { Deleted = new DateTime(2017, 01, 30) });
            var idIO = await m.UpdatePerson(new Person("BB006", "io@danskebank.dk", teamId.ToString()));
            var idYLI = await m.UpdatePerson(new Person("BB007", "yli@danskebank.dk", teamId.ToString()));
            var idLEDY = await m.UpdatePerson(new Person("BB008", "ledy@danskebank.dk", teamId.ToString()));
            var idALLLA = await m.UpdatePerson(new Person("BB009", "allla@danskebank.dk", teamId.ToString()));
            var idMGL = await m.UpdatePerson(new Person("BB010", "mgl@danskebank.dk", teamId.ToString()));
            var idVAAL = await m.UpdatePerson(new Person("BB011", "vaal@danskebank.dk", teamId.ToString()));

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
            await m.SetParticipant(new Participant(new DateTime(2017, 03, 10), teamId, idMGL, Participation.NotParticipating));

            await m.SetParticipant(new Participant(new DateTime(2017, 03, 17), teamId, idIO, Participation.NotParticipating));
            await m.SetParticipant(new Participant(new DateTime(2017, 03, 17), teamId, idYLI, Participation.NotParticipating));


        }
    }
}
