#region Usings

using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.Enums
{
    public class ScorecardScoreTypeModel
    {
        private readonly ScorecardScoreType _scorecardScoreType;

        public ScorecardScoreTypeModel(ScorecardScoreType scorecardScoreType)
        {
            _scorecardScoreType = scorecardScoreType;
        }

        public int OrdinalValue
        {
            get { return (int) _scorecardScoreType; }
        }

        public string Name
        {
            get { return _scorecardScoreType.ToString(); }
        }
    }
}