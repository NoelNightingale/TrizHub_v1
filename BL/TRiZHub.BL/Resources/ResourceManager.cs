#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

#endregion

namespace TRiZHub.BL.Resources
{
    [ExcludeFromCodeCoverage]
    public class ResourceManager
    {
        //to read a file from the assembly - set the build action on the properties of the file to "Embedded Resource"
        public byte[] ReadResource(string path)
        {
            byte[] result = null;
            var assembly = Assembly.GetExecutingAssembly();
            using (var _stream = assembly.GetManifestResourceStream(path))
            {
                result = new byte[_stream.Length];
                _stream.Read(result, 0, result.Length);
            }
            return result;
        }

        public static byte[] DefaultQuestionIcon()
        {
            return new ResourceManager().ReadResource("TRiZHub.BL.Resources.Package.jpg");
        }

        public static byte[] DefaultAnswerIcon()
        {
            return new ResourceManager().ReadResource("TRiZHub.BL.Resources.Category.jpg");
        }

        public static byte[] DefaultProfileIcon()
        {
            return new ResourceManager().ReadResource("TRiZHub.BL.Resources.ProfileImage.png");
        }

        public static byte[] DefaultCategoryIcon()
        {
            return new ResourceManager().ReadResource("TRiZHub.BL.Resources.Category.jpg");
        }

        public static byte[] DefaultSellerIcon()
        {
            return new ResourceManager().ReadResource("TRiZHub.BL.Resources.Seller.png");
        }

        public static byte[] DefaultProductIcon()
        {
            return new ResourceManager().ReadResource("TRiZHub.BL.Resources.Product.png");
        }

        public static string EmailTemplate(string templateName)
        {
            var template = new ResourceManager().ReadResource("TRiZHub.BL.Resources.Email." + templateName);
            return Encoding.UTF8.GetString(template);
        }
    }
}