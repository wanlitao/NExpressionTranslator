using System;
using System.Linq.Expressions;

namespace ExprTranslator.Query
{
    /// <summary>
    /// 查询翻译器
    /// </summary>
    public class QueryTranslator : ExpressionVisitor, IExprQueryTranslator
    {
        public string Translate(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
