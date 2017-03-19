using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mm.Models
{
    public class Team
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public DayOfWeek EventDay { get; set; }
        //public List<Person> Persons { get; set; }

        public Team()
        {
            //Persons = new List<Person>();
        }
    }
}
