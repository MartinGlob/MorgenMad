using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mm.Models;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace mm.DataStore
{
    public class MongoStore
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _db;

        IMongoCollection<Team> _teams;
        IMongoCollection<Participant> _participants;

        public MongoStore()
        {
            _client = new MongoClient("mongodb://localhost");
            _db = _client.GetDatabase("MorgenMad");

            _teams = _db.GetCollection<Team>("Teams");
            _participants = _db.GetCollection<Participant>("Participants");
        }
        public async Task<Guid> CreateTeam(Team team)
        {
            team.Id = Guid.NewGuid();
            await _teams.InsertOneAsync(team);
            return team.Id;
        }

        public async Task<Team> GetTeam(Guid id)
        {
            return await _teams.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Team> GetTeam(string name)
        {
            return await _teams.Find(t => t.Name.Equals(name)).FirstOrDefaultAsync();
        }

        public async Task<Guid> AddPerson(Guid teamId, Person p)
        {
            p.Id = Guid.NewGuid();
            p.Created = DateTime.Now;

            var filter = Builders<Team>.Filter.Eq(e => e.Id, teamId);
            var update = Builders<Team>.Update.Push<Person>(e => e.Persons, p);

            await _teams.FindOneAndUpdateAsync(filter, update);

            return p.Id;
        }

        public async Task AddParticipant(Participant participant)
        {
            await _participants.ReplaceOneAsync(p => p.When == participant.When && p.PersonId == participant.PersonId, participant, new UpdateOptions { IsUpsert = true });
        }

    }
}
