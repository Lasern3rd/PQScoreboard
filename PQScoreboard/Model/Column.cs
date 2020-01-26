using System.Collections.Generic;
using System.Linq;

namespace PQScoreboard
{
    public class Column
    {
        public string Id { get; set; }

        public string Header { get; set; }

        public List<decimal> Data { get; set; }

        public decimal Total
        {
            get
            {
                return Data.Sum();
            }
        }
    }
}
