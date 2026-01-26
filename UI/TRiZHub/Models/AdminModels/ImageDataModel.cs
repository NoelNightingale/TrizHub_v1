#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.AdminModels
{
    public class ImageDataModel
    {
        public Guid? Id { get; set; }

        [Required]
        public virtual string FileName { get; set; }

        [Required]
        public virtual byte[] FileData { get; set; }

        public string ImageURL { get; set; }
    }
}