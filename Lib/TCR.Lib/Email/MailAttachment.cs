#region Usings

using System;
using System.IO;

#endregion

namespace TCR.Lib.Email
{
    public class MailAttachment : IDisposable
    {
        public MailAttachment(byte[] content, string fileName)
        {
            Content = content;
            FileName = fileName;
            ContentAttachment = new MemoryStream(content);
            ContentAttachment.Position = 0;
        }

        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public MemoryStream ContentAttachment { get; }

        public void Dispose()
        {
            ContentAttachment.Close();
            ContentAttachment.Dispose();
        }
    }
}