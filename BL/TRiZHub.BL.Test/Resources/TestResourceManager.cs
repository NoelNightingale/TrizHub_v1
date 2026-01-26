using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TRiZHub.BL.Test.Resources
{
    [ExcludeFromCodeCoverage]
    public class TestResourceManager
    {
        //to read a file from the assembly - set the build action on the properties of the file to "Embedded Resource"
        public  byte[] ReadResource(string path)
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

        public static byte[] FakeImage()
        {
            return new TestResourceManager().ReadResource("TRiZHub.BL.Test.Resources.FakeImage.png");
        }


      
    }
}
