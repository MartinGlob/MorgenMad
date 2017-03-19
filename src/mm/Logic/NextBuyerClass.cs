using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mm.Models;

namespace mm.Logic
{
    public class NextBuyerClass
    {
        List<Person> _list;

        public NextBuyerClass(List<Person> list)
        {
            _list = list;
        }

        public int Count()
        {
            return _list.Count();
        }

        public Person Peek(int idx)
        {
            return _list.Count > 0 ? _list[idx] : null;
        }

        public bool MoveToEnd(Person p)
        {
            if (_list.Count >= 2)
            {
                var i = _list.FindIndex(x => x.Id == p.Id);

                _list.RemoveAt(i);
                _list.Add(p);
                return true;
            }
            else
                return false;
        }
    }
}
