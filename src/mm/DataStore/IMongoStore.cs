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
        Task<Person> GetPerson(string name);
        Task<Person> GetPerson(ObjectId id);
        Task<List<Person>> GetPersons(ObjectId teamId);
        Task<Team> GetTeam(string name);
        Task<Team> GetTeam(ObjectId id);
        Task RemoveAndInsert(Participant participant);
        Task<ObjectId> UpdatePerson(Person p);
        Task<ObjectId> UpdateTeam(Team t);
    }
}