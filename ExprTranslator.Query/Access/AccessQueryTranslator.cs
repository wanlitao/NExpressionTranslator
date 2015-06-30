using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExprTranslator.Query
{
    /// <summary>
    /// Formats a query expression into MS Access query syntax
    /// </summary>
    public class AccessQueryTranslator : QueryTranslator
    {
        private static QueryTypeSystem accessTypeSystem = new AccessTypeSystem();        

        protected override QueryTypeSystem TypeSystem { get { return accessTypeSystem; } }

        #region 静态方法
        public static new string GetQueryText(Expression expression)
        {
            var queryTranslator = new AccessQueryTranslator();
            return queryTranslator.Translate(expression);
        }

        public static new QuerySql GetQuerySql(Expression expression)
        {
            var queryTranslator = new AccessQueryTranslator();
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
                        this.Write("Len(");
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
                        this.Write("Day(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Month":
                        this.Write("Month(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Year":
                        this.Write("Year(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Hour":
                        this.Write("Hour( ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Minute":
                        this.Write("Minute(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Second":
                        this.Write("Second(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "DayOfWeek":
                        this.Write("(Weekday(");
                        this.Visit(m.Expression);
                        this.Write(") - 1)");
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
                        this.Write(" LIKE ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + '%')");
                        return m;
                    case "EndsWith":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Contains":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + '%')");
                        return m;
                    case "Concat":
                        IList<Expression> args = m.Arguments;
                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                        {
                            args = ((NewArrayExpression)args[0]).Expressions;
                        }
                        for (int i = 0, n = args.Count; i < n; i++)
                        {
                            if (i > 0) this.Write(" + ");
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
                        this.Write("UCase(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "ToLower":
                        this.Write("LCase(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "Substring":
                        this.Write("Mid(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + 1, ");
                        if (m.Arguments.Count == 2)
                        {
                            this.Visit(m.Arguments[1]);
                        }
                        else
                        {
                            this.Write("8000");
                        }
                        this.Write(")");
                        return m;
                    case "Replace":
                        this.Write("Replace(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "IndexOf":
                        this.Write("(InStr(");
                        if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            this.Visit(m.Arguments[1]);
                            this.Write(" + 1, ");
                        }
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(") - 1)");
                        return m;
                    case "Trim":
                        this.Write("Trim(");
                        this.Visit(m.Object);
                        this.Write(")");
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
                            this.Write("DateDiff(\"d\",");
                            this.Visit(m.Arguments[0]);
                            this.Write(",");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }
                        break;
                    case "AddYears":
                        this.Write("DateAdd(\"yyyy\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddMonths":
                        this.Write("DateAdd(\"m\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddDays":
                        this.Write("DateAdd(\"d\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddHours":
                        this.Write("DateAdd(\"h\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddMinutes":
                        this.Write("DateAdd(\"n\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddSeconds":
                        this.Write("DateAdd(\"s\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
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
                    case "Truncate":
                        this.Write("Fix");
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("Round(");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                            return m;
                        }
                        break;
                }
            }
            else if (m.Method.DeclaringType == typeof(Math))
            {
                switch (m.Method.Name)
                {
                    case "Abs":
                    case "Cos":
                    case "Exp":
                    case "Sin":
                    case "Tan":
                        this.Write(m.Method.Name.ToUpper());
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Sqrt":
                        this.Write("Sqr(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Sign":
                        this.Write("Sgn(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Atan":
                        this.Write("Atn(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Log":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("Log(");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                            return m;
                        }
                        break;
                    case "Pow":
                        this.Visit(m.Arguments[0]);
                        this.Write("^");
                        this.Visit(m.Arguments[1]);
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("Round(");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                            return m;
                        }
                        break;
                    case "Truncate":
                        this.Write("Fix(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                }
            }
            if (m.Method.Name == "ToString")
            {
                if (m.Object.Type != typeof(string))
                {
                    this.Write("CStr(");
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
                this.Write("IIF(");
                this.Visit(m.Object);
                this.Write(" = ");
                this.Visit(m.Arguments[0]);
                this.Write(", 0, IIF(");
                this.Visit(m.Object);
                this.Write(" < ");
                this.Visit(m.Arguments[0]);
                this.Write(", -1, 1))");
                return m;
            }
            else if (m.Method.IsStatic && m.Method.Name == "Compare" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 2)
            {
                this.Write("IIF(");
                this.Visit(m.Arguments[0]);
                this.Write(" = ");
                this.Visit(m.Arguments[1]);
                this.Write(", 0, IIF(");
                this.Visit(m.Arguments[0]);
                this.Write(" < ");
                this.Visit(m.Arguments[1]);
                this.Write(", -1, 1))");
                return m;
            }
            return base.VisitMethodCall(m);
        }

        protected override Expression VisitNew(NewExpression nex)
        {
            if (nex.Constructor.DeclaringType == typeof(DateTime))
            {
                if (nex.Arguments.Count == 3)
                {
                    this.Write("CDate(");
                    this.Visit(nex.Arguments[0]);
                    this.Write(" & '/' & ");
                    this.Visit(nex.Arguments[1]);
                    this.Write(" & '/' & ");
                    this.Visit(nex.Arguments[2]);
                    this.Write(")");
                    return nex;
                }
                else if (nex.Arguments.Count == 6)
                {
                    this.Write("CDate(");
                    this.Visit(nex.Arguments[0]);
                    this.Write(" & '/' & ");
                    this.Visit(nex.Arguments[1]);
                    this.Write(" & '/' & ");
                    this.Visit(nex.Arguments[2]);
                    this.Write(" & ' ' & ");
                    this.Visit(nex.Arguments[3]);
                    this.Write(" & ':' & ");
                    this.Visit(nex.Arguments[4]);
                    this.Write(" & + ':' & ");
                    this.Visit(nex.Arguments[5]);
                    this.Write(")");
                    return nex;
                }
            }
            return base.VisitNew(nex);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Power)
            {
                this.Write("(");
                this.VisitValue(b.Left);
                this.Write("^");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.Coalesce)
            {
                this.Write("IIF(");
                this.VisitValue(b.Left);
                this.Write(" IS NOT NULL, ");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.LeftShift)
            {
                this.Write("(");
                this.VisitValue(b.Left);
                this.Write(" * (2^");
                this.VisitValue(b.Right);
                this.Write("))");
                return b;
            }
            else if (b.NodeType == ExpressionType.RightShift)
            {
                this.Write("(");
                this.VisitValue(b.Left);
                this.Write(@" \ (2^");
                this.VisitValue(b.Right);
                this.Write("))");
                return b;
            }
            else if (b.NodeType == ExpressionType.Divide) //changed:2013/1/11 Fix 除法的上下取整的Bug
            {
                this.Write("FIX(");
                this.VisitValue(b.Left);
                this.Write(" / ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            return base.VisitBinary(b);
        }

        #region 获取Operater操作符
        protected override string GetOperator(string methodName)
        {
            if (methodName == "Remainder")
            {
                return "MOD";
            }
            else
            {
                return base.GetOperator(methodName);
            }
        }

        protected override string GetOperator(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    return "NOT";
                default:
                    return base.GetOperator(u);
            }
        }

        protected override string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    if (b.Type == typeof(bool) || b.Type == typeof(bool?))
                        return "AND";
                    return "BAND";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Or:
                    if (b.Type == typeof(bool) || b.Type == typeof(bool?))
                        return "OR";
                    return "BOR";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Modulo:
                    return "MOD";
                case ExpressionType.ExclusiveOr:
                    return "XOR";
                case ExpressionType.Divide:
                    if (this.IsInteger(b.Type))
                        return "\\"; // integer divide
                    goto default;
                default:
                    return base.GetOperator(b);
            }
        }
        #endregion

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            this.Write("IIF(");
            this.VisitPredicate(c.Test);
            this.Write(", ");
            this.VisitValue(c.IfTrue);
            this.Write(", ");
            this.VisitValue(c.IfFalse);
            this.Write(")");
            return c;
        }

        protected override void WriteConstant(Type constantType, object value)
        {
            if (value != null && value.GetType() == typeof(bool))
            {
                this.Write(((bool)value) ? -1 : 0);
            }
            else
            {
                base.WriteConstant(constantType, value);
            }
        }
        #endregion        
    }
}
