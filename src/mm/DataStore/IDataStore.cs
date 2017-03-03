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
        List<Person> GetPersons(string teamId);
        void DeletePerson(Guid id);
        List<Event> GetEvents(string teamId, DateTime fromDate);

        List<Participant> GetParticipants(Guid teamId);
        void AddParticipant(DateTime when, Guid teamId, Guid personId, Participation participation);
        void RemoveParticipation(DateTime when, Guid personId);
        void RemoveSpecificParticipation(DateTime when, Guid teamId, Participation status);

    }
}
