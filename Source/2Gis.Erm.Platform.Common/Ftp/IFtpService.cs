using System.IO;
using System.Net;

namespace DoubleGis.Erm.Platform.Common.Ftp
{
    public interface IFtpService
    {
        void UploadFile(string ftpFolder, NetworkCredential networkCredential, string fileName, Stream stream);
        void UploadFile(string ftpFolder, NetworkCredential networkCredential, string filePath);
    }
}