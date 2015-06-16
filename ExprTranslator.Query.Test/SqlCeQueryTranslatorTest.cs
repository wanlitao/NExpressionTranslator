﻿using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprTranslator.Query.Test
{
    [TestClass]
    public class SqlCeQueryTranslatorTest
    {
        [TestMethod]
        public void TestTranslateMethodCallWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName.StartsWith("dr");
            string whereSql = SqlCeQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like @p0 + '%')", whereSql, true);

            customerPredicate = x => x.CompanyName.EndsWith("dr");
            whereSql = SqlCeQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like '%' + @p0)", whereSql, true);

            customerPredicate = x => x.CompanyName.Contains("dr");
            whereSql = SqlCeQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(companyName like '%' + @p0 + '%')", whereSql, true);

            customerPredicate = x => x.CompanyName.Length == 9;
            whereSql = SqlCeQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(len(companyName) = 9)", whereSql, true);

            customerPredicate = x => x.CompanyName.CompareTo("dr") == 1;
            whereSql = SqlCeQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("((case when companyName = @p0 then 0 when companyName < @p0 then -1 else 1 end) = 1)", whereSql, true);
        }

        [TestMethod]
        public void TestTranslateDateTimeWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.createTime.Year == 2015;
            string whereSql = SqlCeQueryTranslator.GetQueryText(customerPredicate);
            Assert.AreEqual("(DatePart(year, createTime) = 2015)", whereSql, true);
        }
    }
}
