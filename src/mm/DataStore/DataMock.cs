using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mm.Models;

namespace mm.DataStore
{
    public class DataMock : IDataStore
    {
        List<Team> _teams = new List<Team> { };
        List<Person> _persons = new List<Person>
        {
            new Person { UserId = "CAJRG", LastGave = new DateTime(2016,10,14), Deleted=new DateTime(2016,10,14) },
            new Person { UserId = "JEHE", LastGave = new DateTime(2016,11,25) },
            new Person { UserId = "RCHI", LastGave = new DateTime(2016,12,09) },
            new Person { UserId = "VAAL", LastGave = new DateTime(2016,12,16) },
            new Person { UserId = "KEPET", LastGave = new DateTime(2016,12,23) },
            new Person { UserId = "TBER", LastGave = new DateTime(2017,01,06), Deleted=new DateTime(2017,01,30) },
            new Person { UserId = "IO", LastGave = new DateTime(2017,01,13) },
            new Person { UserId = "YLI", LastGave = new DateTime(2017,01,20) },
            new Person { UserId = "LEDY", LastGave = new DateTime(2017,01,27) },
            new Person { UserId = "ALLLA", LastGave = new DateTime(2017,02,03) },
            new Person { UserId = "MGL", LastGave = new DateTime(2017,02,10) },
        };
        List<Event> _events = new List<Event> { };
        List<Participant> _participation = new List<Participant> { };

        public int AddTeam(Team team)
        {
            team.Id = _teams.Count() + 1;
            _teams.Add(team);
            return team.Id;
        }

        public int AddPerson(Person person)
        {
            person.Id = _persons.Count() + 1;
            person.Created = DateTime.Now;
            _persons.Add(person);
            return person.Id;
        }

        public List<Team> GetTeams()
        {
            return _teams;
        }

        public List<Person> GetPersons(int teamId)
        {
            return _persons.FindAll(p => p.TeamId == teamId);
        }

        public List<Event> GetEvents(int teamId, DateTime fromDate)
        {
            return _events.FindAll(e => e.TeamId == teamId && e.When >= fromDate);
        }

        public List<Participant> GetParticipations(int teamId)
        {
            return _participation.FindAll(p => p.TeamId == teamId);
        }

        public void AddParticipation(DateTime when, int teamId, int personId, Participation participation)
        {
            _participation.Add(new Participant
            {
                When = when,
                TeamId = teamId,
                PersonId = personId,
                Participating = participation
            });
        }

        public void RemoveParticipation(DateTime when, int personId)
        {
            var idx = _participation.FindIndex(x => x.When == when && x.PersonId == personId);
            if (idx >= 0)
            {
                _participation.RemoveAt(idx);
            }
        }

    }
}
