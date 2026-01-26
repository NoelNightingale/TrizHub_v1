#region Usings

using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.Enums
{
    public class ClientTypeModel
    {
        private readonly ClientEntityType _client;

        public ClientTypeModel(ClientEntityType client)
        {
            _client = client;
        }

        public int OrdinalValue
        {
            get { return (int) _client; }
        }

        public string Name
        {
            get { return _client.ToString(); }
        }
    }
}