using System.Linq.Expressions;

namespace ExprTranslator.Query
{
    public interface IExprQueryTranslator : IExprTranslator
    {
        /// <summary>
        /// 查询参数前缀
        /// </summary>
        string ParameterPrefix { get; }        

        /// <summary>
        /// 翻译成查询Sql语句
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        QuerySql TranslateSql(Expression expression);
    }
}
