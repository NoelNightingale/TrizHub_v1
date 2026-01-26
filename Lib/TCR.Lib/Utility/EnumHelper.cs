#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace TCR.Lib.Utility
{
    public static class EnumHelper
    {
        /// <summary>
        ///     Gets all the Enum values as a IEnumerable list
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TEnum> GetAllValues<TEnum>()
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            return Enum.GetValues(typeof (TEnum)).Cast<TEnum>();
        }

        /// <summary>
        ///     Gets the enum object by passing the string of the enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumString"></param>
        /// <returns></returns>
        public static TEnum GetEnumValue<TEnum>(string enumString)
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            var enumType = typeof (TEnum);
            if (!enumType.IsEnum)
                throw new Exception("TEnum must be an Enumeration type.");
            TEnum val;
            return Enum.TryParse(enumString, true, out val) ? val : default(TEnum);
        }

        /// <summary>
        ///     Gets the enum object by passing the int value of the enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumInt"></param>
        /// <returns></returns>
        public static TEnum GetEnumValue<TEnum>(int enumInt)
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            var enumType = typeof (TEnum);
            if (!enumType.IsEnum)
                throw new Exception("TEnum must be an Enumeration type.");
            return (TEnum) Enum.ToObject(enumType, enumInt);
        }

        public static string[] GetEnumStringValues<TEnum>()
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            return GetAllValues<TEnum>().Select(a => a.ToString()).ToArray();
        }
    }
}