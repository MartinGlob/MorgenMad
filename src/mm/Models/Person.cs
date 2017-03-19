using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mm.Models
{
    public class Person
    {
        [BsonId]
        public string Id { get; set; } // B number
        public ObjectId TeamId { get; set; }
        public string Name { get; set; } // Short name - first part of email
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
        public string EMail { get; set; }

        public Person()
        {
            Created = DateTime.UtcNow;
        }

        public Person(string id, string email, string teamId)
        {
            Created = DateTime.Now;
            Id = id;
            TeamId = ObjectId.Parse(teamId);
            Name = email.Split('@')[0].ToUpper();
            EMail = email.ToLower();
        }

        public bool WasActive(DateTime when)
        {
            if (Created.Date > when.Date)
                return false;
            if (Deleted != null && Deleted.Value.Date <= when.Date)
                return false;
            return true;
        }
    }
}
