#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

#endregion

namespace TCR.Lib.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class RequiredTrueAttribute : ValidationAttribute
    {
        public RequiredTrueAttribute(bool accepted)
        {
            Accepted = accepted;
        }

        public RequiredTrueAttribute()
        {
            Accepted = true;
        }

        // Internal field to hold the mask value.

        public bool Accepted { get; }

        public override bool IsValid(object value)
        {
            var isAccepted = (bool) value;
            return isAccepted;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Accepted);
        }
    }
}