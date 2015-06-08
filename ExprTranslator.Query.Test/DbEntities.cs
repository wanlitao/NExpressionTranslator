using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExprTranslator.Query.Test
{
    public class Customer
    {
        public long CustomerID { get; set; }
        public string ContactName { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime createTime { get; set; }
    }
}