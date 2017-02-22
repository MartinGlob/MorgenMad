using mm.Logic;
using mm.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using mm.Models;

namespace Tests
{
    public class NextGiverTests
    {

        IDataStore _ds;

        public NextGiverTests()
        {
            _ds = new DataMock(false);
        }

        [Fact]
        public void NextGiverEmptyList()
        {
            var b = new BreakfastLogic(_ds, 1);
            var l = b.NextGiverList(null);
            Assert.Empty(l);
        }

        [Fact]
        public void SingleNewPersonNeverGave()
        {

            TestData.AddPersons(_ds, 1);
            var l = new BreakfastLogic(_ds, 1).NextGiverList(null);
            Assert.Equal(1, l.Count());
            Assert.Equal(1, l[0].Id);
        }

        [Fact]
        public void FivePersonsAllGave()
        {
            TestData.AddPersons(_ds, 5);

            var l = new BreakfastLogic(_ds, 1).NextGiverList(null);
            Assert.Equal(5, l.Count());

            Assert.Equal(1, l[0].Id);
            Assert.Equal(2, l[1].Id);
            Assert.Equal(3, l[2].Id);
            Assert.Equal(4, l[3].Id);
            Assert.Equal(5, l[4].Id);
        }

        [Fact]
        public void AddNewPersonToFivePersonsAllGave()
        {
            TestData.AddPersons(_ds, 5);

            var idAdded = _ds.AddPerson(new Person { Id = 50, TeamId = 1 });

            var l = new BreakfastLogic(_ds, 1).NextGiverList(null);
            Assert.Equal(6, l.Count());

            Assert.Equal(1, l[0].Id);
            Assert.Equal(idAdded, l[1].Id);
            Assert.Equal(2, l[2].Id);
            Assert.Equal(3, l[3].Id);
            Assert.Equal(4, l[4].Id);
            Assert.Equal(5, l[5].Id);
        }

        [Fact]
        public void FivePersonsOneDeletedAllGave()
        {
            TestData.AddPersons(_ds, 5);
            _ds.DeletePerson(3);

            var l = new BreakfastLogic(_ds, 1).NextGiverList(null);
            Assert.Equal(5 - 1, l.Count());
            Assert.Equal(1, l[0].Id);
            Assert.Equal(2, l[1].Id);
            Assert.Equal(4, l[2].Id);
            Assert.Equal(5, l[3].Id);
        }

        [Fact]
        public void TwoNewPersonNeverGave()
        {
            TestData.AddPersons(_ds, 2, noLastGaveDate: true);

            var l = new BreakfastLogic(_ds, 1).NextGiverList(null);
            Assert.Equal(2, l.Count());

            Assert.NotNull(l[0].LastGave);
            Assert.NotNull(l[1].LastGave);

            Assert.True(l[0].LastGave < l[1].LastGave);
        }

    }
}

