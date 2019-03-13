using SignalrChat.Models;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web;

namespace SignalRApp.App_Data
{
    public class UserHelper
    {
        /// <summary>
        /// Generate base64 string from image path
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string LoadImage(string Path)
        {
            string base64String = null;
            if (Path !="")
            {
                using (Image image = Image.FromFile(Path))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                }
            }
            return "data:image/jpg;base64," + base64String;

        }

        /// <summary>
        /// Retrieve image path to be saved in db.
        /// </summary>
        /// <param name="ImageFile"></param>
        /// <returns></returns>
        public  string ImageProcessor(HttpPostedFileBase ImageFile)
        {
            string FileName = null;
            string UploadPath = null;

            if (ImageFile != null)
            {
                FileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                string FileExtension = Path.GetExtension(ImageFile.FileName);
                FileName = FileName.Trim() + FileExtension;
                UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
            }
            return UploadPath + FileName;
        }
    }
}
