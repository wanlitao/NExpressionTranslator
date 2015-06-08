using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprTranslator.Query.Test
{
    [TestClass]
    public class MySqlQueryTranslatorTest
    {
        [TestMethod]
        public void TestTranslateMethodCallWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName.StartsWith("dr");
            string whereSql = MySqlQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like CONCAT('dr','%'))", whereSql, true);

            customerPredicate = x => x.CompanyName.EndsWith("dr");
            whereSql = MySqlQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like CONCAT('%','dr'))", whereSql, true);

            customerPredicate = x => x.CompanyName.Contains("dr");
            whereSql = MySqlQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like CONCAT('%','dr','%'))", whereSql, true);

            customerPredicate = x => x.CompanyName.Length == 9;
            whereSql = MySqlQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(CHAR_LENGTH(companyName) = 9)", whereSql, true);

            customerPredicate = x => x.CompanyName.CompareTo("dr") == 1;
            whereSql = MySqlQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("((case when companyName = 'dr' then 0 when companyName < 'dr' then -1 else 1 end) = 1)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateDateTimeWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.createTime.Year == 2015;
            string whereSql = MySqlQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(year(createTime) = 2015)", whereSql, true);
        }
    }
}
