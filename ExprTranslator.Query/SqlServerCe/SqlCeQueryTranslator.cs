using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExprTranslator.Query
{
    /// <summary>
    /// Formats a query expression into SQL Server Compact Edition language syntax
    /// </summary>
    public class SqlCeQueryTranslator : TSqlQueryTranslator
    {
        #region 静态方法
        public static new string GetQueryText(Expression expression,
            Func<MemberInfo, string> memberColumnNameGetter = null)
        {
            var queryTranslator = new SqlCeQueryTranslator();
            queryTranslator.MemberColumnNameConverter = memberColumnNameGetter;
            return queryTranslator.Translate(expression);
        }

        public static new QuerySql GetQuerySql(Expression expression,
            Func<MemberInfo, string> memberColumnNameGetter = null)
        {
            var queryTranslator = new SqlCeQueryTranslator();
            queryTranslator.MemberColumnNameConverter = memberColumnNameGetter;
            return queryTranslator.TranslateSql(expression);
        }
        #endregion

        #region 表达式翻译
        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(string))
            {
                switch (m.Member.Name)
                {
                    case "Length":
                        this.Write("LEN(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                }
            }
            else if (m.Member.DeclaringType == typeof(DateTime) || m.Member.DeclaringType == typeof(DateTimeOffset))
            {
                switch (m.Member.Name)
                {
                    case "Day":
                        this.Write("DATEPART(day, ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Month":
                        this.Write("DATEPART(month, ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Year":
                        this.Write("DATEPART(year, ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Hour":
                        this.Write("DATEPART(hour, ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Minute":
                        this.Write("DATEPART(minute, ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Second":
                        this.Write("DATEPART(second, ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Millisecond":
                        this.Write("DATEPART(millisecond, ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "DayOfWeek":
                        this.Write("(DATEPART(weekday, ");
                        this.Visit(m.Expression);
                        this.Write(") - 1)");
                        return m;
                    case "DayOfYear":
                        this.Write("DATEPART(dayofyear, ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                }
            }
            return base.VisitMember(m);
        }
        #endregion        
    }
}
