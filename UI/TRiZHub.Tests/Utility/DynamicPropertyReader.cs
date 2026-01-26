using System.Diagnostics.CodeAnalysis;

namespace TRiZHub.Tests.Utility
{
    [ExcludeFromCodeCoverage]
    public static class DynamicPropertyReader
    {
        public static T DynamicProperty<T>(this object source, string propertyName)
        {
           var value = (source.GetType().GetProperty(propertyName).GetValue(source));
            if(value == null)
                return default(T);
            return (T)value;
        }
    }
}
