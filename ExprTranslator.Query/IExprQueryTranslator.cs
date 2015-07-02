using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExprTranslator.Query
{
    public interface IExprQueryTranslator : IExprTranslator
    {
        /// <summary>
        /// 查询参数前缀
        /// </summary>
        string ParameterPrefix { get; }

        /// <summary>
        /// 属性column名转换器
        /// </summary>
        Func<MemberInfo, string> MemberColumnNameConverter { get; set; }

        /// <summary>
        /// 翻译成查询Sql语句
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        QuerySql TranslateSql(Expression expression);
    }
}
