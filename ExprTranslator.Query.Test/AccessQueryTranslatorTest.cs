using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprTranslator.Query.Test
{
    [TestClass]
    public class AccessQueryTranslatorTest
    {
        [TestMethod]
        public void TestTranslateMethodCallWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName.StartsWith("dr");
            string whereSql = AccessQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like @p0 + '%')", whereSql, true);

            customerPredicate = x => x.CompanyName.EndsWith("dr");
            whereSql = AccessQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like '%' + @p0)", whereSql, true);

            customerPredicate = x => x.CompanyName.Contains("dr");
            whereSql = AccessQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like '%' + @p0 + '%')", whereSql, true);

            customerPredicate = x => x.CompanyName.Length == 9;
            whereSql = AccessQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(len(companyName) = 9)", whereSql, true);

            customerPredicate = x => x.CompanyName.CompareTo("dr") == 1;
            whereSql = AccessQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(IIF(companyName = @p0, 0, IIF(companyName < @p0, -1, 1)) = 1)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateDateTimeWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.createTime.Year == 2015;
            string whereSql = AccessQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(year(createTime) = 2015)", whereSql, true);
        }
    }
}
