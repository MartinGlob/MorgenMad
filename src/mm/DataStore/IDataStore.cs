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
        void DeletePerson(int id);
        List<Event> GetEvents(int teamId, DateTime fromDate);

        List<Participant> GetParticipants(int teamId);
        void AddParticipant(DateTime when, int teamId, int personId, Participation participation);
        void RemoveParticipation(DateTime when, int personId);
        void RemoveSpecificParticipation(DateTime when, int teamId, Participation status);

    }
}
