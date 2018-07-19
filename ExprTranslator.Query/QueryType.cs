using System;
using System.Data;

namespace ExprTranslator.Query
{
    /// <summary>
    /// 查询数据类型
    /// </summary>
    public class QueryType
    {
        SqlDbType dbType;
        bool notNull;
        int length;
        short precision;
        short scale;

        public QueryType(SqlDbType dbType, bool notNull, int length, short precision, short scale)
        {
            this.dbType = dbType;
            this.notNull = notNull;
            this.length = length;
            this.precision = precision;
            this.scale = scale;
        }

        public DbType DbType
        {
            get { return QueryTypeSystem.GetDbType(this.dbType); }
        }

        public SqlDbType SqlDbType
        {
            get { return this.dbType; }
        }

        public int Length
        {
            get { return this.length; }
        }

        public bool NotNull
        {
            get { return this.notNull; }
        }

        public short Precision
        {
            get { return this.precision; }
        }

        public short Scale
        {
            get { return this.scale; }
        }
    }

    public class QueryTypeSystem
    {
        public virtual QueryType GetParameterType(Type type)
        {
            bool isNotNull = type.IsValueType && !TypeHelper.IsNullableType(type);
            type = TypeHelper.GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return NewType(SqlDbType.Bit, isNotNull, 0, 0, 0);
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return NewType(SqlDbType.TinyInt, isNotNull, 0, 0, 0);
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return NewType(SqlDbType.SmallInt, isNotNull, 0, 0, 0);
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return NewType(SqlDbType.Int, isNotNull, 0, 0, 0);
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return NewType(SqlDbType.BigInt, isNotNull, 0, 0, 0);
                case TypeCode.Single:
                case TypeCode.Double:
                    return NewType(SqlDbType.Float, isNotNull, 0, 0, 0);
                case TypeCode.String:
                    return NewType(SqlDbType.NVarChar, isNotNull, 0, 0, 0);
                case TypeCode.Char:
                    return NewType(SqlDbType.NChar, isNotNull, 1, 0, 0);
                case TypeCode.DateTime:
                    return NewType(SqlDbType.DateTime, isNotNull, 0, 0, 0);
                case TypeCode.Decimal:
                    return NewType(SqlDbType.Decimal, isNotNull, 0, 29, 4);
                default:
                    if (type == typeof(byte[]))
                        return NewType(SqlDbType.VarBinary, isNotNull, 0, 0, 0);
                    else if (type == typeof(Guid))
                        return NewType(SqlDbType.UniqueIdentifier, isNotNull, 0, 0, 0);
                    else if (type == typeof(DateTimeOffset))
                        return NewType(SqlDbType.DateTimeOffset, isNotNull, 0, 0, 0);
                    else if (type == typeof(TimeSpan))
                        return NewType(SqlDbType.Time, isNotNull, 0, 0, 0);
                    return null;
            }
        }

        private QueryType NewType(SqlDbType type, bool isNotNull, int length, short precision, short scale)
        {
            return new QueryType(type, isNotNull, length, precision, scale);
        }

        public virtual int StringDefaultSize
        {
            get { return Int32.MaxValue; }
        }

        public virtual int BinaryDefaultSize
        {
            get { return Int32.MaxValue; }
        }

        public static DbType GetDbType(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    return DbType.Int64;
                case SqlDbType.Binary:
                    return DbType.Binary;
                case SqlDbType.Bit:
                    return DbType.Boolean;
                case SqlDbType.Char:
                    return DbType.AnsiStringFixedLength;
                case SqlDbType.Date:
                    return DbType.Date;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                    return DbType.DateTime;
                case SqlDbType.DateTime2:
                    return DbType.DateTime2;
                case SqlDbType.DateTimeOffset:
                    return DbType.DateTimeOffset;
                case SqlDbType.Decimal:
                    return DbType.Decimal;
                case SqlDbType.Float:
                case SqlDbType.Real:
                    return DbType.Double;
                case SqlDbType.Image:
                    return DbType.Binary;
                case SqlDbType.Int:
                    return DbType.Int32;
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return DbType.Currency;
                case SqlDbType.NChar:
                    return DbType.StringFixedLength;
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    return DbType.String;
                case SqlDbType.SmallInt:
                    return DbType.Int16;
                case SqlDbType.Text:
                    return DbType.AnsiString;
                case SqlDbType.Time:
                    return DbType.Time;
                case SqlDbType.Timestamp:
                    return DbType.Binary;
                case SqlDbType.TinyInt:
                    return DbType.SByte;
                case SqlDbType.Udt:
                    return DbType.Object;
                case SqlDbType.UniqueIdentifier:
                    return DbType.Guid;
                case SqlDbType.VarBinary:
                    return DbType.Binary;
                case SqlDbType.VarChar:
                    return DbType.AnsiString;
                case SqlDbType.Variant:
                    return DbType.Object;
                case SqlDbType.Xml:
                    return DbType.String;
                default:
                    throw new InvalidOperationException(string.Format("Unhandled sql type: {0}", dbType));
            }
        }
    }
}
