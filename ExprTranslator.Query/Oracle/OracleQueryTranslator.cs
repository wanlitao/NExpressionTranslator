using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExprTranslator.Query
{
    /// <summary>
    /// Formats a query expression into Oracle language syntax
    /// </summary>
    public class OracleQueryTranslator : QueryTranslator
    {
        private static QueryTypeSystem oracleTypeSystem = new OracleTypeSystem();

        protected override QueryTypeSystem TypeSystem { get { return oracleTypeSystem; } }

        /// <summary>
        /// 查询参数前缀
        /// </summary>
        public override string ParameterPrefix { get { return ":"; } }

        #region 静态方法
        public static new string GetQueryText(Expression expression,
            Func<MemberInfo, string> memberColumnNameGetter = null)
        {
            var queryTranslator = new OracleQueryTranslator();
            queryTranslator.MemberColumnNameConverter = memberColumnNameGetter;
            return queryTranslator.Translate(expression);
        }

        public static new QuerySql GetQuerySql(Expression expression,
            Func<MemberInfo, string> memberColumnNameGetter = null)
        {
            var queryTranslator = new OracleQueryTranslator();
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
                        this.Write("EXTRACT(day from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Month":
                        this.Write("EXTRACT(month from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Year":
                        this.Write("EXTRACT(year from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Hour":
                        this.Write("EXTRACT(hour from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Minute":
                        this.Write("EXTRACT(minute from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Second":
                        this.Write("EXTRACT(second from ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Millisecond":
                        this.Write("(EXTRACT(second from ");
                        this.Visit(m.Expression);
                        this.Write(") * 1000)");
                        return m;
                    case "DayOfWeek":
                        this.Write("(TO_CHAR(");
                        this.Visit(m.Expression);
                        this.Write(", 'D') - 1)");
                        return m;
                    case "DayOfYear":
                        this.Write("TO_CHAR(");
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
                        this.Write("ADD_MONTHS(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" * 12)");
                        return m;
                    case "AddMonths":
                        this.Write("ADD_MONTHS(");
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
                        this.Write("CEIL(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Floor":
                        this.Write("FLOOR(");
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
                        this.Write("CEIL(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Log10":
                        goto case "Log";
                    case "Log":
                        this.Write("LOG(");
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
                        this.Write(")");
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

        protected override Expression VisitNew(NewExpression nex)
        {
            if (nex.Constructor.DeclaringType == typeof(DateTime))
            {
                if (nex.Arguments.Count == 3)
                {
                    this.Write("TO_DATE(");
                    this.Visit(nex.Arguments[0]);
                    this.Write("||'-'||");
                    this.Visit(nex.Arguments[1]);
                    this.Write("||'-'||");
                    this.Visit(nex.Arguments[2]);
                    this.Write(", 'yyyy-mm-dd')");
                    return nex;
                }
                else if (nex.Arguments.Count == 6)
                {
                    this.Write("TO_DATE(");
                    this.Visit(nex.Arguments[0]);
                    this.Write("||'-'||");
                    this.Visit(nex.Arguments[1]);
                    this.Write("||'-'||");
                    this.Visit(nex.Arguments[2]);
                    this.Write("||' '||");
                    this.Visit(nex.Arguments[3]);
                    this.Write("||':'||");
                    this.Visit(nex.Arguments[4]);
                    this.Write("||':'||");
                    this.Visit(nex.Arguments[5]);
                    this.Write(", 'yyyy-mm-dd hh24:mi:ss')");
                    return nex;
                }
            }
            return base.VisitNew(nex);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Power)
            {
                this.Write("POWER(");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.Coalesce)
            {
                this.Write("COALESCE(");
                this.VisitValue(b.Left);
                this.Write(", ");
                Expression right = b.Right;
                while (right.NodeType == ExpressionType.Coalesce)
                {
                    BinaryExpression rb = (BinaryExpression)right;
                    this.VisitValue(rb.Left);
                    this.Write(", ");
                    right = rb.Right;
                }
                this.VisitValue(right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.LeftShift)
            {
                this.Write("(");
                this.VisitValue(b.Left);
                this.Write(" * POWER(2, ");
                this.VisitValue(b.Right);
                this.Write("))");
                return b;
            }
            else if (b.NodeType == ExpressionType.RightShift)
            {
                this.Write("(");
                this.VisitValue(b.Left);
                this.Write(" / POWER(2, ");
                this.VisitValue(b.Right);
                this.Write("))");
                return b;
            }
            else if (b.NodeType == ExpressionType.Add && b.Type == typeof(string))
            {
                this.Write("(");
                int n = 0;
                this.VisitConcatArg(b.Left, ref n);
                this.VisitConcatArg(b.Right, ref n);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.Divide && this.IsInteger(b.Type))
            {
                this.Write("TRUNC(");
                base.VisitBinary(b);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.Modulo)
            {
                this.Write("MOD(");
                this.VisitValue(b.Left);
                this.Write(",");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            return base.VisitBinary(b);
        }

        #region 辅助方法
        private void VisitConcatArg(Expression e, ref int n)
        {
            if (e.NodeType == ExpressionType.Add && e.Type == typeof(string))
            {
                BinaryExpression b = (BinaryExpression)e;
                VisitConcatArg(b.Left, ref n);
                VisitConcatArg(b.Right, ref n);
            }
            else
            {
                if (n > 0)
                    this.Write("||");
                this.Visit(e);
                n++;
            }
        }
        #endregion

        protected override Expression VisitValue(Expression expr)
        {
            if (IsPredicate(expr))
            {
                this.Write("CASE WHEN (");
                this.Visit(expr);
                this.Write(") THEN 1 ELSE 0 END");
                return expr;
            }
            return base.VisitValue(expr);
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            if (this.IsPredicate(c.Test))
            {
                this.Write("(CASE WHEN ");
                this.VisitPredicate(c.Test);
                this.Write(" THEN ");
                this.VisitValue(c.IfTrue);
                Expression ifFalse = c.IfFalse;
                while (ifFalse != null && ifFalse.NodeType == ExpressionType.Conditional)
                {
                    ConditionalExpression fc = (ConditionalExpression)ifFalse;
                    this.Write(" WHEN ");
                    this.VisitPredicate(fc.Test);
                    this.Write(" THEN ");
                    this.VisitValue(fc.IfTrue);
                    ifFalse = fc.IfFalse;
                }
                if (ifFalse != null)
                {
                    this.Write(" ELSE ");
                    this.VisitValue(ifFalse);
                }
                this.Write(" END)");
            }
            else
            {
                this.Write("(CASE ");
                this.VisitValue(c.Test);
                this.Write(" WHEN 0 THEN ");
                this.VisitValue(c.IfFalse);
                this.Write(" ELSE ");
                this.VisitValue(c.IfTrue);
                this.Write(" END)");
            }
            return c;
        }
        #endregion        
    }
}
