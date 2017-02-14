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
        Breakfast _b;
        IDataStore _db;
        int _teamId;

        public NextGiverTests()
        {
            _db = new DataMock();
            _b = new Breakfast(_db);
        }

        [Fact]
        public void NextGiverEmptyList()
        {
            var persons = TestData.CreatePersons(0);
            var l = _b.WhoIsNextList(persons);
            Assert.Empty(l);
        }

        [Fact]
        public void SingleNewPersonNeverGave()
        {
            var persons = TestData.CreatePersons(1);
            var l = _b.WhoIsNextList(persons);
            Assert.Equal(1, l.Count());
            Assert.Equal(1, l[0].Id);
        }

        [Fact]
        public void FivePersonsAllGave()
        {
            var persons = TestData.CreatePersons(5);

            var l = _b.WhoIsNextList(persons);
            Assert.Equal(persons.Count, l.Count());

            Assert.Equal(1, l[0].Id);
            Assert.Equal(2, l[1].Id);
            Assert.Equal(3, l[2].Id);
            Assert.Equal(4, l[3].Id);
            Assert.Equal(5, l[4].Id);
        }

        [Fact]
        public void AddNewPersonToFivePersonsAllGave()
        {
            var persons = TestData.CreatePersons(5);

            persons.Add(new Person { Id = 50 });

            var l = _b.WhoIsNextList(persons);
            Assert.Equal(persons.Count, l.Count());
            Assert.Equal(1, l[0].Id);
            Assert.Equal(50, l[1].Id);
            Assert.Equal(2, l[2].Id);
            Assert.Equal(3, l[3].Id);
            Assert.Equal(4, l[4].Id);
            Assert.Equal(5, l[5].Id);
        }

        [Fact]
        public void FivePersonsOneDeletedAllGave()
        {
            var persons = TestData.CreatePersons(5);
            persons[2].Deleted = DateTime.Now;

            var l = _b.WhoIsNextList(persons);
            Assert.Equal(persons.Count - 1, l.Count());
            Assert.Equal(1, l[0].Id);
            Assert.Equal(2, l[1].Id);
            Assert.Equal(4, l[2].Id);
            Assert.Equal(5, l[3].Id);
        }

        [Fact]
        public void TwoNewPersonNeverGave()
        {
            var persons = TestData.CreatePersons(2, noLastGaveDate: true);

            var l = _b.WhoIsNextList(persons);
            Assert.Equal(persons.Count, l.Count());
            Assert.Equal(1, l[0].Id);
            Assert.Equal(2, l[1].Id);
        }

    }
}

