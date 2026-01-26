#region Usings

using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.Enums
{
    public class SecurityTypeModel
    {
        private readonly PrivilegeType _priv;

        public SecurityTypeModel(PrivilegeType priv)
        {
            _priv = priv;
        }

        public int OrdinalValue
        {
            get { return (int) _priv; }
        }

        public string Name
        {
            get { return _priv.ToString(); }
        }
    }
}