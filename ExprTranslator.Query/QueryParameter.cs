using System;

namespace ExprTranslator.Query
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class QueryParameter
    {
        private readonly string name;
        private readonly Type type;
        private readonly object value;
        private readonly QueryType queryType;
        

        public QueryParameter(string name, Type type, object value, QueryType queryType)
        {
            this.name = name;
            this.type = type;
            this.queryType = queryType;
            this.value = value;
        }

        public string Name
        {
            get { return this.name; }
        }

        public Type Type
        {
            get { return this.type; }
        }

        public QueryType QueryType
        {
            get { return this.queryType; }
        }
        
        public object Value
        {
            get { return this.value; }
        }
    }
}
