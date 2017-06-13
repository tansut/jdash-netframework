using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{
    public class QueryResult<T>
    {
        public IEnumerable<T> data { get; set; }
        public bool hasMore { get; set; }
    }

}
