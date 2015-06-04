using System.Linq.Expressions;

namespace ExprTranslator
{
    /// <summary>
    /// 表达式翻译接口
    /// </summary>
    public interface IExprTranslator
    {
        /// <summary>
        /// 翻译成字符串
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string Translate(Expression expression);
    }
}
