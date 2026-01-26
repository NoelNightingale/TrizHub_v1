#region Usings

using System;
using System.Net;
using System.Web.Hosting;
using System.Web.Mvc;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.ImageData;

#endregion

namespace TRiZHub.Controllers
{
    [AllowAnonymous]
    public class ImageController : Controller
    {
        private DataContext _Context;
        private IImageDataProvider ImageDataProvider { get; }

        #region Constructor

        public ImageController()
        {
            _Context = new DataContext();
            ImageDataProvider = new ImageDataProvider(_Context);
        }

        protected override void Dispose(bool disposing)
        {
            _Context.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        [OutputCache(Duration = 43200)]
        [HttpGet]
        public ActionResult GetJpgImage(Guid? id)
        {
            try
            {
                if (id == null || id == Guid.Empty)
                    return GetJpgDefaultImage();
                var image = ImageDataProvider.GetImage(id.Value);
                return File(image.FileData, "image/png");
            }
            catch (ImageDataException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [OutputCache(Duration = 43200)]
        [HttpGet]
        public ActionResult GetJpgDefaultImage()
        {
            try
            {
                var contentsLocation = HostingEnvironment.MapPath("~/Content");
                var file = System.IO.File.ReadAllBytes(contentsLocation + "/Images/no_image.jpg");
                return File(file, "image/jpeg");
            }
            catch (ImageDataException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [OutputCache(Duration = 43200)]
        [HttpGet]
        public ActionResult MyProfileImage()
        {
            try
            {
                var contentsLocation = HostingEnvironment.MapPath("~/Content");
                var file = System.IO.File.ReadAllBytes(contentsLocation + "/Images/no_image.jpg");
                return File(file, "image/jpeg");
            }
            catch (ImageDataException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}