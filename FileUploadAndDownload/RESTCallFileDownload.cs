using System;
using System.IO;
using System.Net;
using System.Net.Mime;

namespace FileUploadAndDownload
{
    class RESTCallFileDownload
    {
        public static string FileDownloadRequest(string domain, string apiUrl, string cookie, string headers, string outputFolder)
        {
            string[] cookies = cookie.Split(';');
            CookieContainer cookieContainer = new CookieContainer();
            if (cookie.Length != 0)
            {

                Uri target = new Uri(domain);
                foreach (string cookie_ in cookies)
                {
                    if (cookie_.Length != 0)
                    {
                        cookieContainer.Add(new Cookie(cookie_.Split('=')[0], cookie_.Split('=')[1]) { Domain = target.Host });
                    }
                }
            }
            string remoteUrl = string.Format(apiUrl);
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(remoteUrl);
            httpRequest.CookieContainer = cookieContainer;

            if (headers.Length != 0)
            {
                string[] headers_ = headers.Split(';');
                foreach (string header in headers_)
                {
                    if (header.Length != 0)
                    {
                        if (header.Split('=')[0] == "Accept")
                        {
                            httpRequest.Accept = header.Split('=')[1] + ";";
                        }
                        else
                        {
                            httpRequest.Headers.Add(header.Split('=')[0], header.Split('=')[1]);
                        }

                    }
                }
            }
            try
            {
                WebResponse response = httpRequest.GetResponse();
                string contentDisposition = response.Headers["Content-Disposition"];
                if (contentDisposition != "")
                {
                    var cp = new ContentDisposition(contentDisposition);
                    string fileName = cp.FileName;
                    using (Stream output = File.OpenWrite("" + outputFolder + "/" + fileName))
                    using (Stream input = response.GetResponseStream())
                    {
                        input.CopyTo(output);
                    }

                    return "File saved in  : " + "" + outputFolder + "/" + fileName;
                }
                else
                {
                    return "Unable to retrive file name & extension from the response ,  contentDisposition is empty";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


    }
}
