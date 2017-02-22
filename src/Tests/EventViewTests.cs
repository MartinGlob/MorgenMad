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

        IDataStore _ds;

        public EventViewTests()
        {
            _ds = new DataMock();
        }

        [Fact]
        public void Test1()
        {
            var b = new BreakfastLogic(_ds, TeamId);

            var view = b.CreateEventList(TeamId,DateTime.MinValue);

        }
    }
}
