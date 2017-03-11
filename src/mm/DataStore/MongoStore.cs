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
    public class MongoStore : IMongoStore
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _db;

        IMongoCollection<Team> _teams;

   
        IMongoCollection<Participant> _participants;
        IMongoCollection<Person> _persons;

        public MongoStore()
        {
            _client = new MongoClient("mongodb://localhost");
            _db = _client.GetDatabase("MorgenMad");

            _teams = _db.GetCollection<Team>("Teams");
            _teams.Indexes.CreateOneAsync(Builders<Team>.IndexKeys.Ascending(_ => _.Name), new CreateIndexOptions { Unique = true });
            
            _participants = _db.GetCollection<Participant>("Participants");

            _persons = _db.GetCollection<Person>("Persons");
            _persons.Indexes.CreateOneAsync(Builders<Person>.IndexKeys.Ascending(_ => _.Name), new CreateIndexOptions { Unique = true });

        }

        public async void ClearAll()
        {
            await _db.DropCollectionAsync("Teams");
            await _db.DropCollectionAsync("Persons");
            await _db.DropCollectionAsync("Participants");
        }

        public async Task<ObjectId> UpdateTeam(Team t)
        {
            if (t.Id == ObjectId.Empty)
            {
                t.Created = DateTime.Now;
                await _teams.InsertOneAsync(t);
            }
            else
            {
                await _teams.ReplaceOneAsync(x => x.Id == t.Id, t);
            }
            return t.Id;
        }

        public async Task<Team> GetTeam(ObjectId id)
        {
            return await _teams.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Team> GetTeam(string name)
        {
            return await _teams.Find(x => x.Name.ToLower() == name.ToLower()).SingleOrDefaultAsync();
        }

        public async Task<ObjectId> UpdatePerson(Person p)
        {
            if (p.Id == ObjectId.Empty)
            {
                p.Created = p.Created == null ? DateTime.Now : p.Created;
                await _persons.InsertOneAsync(p);
            }
            else
            {
                await _persons.ReplaceOneAsync(x => x.Id == p.Id, p);
            }
            return p.Id;
        }

        public async Task<Person> GetPerson(ObjectId id)
        {
            return await _persons.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Person> GetPerson(string name)
        {
            return await _persons.Find(x => x.Name.ToLower() == name.ToLower()).SingleOrDefaultAsync();
        }

        public async Task<List<Person>> GetPersons(ObjectId teamId)
        {
            return await _persons.Find(p => p.TeamId == teamId).ToListAsync();
        }

        public async Task RemoveAndInsert(Participant participant)
        {
            // first, remove the participant from the list if present
            if (participant.Participating != Participation.Participating)
            {
                await _participants.DeleteOneAsync(p => p.When == participant.When && p.PersonId == participant.PersonId);
            }

            // add a new entry depending on what the participant WAS doing
            switch (participant.Participating)
            {
                case Participation.Buying:
                    participant.Participating = Participation.NotParticipating;
                    break;
                case Participation.Participating:
                    participant.Participating = Participation.NotParticipating;
                    break;
                case Participation.Override:
                case Participation.NotParticipating:
                    return;
            }
            participant.Id = ObjectId.GenerateNewId();
            await _participants.InsertOneAsync(participant);
            return;
        }

        public async Task SetParticipant(Participant p)
        {
            await _participants.InsertOneAsync(p);
        }

        public async Task<List<Participant>> GetParticipants(ObjectId teamId)
        {
            return await _participants.Find(p => p.TeamId == teamId).ToListAsync();
        }

    }
}
