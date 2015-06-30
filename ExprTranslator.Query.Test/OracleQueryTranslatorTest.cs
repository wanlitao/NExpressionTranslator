using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

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
            Assert.AreEqual("(companyName like (:p0 || '%'))", whereSql, true);

            customerPredicate = x => x.CompanyName.EndsWith("dr");
            whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like ('%' || :p0))", whereSql, true);

            customerPredicate = x => x.CompanyName.Contains("dr");
            whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like ('%' || :p0 || '%'))", whereSql, true);

            customerPredicate = x => x.CompanyName.Length == 9;
            whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(LENGTH(companyName) = 9)", whereSql, true);

            customerPredicate = x => x.CompanyName.CompareTo("dr") == 1;
            whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("((case when companyName = :p0 then 0 when companyName < :p0 then -1 else 1 end) = 1)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateDateTimeWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.createTime.Year == 2015;
            string whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(extract(year from createTime) = 2015)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateMultiParamWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == "drore" && x.City == "Hangzhou";
            string whereSql = OracleQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("((companyName = :p0) and (city = :p1))", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateQuerySql()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == "drore" && x.City == "Hangzhou";
            QuerySql whereSql = OracleQueryTranslator.GetQuerySql(customerPredicate);
            Assert.AreEqual("((companyName = :p0) and (city = :p1))", whereSql.whereStr, true);
            Assert.AreEqual(2, whereSql.parameters.Length);
            Assert.AreEqual(SqlDbType.NVarChar, whereSql.parameters[0].QueryType.SqlDbType);
            Assert.AreEqual(2000, whereSql.parameters[1].QueryType.Length);
        }
    }
}
