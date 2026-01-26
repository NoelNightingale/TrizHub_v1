#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

#endregion

namespace TCR.Lib.BL
{
    public class AuditedEntity : IAuditedEntity
    {
        public virtual string Describe()
        {
            var sb = new StringBuilder();

            foreach (var pi in GetType().GetProperties())
            {
                var colType = pi.PropertyType;

                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof (Nullable<>)))
                {
                    colType = colType.GetGenericArguments()[0];
                }


                if (IsAllowedType(colType) && IsNotMapperProperty(pi))
                {
                    sb.Append("[" + pi.Name + " = " + ReadValue(colType, pi.GetValue(this, null)) + "]");
                }
            }
            return sb.ToString();
        }

        private string ReadValue(Type colType, object val)
        {
            if (val == null)
                return "null";

            switch (Type.GetTypeCode(colType))
            {
                case TypeCode.Boolean:
                    return (bool) val ? "true" : "false";
                case TypeCode.Byte:
                    return val.ToString();
                case TypeCode.Char:
                    return val.ToString();
                case TypeCode.DBNull:
                    return "null";
                case TypeCode.DateTime:
                    return ((DateTime) val).ToString("yyyy-MM-dd HH:mm:ss");
                case TypeCode.Decimal:
                    return ((decimal) val).ToString("###,###,##0.00");
                case TypeCode.Double:
                    return ((double) val).ToString("###,###,##0.00");
                case TypeCode.Empty:
                    return "";
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return val.ToString();
                case TypeCode.Object:
                    if (colType == typeof (Guid))
                        return ((Guid) val).ToString();
                    return "object";
            }
            return "unknown type";
        }

        private static bool IsNotMapperProperty(PropertyInfo pi)
        {
            var notMapped = pi.GetCustomAttribute<NotMappedAttribute>();
            return notMapped == null;
        }

        private static bool IsAllowedType(Type colType)
        {
            switch (Type.GetTypeCode(colType))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DBNull:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                {
                    if (colType == typeof (Guid))
                        return true;
                }
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}