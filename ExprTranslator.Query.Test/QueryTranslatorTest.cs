using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprTranslator.Query.Test
{
    [TestClass]
    public class QueryTranslatorTest
    {
        [TestMethod]
        public void TestTranslateEqualWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CustomerID == 1;            
            string whereSql = QueryTranslator.GetQueryText(customerPredicate);            
            Assert.AreEqual("(customerID = 1)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateNullWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == null;
            string whereSql = QueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName is null)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateWhereStrWithLocalVariable()
        {
            string companyName = "drore";
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == companyName;            
            string whereSql = QueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName = 'drore')", whereSql, true);
        }
    }
}
