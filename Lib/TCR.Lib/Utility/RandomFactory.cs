#region Usings

using System;
using System.Linq;
using System.Security.Cryptography;

#endregion

namespace TCR.Lib.Utility
{
    public static class RandomFactory
    {
        //minValue is inclusive and maxValue is exclusive!
        //http://msdn.microsoft.com/en-us/library/2dx6wyd4.aspx
        public static int GenerateInteger(int minValue, int maxValue)
        {
            var bytes = new byte[1];
            RandomNumberGenerator.Create().GetBytes(bytes);
            var random = new Random(bytes[0]);
            var randomNumber = random.Next(minValue, maxValue);
            return randomNumber;
        }

        public static decimal GenerateDecimal(int minValue, int maxValue, int digits)
        {
            var bytes = new byte[1];
            RandomNumberGenerator.Create().GetBytes(bytes);
            var random = new Random(bytes[0]);
            var randomInt = random.Next(minValue, maxValue);
            var randomDbl = random.NextDouble();

            return (decimal) (randomInt + Math.Round(randomDbl, digits));
        }

        public static bool GenerateBoolean()
        {
            var number = GenerateInteger(0, 2);
            return Convert.ToBoolean(number);
        }

        public static DateTime GenerateDateTime(DateTime minValue, DateTime maxValue)
        {
            var bytes = new byte[1];
            RandomNumberGenerator.Create().GetBytes(bytes);
            var random = new Random(bytes[0]);
            var randomDbl = random.NextDouble();

            var timeSpan = maxValue - minValue;
            var randomSpan = new TimeSpan((long) (timeSpan.Ticks*randomDbl));
            return minValue + randomSpan;
        }

        public static TEnum GenerateEnum<TEnum>()
        {
            Type enumType;
            if (IsNullable(typeof (TEnum)))
                enumType = Nullable.GetUnderlyingType(typeof (TEnum));
            else
                enumType = typeof (TEnum);

            if (!enumType.IsEnum)
                throw new Exception("Generic parameter must be an enum.");

            var enumMembers = enumType.GetFields()
                .Where(f => f.IsLiteral)
                .Select(f => (TEnum) Enum.Parse(enumType, f.GetValue(null).ToString(), false))
                .ToList();

            var randomEnum = enumMembers.OrderBy(x => Guid.NewGuid()).Take(1).FirstOrDefault();
            return randomEnum;
        }

        #region Helper Methods

        public static bool IsNullable(Type type)
        {
            bool isNullable;

            try
            {
                isNullable = type.IsGenericType && (type.GetGenericTypeDefinition() == typeof (Nullable<>));
            }
            catch
            {
                isNullable = false;
            }

            return isNullable;
        }

        #endregion

        public static string GeneratePassword(int lowercase, int uppercase, int numerics, int specials)
        {
            var lowers = "abcdefghijklmnopqrstuvwxyz";
            var uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var number = "0123456789";
            //string specialChars = "~@#$%^&*";

            var random = new Random();

            var generated = "!";
            for (var i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                    );

            for (var i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                    );

            for (var i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                    );

            //for (int i = 1; i <= specials; i++)
            //    generated = generated.Insert(
            //        random.Next(generated.Length),
            //        specialChars[random.Next(number.Length - 1)].ToString()
            //    );

            return generated.Replace("!", string.Empty);
        }
    }
}