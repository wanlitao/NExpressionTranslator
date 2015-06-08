using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprTranslator.Query.Test
{
    [TestClass]
    public class SQLiteQueryTranslatorTest
    {
        [TestMethod]
        public void TestTranslateMethodCallWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName.StartsWith("dr");
            string whereSql = SQLiteQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("like('dr' || '%', companyName)", whereSql, true);

            customerPredicate = x => x.CompanyName.EndsWith("dr");
            whereSql = SQLiteQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("like('%' || 'dr', companyName)", whereSql, true);

            customerPredicate = x => x.CompanyName.Contains("dr");
            whereSql = SQLiteQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("like('%' || 'dr' || '%', companyName)", whereSql, true);

            customerPredicate = x => x.CompanyName.Length == 9;
            whereSql = SQLiteQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(length(companyName) = 9)", whereSql, true);

            customerPredicate = x => x.CompanyName.CompareTo("dr") == 1;
            whereSql = SQLiteQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("((case when companyName = 'dr' then 0 when companyName < 'dr' then -1 else 1 end) = 1)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateDateTimeWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.createTime.Year == 2015;
            string whereSql = SQLiteQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(strftime('%Y', createTime) = 2015)", whereSql, true);
        }
    }
}
