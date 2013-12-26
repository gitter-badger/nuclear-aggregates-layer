using System;
using System.IO;
using System.Net;

using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.Platform.Common.Ftp
{
    public sealed class FtpService : IFtpService
    {
        public void UploadFile(string ftpFolder, NetworkCredential networkCredential, string fileName, Stream stream)
        {
            if (string.IsNullOrEmpty(ftpFolder))
            {
                throw new ArgumentNullException("ftpFolder");
            }

            if (null == networkCredential)
            {
                throw new ArgumentNullException("networkCredential");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (null == stream)
            {
                throw new ArgumentNullException("stream");
            }

            var request = (FtpWebRequest)WebRequest.Create(Path.Combine(ftpFolder, Path.GetFileName(fileName)));
            request.Credentials = networkCredential;
            //request.EnableSsl = false;
            request.UseBinary = true;
            request.Proxy = null;

            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Timeout = 10000000;
            request.ReadWriteTimeout = 10000000;
            request.KeepAlive = true;

            using (var output = request.GetRequestStream())
            {
                var buffer = new byte[1024];
                var lastBytesRead = -1;
                while (lastBytesRead != 0)
                {
                    lastBytesRead = stream.Read(buffer, 0, 1024);
                    if (lastBytesRead > 0)
                    {
                        output.Write(buffer, 0, lastBytesRead);
                    }
                }
            }

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != FtpStatusCode.ClosingData)
                {
                    throw new Exception(string.Format(ResPlatform.AnErrorOccuredWhileUpdatingFileOnFtp, ftpFolder));
                }
            }
        }

        public void UploadFile(string ftpFolder, NetworkCredential networkCredential, string filePath)
        {
            if (string.IsNullOrEmpty(ftpFolder))
            {
                throw new ArgumentNullException("ftpFolder");
            }

            if (null == networkCredential)
            {
                throw new ArgumentNullException("networkCredential");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            using (var stream = File.Open(filePath, FileMode.Open))
            {
                UploadFile(ftpFolder, networkCredential, Path.GetFileName(filePath), stream);
            }
        }
    }
}