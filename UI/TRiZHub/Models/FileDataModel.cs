#region Usings

using System;

#endregion

namespace TRiZHub.Models
{
    public class FileDataModel
    {
        public string FileName { get; set; }
        public string FileDataBase64 { get; set; }

        public byte[] FileData
        {
            get
            {
                if (string.IsNullOrEmpty(FileDataBase64))
                    return null;
                if (FileDataBase64.Contains(","))
                {
                    var tmp = FileDataBase64.Split(",".ToCharArray());
                    try
                    {
                        return Convert.FromBase64String(tmp[1]);
                    }
                    catch (Exception)
                    {
                        // If File data Byte array failed. Then for editing 
                        // purposes the model should still be passed to the view empty.
                        return null;
                    }
                }
                try
                {
                    return Convert.FromBase64String(FileDataBase64);
                }
                catch (Exception)
                {
                    // If File data Byte array failed. Then for editing 
                    // purposes the model should still be passed to the view empty.
                    return null;
                }
            }
        }
    }
}