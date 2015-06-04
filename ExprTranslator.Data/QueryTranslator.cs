using System;
using System.Linq.Expressions;

namespace ExprTranslator.Data
{
    /// <summary>
    /// 查询翻译器
    /// </summary>
    public class QueryTranslator : ExpressionVisitor, IExprTranslator
    {
        public string Translate(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
