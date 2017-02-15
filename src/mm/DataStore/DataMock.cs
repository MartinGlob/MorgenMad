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
        List<Person> _persons = new List<Person> { };

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

        public void DeletePerson(int id)
        {
            var person = _persons.Find(p => p.Id == id);
            if (person != null)
                person.Deleted = DateTime.Now;
        }
    }
}
