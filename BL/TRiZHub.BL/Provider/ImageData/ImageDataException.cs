#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ImageData
{
    public class ImageDataException : Exception
    {
        public ImageDataException(string error) : base(error)
        {
        }
    }
}