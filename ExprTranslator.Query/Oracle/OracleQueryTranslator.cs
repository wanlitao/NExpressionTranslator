using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExprTranslator.Query
{
    /// <summary>
    /// Formats a query expression into Oracle language syntax
    /// </summary>
    public class OracleQueryTranslator : QueryTranslator
    {
        public static new string GetQueryText(Expression expression)
        {
            var queryTranslator = new OracleQueryTranslator();
            return queryTranslator.Translate(expression);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(string))
            {
                switch (m.Member.Name)
                {
                    case "Length":
                        this.Write("LENGTH(");
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
                        this.Write("extract(day from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Month":
                        this.Write("extract(month from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Year":
                        this.Write("extract(year from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Hour":
                        this.Write("extract(hour from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Minute":
                        this.Write("extract(minute from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Second":
                        this.Write("extract(second from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Millisecond":
                        this.Write("(extract(second from ");
                        this.Visit(m.Expression);
                        this.Write(") * 1000)");
                        return m;
                    case "DayOfWeek":
                        this.Write("(to_char(");
                        this.Visit(m.Expression);
                        this.Write(", 'D') - 1)");
                        return m;
                    case "DayOfYear":
                        this.Write("to_char(");
                        this.Visit(m.Expression);
                        this.Write(", 'DDD')");
                        return m;
                }
            }
            return base.VisitMember(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(string))
            {
                switch (m.Method.Name)
                {
                    case "StartsWith":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE (");
                        this.Visit(m.Arguments[0]);
                        this.Write(" || '%'))");
                        return m;
                    case "EndsWith":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE ('%' || ");
                        this.Visit(m.Arguments[0]);
                        this.Write("))");
                        return m;
                    case "Contains":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE ('%' || ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" || '%'))");
                        return m;
                    case "Concat":
                        IList<Expression> args = m.Arguments;
                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                        {
                            args = ((NewArrayExpression)args[0]).Expressions;
                        }
                        for (int i = 0, n = args.Count; i < n; i++)
                        {
                            if (i > 0) this.Write(" || ");
                            this.Visit(args[i]);
                        }                        
                        return m;
                    case "IsNullOrEmpty":
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(" IS NULL OR ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" = '')");
                        return m;
                    case "ToUpper":
                        this.Write("UPPER(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "ToLower":
                        this.Write("LOWER(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "Replace":
                        this.Write("REPLACE(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Substring":
                        this.Write("SUBSTR(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + 1");
                        if (m.Arguments.Count == 2)
                        {
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                        }
                        this.Write(")");
                        return m;
                    case "Remove":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("SUBSTR(");
                            this.Visit(m.Object);
                            this.Write(", 1, ");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                        }
                        else
                        {
                            this.Write("SUBSTR(");
                            this.Visit(m.Object);
                            this.Write(", 1, ");
                            this.Visit(m.Arguments[0]);
                            this.Write(") + SUBSTR(");
                            this.Visit(m.Object);
                            this.Write(", ");
                            this.Visit(m.Arguments[0]);
                            this.Write(" + ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                        }
                        return m;
                    case "IndexOf":
                        this.Write("(INSTR(");
                        this.Visit(m.Object);                        
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(" + 1");
                        }
                        this.Write(") - 1)");
                        return m;
                    case "Trim":
                        this.Write("RTRIM(LTRIM(");
                        this.Visit(m.Object);
                        this.Write("))");
                        return m;
                }
            }
            else if (m.Method.DeclaringType == typeof(DateTime))
            {
                switch (m.Method.Name)
                {
                    case "op_Subtract":
                        if (m.Arguments[1].Type == typeof(DateTime))
                        {
                            this.Write("ROUND(TO_NUMBER(");
                            this.Visit(m.Arguments[1]);
                            this.Write(" - ");
                            this.Visit(m.Arguments[0]);
                            this.Write("))");
                            return m;
                        }
                        break;
                    case "AddYears":
                        this.Write("add_months(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" * 12)");
                        return m;
                    case "AddMonths":
                        this.Write("add_months(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "AddDays":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "AddHours":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" / 24)");
                        return m;
                    case "AddMinutes":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" / (24 * 60))");
                        return m;
                    case "AddSeconds":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" / (24 * 60 * 60))");
                        return m;
                    case "AddMilliseconds":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" / (24 * 60 * 60 * 1000))");
                        return m;
                }
            }
            else if (m.Method.DeclaringType == typeof(Decimal))
            {
                switch (m.Method.Name)
                {
                    case "Add":
                    case "Subtract":
                    case "Multiply":
                    case "Divide":
                    case "Remainder":
                        this.Write("(");
                        this.VisitValue(m.Arguments[0]);
                        this.Write(" ");
                        this.Write(GetOperator(m.Method.Name));
                        this.Write(" ");
                        this.VisitValue(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Negate":
                        this.Write("-");
                        this.Visit(m.Arguments[0]);
                        this.Write("");
                        return m;
                    case "Ceiling":
                        this.Write("Ceil(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Floor":                        
                        this.Write("Floor(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                            return m;
                        }
                        else if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }
                        break;
                    case "Truncate":
                        this.Write("TRUNC(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                }
            }
            else if (m.Method.DeclaringType == typeof(Math))
            {
                switch (m.Method.Name)
                {
                    case "Abs":
                    case "Acos":
                    case "Asin":
                    case "Atan":
                    case "Atan2":
                    case "Cos":
                    case "Exp":                    
                    case "Sin":
                    case "Tan":
                    case "Sqrt":
                    case "Sign":                    
                    case "Floor":
                        this.Write(m.Method.Name.ToUpper());
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Ceiling":
                        this.Write("Ceil(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Log10":
                        goto case "Log";
                    case "Log":
                        this.Write("Log(");
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("10, ");
                        }
                        else
                        {
                            this.Write(m.Arguments[1]);
                            this.Write(", ");
                        }
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;          
                    case "Pow":
                        this.Write("POWER(");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                            return m;
                        }
                        else if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }
                        break;
                    case "Truncate":
                        this.Write("TRUNC(");
                        this.Visit(m.Arguments[0]);
                        this.Write(",0)");
                        return m;
                }
            }
            if (m.Method.Name == "ToString")
            {
                if (m.Object.Type != typeof(string))
                {
                    this.Write("TO_CHAR(");
                    this.Visit(m.Object);
                    this.Write(")");
                }
                else
                {
                    this.Visit(m.Object);
                }
                return m;
            }
            else if (!m.Method.IsStatic && m.Method.Name == "CompareTo" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 1)
            {
                this.Write("(CASE WHEN ");
                this.Visit(m.Object);
                this.Write(" = ");
                this.Visit(m.Arguments[0]);
                this.Write(" THEN 0 WHEN ");
                this.Visit(m.Object);
                this.Write(" < ");
                this.Visit(m.Arguments[0]);
                this.Write(" THEN -1 ELSE 1 END)");
                return m;
            }
            else if (m.Method.IsStatic && m.Method.Name == "Compare" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 2)
            {
                this.Write("(CASE WHEN ");
                this.Visit(m.Arguments[0]);
                this.Write(" = ");
                this.Visit(m.Arguments[1]);
                this.Write(" THEN 0 WHEN ");
                this.Visit(m.Arguments[0]);
                this.Write(" < ");
                this.Visit(m.Arguments[1]);
                this.Write(" THEN -1 ELSE 1 END)");
                return m;
            }
            return base.VisitMethodCall(m);
        }
    }
}
