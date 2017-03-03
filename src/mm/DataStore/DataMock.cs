//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using mm.Models;

//namespace mm.DataStore
//{
//    public class DataMock : IDataStore
//    {
//        List<Team> _teams = new List<Team> { };
//        List<Person> _persons = new List<Person> { };

//        List<Event> _events = new List<Event> { };
//        List<Participant> _participation = new List<Participant> { };

//        public DataMock(bool addData = true)
//        {
//            if (addData)
//                AddData();
//        }

//        public void AddData()
//        {
//            var idCAJRG = AddPerson(new Person {     Name = "CAJRG", Deleted = new DateTime(2016, 10, 14) });
//            var idJEHE = AddPerson(new Person {      Name = "JEHE", });
//            var idRCHI = AddPerson(new Person {      Name = "RCHI", });
//            var idKEPET = AddPerson(new Person {     Name = "KEPET", });
//            var idTBER = AddPerson(new Person {      Name = "TBER", Deleted = new DateTime(2017, 01, 30) });
//            var idIO = AddPerson(new Person {        Name = "IO", });
//            var idYLI = AddPerson(new Person {       Name = "YLI", });
//            var idLEDY = AddPerson(new Person {      Name = "LEDY", });
//            var idALLLA = AddPerson(new Person {     Name = "ALLLA", });
//            var idMGL = AddPerson(new Person {       Name = "MGL", });
//            var idVAAL = AddPerson(new Person {      Name = "VAAL", });

//            AddParticipant(new DateTime(2016, 10, 14), 2, idCAJRG, Participation.Buying);
//            AddParticipant(new DateTime(2016, 11, 25), 2, idJEHE, Participation.Buying);
//            AddParticipant(new DateTime(2016, 12, 09), 2, idRCHI, Participation.Buying);
//            AddParticipant(new DateTime(2016, 12, 16), 2, idVAAL, Participation.Buying);
//            AddParticipant(new DateTime(2016, 12, 23), 2, idKEPET, Participation.Buying);
//            AddParticipant(new DateTime(2017, 01, 06), 2, idTBER, Participation.Buying);
//            AddParticipant(new DateTime(2017, 01, 13), 2, idIO, Participation.Buying);
//            AddParticipant(new DateTime(2017, 01, 20), 2, idYLI, Participation.Buying);
//            AddParticipant(new DateTime(2017, 01, 27), 2, idLEDY, Participation.Buying);
//            AddParticipant(new DateTime(2017, 02, 03), 2, idALLLA, Participation.Buying);

//            AddParticipant(new DateTime(2017, 02, 10), 2, idMGL, Participation.Buying);
//            AddParticipant(new DateTime(2017, 02, 10), 2, idJEHE, Participation.NotParticipating);
//            AddParticipant(new DateTime(2017, 02, 10), 2, idRCHI, Participation.NotParticipating);

//            AddParticipant(new DateTime(2017, 02, 17), 2, idVAAL, Participation.Buying);
//            AddParticipant(new DateTime(2017, 02, 24), 2, idRCHI, Participation.Buying);

//            AddParticipant(new DateTime(2017, 03, 17), 2, idMGL, Participation.NotParticipating);

//        }

//        public int AddTeam(Team team)
//        {
//            team.Id = _teams.Count() + 1;
//            _teams.Add(team);
//            return team.Id;
//        }

//        public int AddPerson(Person person)
//        {
//            person.Id = _persons.Count() + 1;
//            person.Created = person.Created != DateTime.MinValue ? DateTime.Now : person.Created;
//            _persons.Add(person);
//            return person.Id;
//        }

//        public List<Team> GetTeams()
//        {
//            return _teams;
//        }

//        public List<Person> GetPersons(int teamId)
//        {
//            return (from p in _persons
//                    where p.TeamId == teamId
//                    select new Person(p)).ToList();
//        }

//        public List<Event> GetEvents(int teamId, DateTime fromDate)
//        {
//            return _events.FindAll(e => e.TeamId == teamId && e.When >= fromDate);
//        }

//        public List<Participant> GetParticipants(int teamId)
//        {
//            return (from p in _participation
//                    where p.TeamId == teamId
//                    select new Participant(p)).ToList();
//        }

//        public void AddParticipant(DateTime when, int teamId, int personId, Participation participation)
//        {
//            _participation.Add(new Participant
//            {
//                When = when,
//                TeamId = teamId,
//                PersonId = personId,
//                Participating = participation
//            });
//        }

//        public void RemoveParticipation(DateTime when, int personId)
//        {
//            var idx = _participation.FindIndex(x => x.When.Date == when.Date && x.PersonId == personId);
//            if (idx >= 0)
//            {
//                _participation.RemoveAt(idx);
//            }
//        }

//        public void RemoveSpecificParticipation(DateTime when, int teamId, Participation status)
//        {
//            var idx = _participation.FindIndex(x => x.When.Date == when.Date && x.TeamId == teamId && x.Participating == status);
//            if (idx >= 0)
//            {
//                _participation.RemoveAt(idx);
//            }
//        }

//        public void DeletePerson(int id)
//        {
//            var person = _persons.Find(p => p.Id == id);
//            if (person != null)
//                person.Deleted = DateTime.Now;
//        }
//    }
//}
