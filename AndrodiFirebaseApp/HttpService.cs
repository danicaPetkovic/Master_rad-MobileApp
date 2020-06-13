using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace AndrodiFirebaseApp
{
    public class HttpService
    {
        public static void SendRequest()
        {
            var imageLenghts = SendRequestForImageLenghts();
            SendRequestForImages(imageLenghts);
        }

        private static List<int> SendRequestForImageLenghts()
        {
            var path = "http://172.20.10.2:49371/do_POST";
            WebRequest request = WebRequest.Create(path);
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write("Send image lenghts");
                streamWriter.Close();
            }

            List<int> imageLenghts = new List<int>();
            WebResponse response = request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                var responseFromServer = reader.ReadLine();
                if (responseFromServer != null)
                {
                    var responseFromServerSplit = responseFromServer.Split(',');
                    foreach (string len in responseFromServerSplit)
                        imageLenghts.Add(Int32.Parse(len));
                }
            }
            return imageLenghts;
        }

        private static void SendRequestForImages(List<int> imageLenghts)
        {
            var path = "http://172.20.10.2:49371/do_POST";
            WebRequest request = WebRequest.Create(path);
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write("Send images");
                streamWriter.Close();
            }

            WebResponse response = request.GetResponse();
            using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
            {
                var filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";
                var count = 0;
                foreach (var len in imageLenghts)
                {
                    Byte[] lnByte = reader.ReadBytes(len);

                    using (FileStream lxFS = new FileStream(filePath + count++ + ".jpg", FileMode.Create))
                    {
                        lxFS.Write(lnByte, 0, lnByte.Length);
                    }
                }
            }
            response.Close();
        }

        public static void SendResponse(string responseMessage)
        {
            var path = "http://172.20.10.2:49371/do_POST";
            WebRequest request = WebRequest.Create(path);
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(responseMessage);
                streamWriter.Close();
            }

            var filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";
            foreach (string file in Directory.EnumerateFiles(filePath))
            {
                File.Delete(file);
            }

            WebResponse response = request.GetResponse();
            response.Close();
        }

        public static void GetAllFiles()
        {
            var filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";
            foreach (string file in Directory.EnumerateFiles(filePath))
            {
                var contents = File.ReadAllBytes(file);
                Bitmap bmp;
                using (var ms = new MemoryStream(contents))
                {
                    bmp = new Bitmap(ms);
                }
            }
        }
    }
}