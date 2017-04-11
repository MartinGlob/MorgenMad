using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mm.Models
{
    public class Calendar
    {
        [BsonId]
        public string Id { get; set; } // iso country code
        public Dictionary<string, string> Dates { get; set; } // dates are local
        public Calendar()
        {
            Dates = new Dictionary<string, string>();
        }
    }
}
