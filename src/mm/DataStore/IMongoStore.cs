using System.Collections.Generic;
using System.Threading.Tasks;
using mm.Models;
using MongoDB.Bson;

namespace mm.DataStore
{
    public interface IMongoStore
    {
        void ClearAll();
        Task<List<Participant>> GetParticipants(ObjectId teamId);
        Task<Person> GetPerson(string id);
        Task<List<Person>> GetPersons(ObjectId teamId);
        Task<List<Team>> GetTeams();
        Task<Team> GetTeam(string name);
        Task<Team> GetTeam(ObjectId id);
        void RemoveAndInsert(Participant participant);
        Task<string> UpdatePerson(Person p);
        Task AddPerson(Person p);
        Task<ObjectId> UpdateTeam(Team t);
        Task Log(string msg);
    }
}