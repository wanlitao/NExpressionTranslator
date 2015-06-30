using System;
using System.Data;
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
            Assert.AreEqual("(companyName = @p0)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateMultiParamWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == "drore" && x.City == "Hangzhou";
            string whereSql = QueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("((companyName = @p0) and (city = @p1))", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateQuerySql()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == "drore" && x.City == "Hangzhou";
            QuerySql whereSql = QueryTranslator.GetQuerySql(customerPredicate);
            Assert.AreEqual("((companyName = @p0) and (city = @p1))", whereSql.whereStr, true);
            Assert.AreEqual(2, whereSql.parameters.Length);
            Assert.AreEqual(SqlDbType.NVarChar, whereSql.parameters[0].QueryType.SqlDbType);
            Assert.AreEqual(DbType.String, whereSql.parameters[1].QueryType.DbType);
        }
    }
}
