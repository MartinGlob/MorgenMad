using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mm.Models;
using mm.Logic;
using mm.DataStore;
using Xunit;

namespace Tests
{
    public class EventViewTests
    {
        const int TeamId = 2;

        MongoStore _ds;

        public EventViewTests()
        {
            _ds = new MongoStore();
        }

        [Fact]
        public void Test1()
        {
           
        }
    }
}
