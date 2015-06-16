
namespace ExprTranslator.Query
{
    public interface IExprQueryTranslator : IExprTranslator
    {
        /// <summary>
        /// 查询参数前缀
        /// </summary>
        string ParameterPrefix { get; }

        /// <summary>
        /// 查询参数列表
        /// </summary>
        QueryParameter[] Parameters { get; }
    }
}
