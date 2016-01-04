
namespace ExprTranslator.Query
{
    /// <summary>
    /// 查询Sql语句
    /// </summary>
    public class QuerySql
    {
        public QuerySql(string where, params QueryParameter[] paramArr)
        {
            whereStr = where;
            parameters = paramArr;
        }

        /// <summary>
        /// where条件
        /// </summary>
        public string whereStr { get; set; }

        /// <summary>
        /// 查询参数
        /// </summary>
        public QueryParameter[] parameters { get; set; }
    }
}
