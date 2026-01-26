#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.ImageData
{
    public class ImageDataProvider : TRiZHubProvider, IImageDataProvider
    {
        public ImageDataProvider(DataContext context) : base(context)
        {
        }

        public ImageDataProvider(DataContext context, ICurrentUser currentUser) : base(context, currentUser)
        {
        }


        public Entities.MasterData.ImageData ImageDataSave(Guid? id, string filename, byte[] fileData,
            PrivilegeType privilege,
            bool removeExistingImage = false)
        {
            if (CurrentUser == null)
                throw new ImageDataException("No user account is setup with this action.");

            switch (privilege)
            {
                case PrivilegeType.UserMaintenance:
                    Authenticate(PrivilegeType.UserMaintenance);
                    break;
            }

            // If remove image is active but Id is null, then Image is new Image file.
            if (removeExistingImage && id == null)
                removeExistingImage = false;

            Entities.MasterData.ImageData item = null;

            if (id != null) // check if found by id when Id is set
                item = DataContext.ImageDataSet.Single(r => r.Id == id);

            // Remove existing image
            if (removeExistingImage)
            {
                DataContext.ImageDataSet.Remove(item);
                item = null;
            }

            // create new image
            if (item == null)
            {
                item = new Entities.MasterData.ImageData();
                DataContext.ImageDataSet.Add(item);
            }

            item.FileName = filename;
            item.FileData = Entities.MasterData.ImageData.CreateGenericImage(fileData);
            //File Data must be limited by size before save to db - in case the user uploaded a 4Mb file. 

            DataContextSaveChanges();

            return item;
        }

        public Entities.MasterData.ImageData GetImage(Guid? id)
        {
            return DataContext.ImageDataSet.Single(a => a.Id == id);
        }

        public IQueryable<Entities.MasterData.ImageData> GetImageList()
        {
            return DataContext.ImageDataSet;
        }
    }
}