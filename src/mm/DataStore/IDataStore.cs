using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mm.Models;

namespace mm.DataStore
{
    public interface IDataStore
    {
        int AddTeam(Team team);
        List<Team> GetTeams();

        int AddPerson(Person person);
        List<Person> GetPersons(int teamId);
        List<Event> GetEvents(int teamId, DateTime fromDate);

        List<Participant> GetParticipations(int teamId);
        void AddParticipation(DateTime when, int teamId, int personId, Participation participation);
        void RemoveParticipation(DateTime when, int personId);
    }
}
