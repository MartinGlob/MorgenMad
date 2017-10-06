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
        IMongoCollection<LogItem> _log;
        IMongoCollection<Calendar> _calendars;

        public MongoStore(string connectionString)
        {
            _client = new MongoClient(connectionString);
            _db = _client.GetDatabase("MorgenMad");

            _teams = _db.GetCollection<Team>("Teams");
            _teams.Indexes.CreateOneAsync(Builders<Team>.IndexKeys.Ascending(_ => _.Name), new CreateIndexOptions { Unique = true });

            _participants = _db.GetCollection<Participant>("Participants");

            _persons = _db.GetCollection<Person>("Persons");
            _persons.Indexes.CreateOneAsync(Builders<Person>.IndexKeys.Ascending(_ => _.Name), new CreateIndexOptions { Unique = true });

            _log = _db.GetCollection<LogItem>("Log");

            _calendars = _db.GetCollection<Calendar>("Calendars");
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

        public async Task<List<Team>> GetTeams()
        {
            return await _teams.Find(_ => true).ToListAsync();
        }

        public async Task<Team> GetTeam(ObjectId id)
        {
            return await _teams.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Team> GetTeam(string name)
        {
            return await _teams.Find(x => x.Name.ToLower() == name.ToLower()).SingleOrDefaultAsync();
        }

        public async Task<string> UpdatePerson(Person p)
        {
            await _persons.ReplaceOneAsync(x => x.Id == p.Id, p, new UpdateOptions { IsUpsert = true });
            return p.Id;
        }

        public async Task AddPerson(Person p)
        {
            await _persons.InsertOneAsync(p);
        }

        public async Task<Person> GetPerson(string id)
        {
            id = id.ToLower();
            return await _persons.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        //public async Task<Person> GetPerson(string name)
        //{
        //    return await _persons.Find(x => x.Name.ToLower() == name.ToLower()).SingleOrDefaultAsync();
        //}

        public async Task<List<Person>> GetPersons(ObjectId teamId)
        {
            return await _persons.Find(p => p.TeamId == teamId).ToListAsync();
        }

        private bool IsSameDate(DateTime a, DateTime b)
        {
            return a.ToUniversalTime().Date == b.ToUniversalTime().Date;
        }

        //todo fix up deletemany - should not be nessecary
        public void RemoveAndInsert(Participant participant)
        {

            var r = _participants.DeleteMany(p => p.PersonId == participant.PersonId && p.When == participant.When);


            // add a new entry depending on what the participant WAS doing
            switch (participant.Participating)
            {
                case Participation.WasBuying:
                    participant.Participating = Participation.Buying;
                    break;
                case Participation.Buying:
                    participant.Participating = Participation.NotParticipating;
                    break;
                case Participation.Participating:
                    participant.Participating = Participation.NotParticipating;
                    break;
                case Participation.Override:
                    var x = _participants.DeleteMany(p => p.Participating == Participation.Override && p.When == participant.When);
                    break;
                case Participation.NotParticipating:
                    return;
            }
            participant.Id = ObjectId.GenerateNewId();
            _participants.InsertOne(participant);
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

        public async Task Log(string msg)
        {
            await _log.InsertOneAsync(new LogItem { Msg = msg });
        }

        public async Task<List<Calendar>> GetCalendars()
        {
            return await _calendars.Find(_ => true).ToListAsync();
        }

        public async Task<Calendar> GetCalendar(string id)
        {
            return await _calendars.Find(t => t.Id == id).FirstOrDefaultAsync();

        }

        public async Task UpdateCalendar(Calendar calendar)
        {
            try
            {
                var r = await _calendars.ReplaceOneAsync(c => c.Id == calendar.Id, calendar, new UpdateOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                int i = 0;
            }
        }
    }
}
