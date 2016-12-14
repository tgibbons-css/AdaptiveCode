using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sermo.Data.Contracts
{
    public class RoomRecord
    {
        //public RoomRecord(string name)
        //{
        //    Id = 0;
        //    Name = name;
        //}
        // Don't need this anymore.
        // Testing issue with github...1

        public RoomRecord(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }
    }
}
