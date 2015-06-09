using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprTranslator.Query.Test
{
    [TestClass]
    public class OracleQueryTranslatorTest
    {
        [TestMethod]
        public void TestTranslateMethodCallWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName.StartsWith("dr");
            string whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like ('dr' || '%'))", whereSql, true);

            customerPredicate = x => x.CompanyName.EndsWith("dr");
            whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like ('%' || 'dr'))", whereSql, true);

            customerPredicate = x => x.CompanyName.Contains("dr");
            whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like ('%' || 'dr' || '%'))", whereSql, true);

            customerPredicate = x => x.CompanyName.Length == 9;
            whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(LENGTH(companyName) = 9)", whereSql, true);

            customerPredicate = x => x.CompanyName.CompareTo("dr") == 1;
            whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("((case when companyName = 'dr' then 0 when companyName < 'dr' then -1 else 1 end) = 1)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateDateTimeWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.createTime.Year == 2015;
            string whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(extract(year from createTime) = 2015)", whereSql, true);
        }
    }
}
