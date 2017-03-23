using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mm.Models
{
    public enum Participation { Buying = 1, Participating = 2, NotParticipating = 3, Override = 4 }

    public class Participant
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId TeamId { get; set; }
        public DateTime When { get; set; }
        public string PersonId { get; set; }
        public Participation Participating { get; set; }

        public Participant() {}

        public Participant(string when, string id, string status)
        {
            When = DateTime.ParseExact(when, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AssumeLocal).ToUniversalTime();
            PersonId = id;
            Participating = (Participation)Enum.Parse(typeof(Participation), status);
        }

        public Participant(DateTime when, ObjectId teamId, string personId, Participation participating)
        {
            When = when;
            TeamId = teamId;
            PersonId = personId;
            Participating = participating;
        }
    }
}
