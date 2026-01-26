#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.ImageData
{
    public interface IImageDataProvider : ITRiZHubProvider
    {
        Entities.MasterData.ImageData ImageDataSave(Guid? id, string filename, byte[] fileData, PrivilegeType privilege,
            bool removeExistingImage = false);

        Entities.MasterData.ImageData GetImage(Guid? id);
        IQueryable<Entities.MasterData.ImageData> GetImageList();
    }
}