using Aliyun.OSS;
using DigRich.Common.Extension;
using DigRich.Common.Helper;
using DigRichCore.OSS.Aliyun;
using DigRichCore.OSS.Providers.Local;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace DigRichCore.OSS.Local {
    public class LocalOSSProvider : IOSSProvider {
        private readonly LocalOSSConfig Config;
        public LocalOSSProvider(IOptionsSnapshot<LocalOSSConfig> _config) {
            Config = _config.Value;
        }
        public void AppendFile(string fileName, string content) {
            byte[] fileContent = System.Text.Encoding.UTF8.GetBytes(content);
            var path = GetPhysicalPath(fileName);
            //在路径fullPath下关联一个FileStream对象
            FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            //将内容写入文件流
            fs.Write(fileContent, 0, fileContent.Length);
            //必须关闭文件流，否则得到的文本什么内容都没有
            fs.Close();//必须关闭
        }
        private byte[] StreamToBytes(Stream stream) {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        public void AppendFile(string fileName, System.IO.Stream stream) {
            var path = GetPhysicalPath(fileName);
            var fileContent = StreamToBytes(stream);
            FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            //将内容写入文件流
            fs.Write(fileContent, 0, fileContent.Length);
            //必须关闭文件流，否则得到的文本什么内容都没有
            fs.Close();//必须关闭
        }

        public void CopyFile(string sourceFileName, string destFileName, bool overwrite = false) {
            if (string.IsNullOrWhiteSpace(sourceFileName)) {
                throw new OSSException(OSSErrorMsg.FileNotExist.ToDescription());
            }
            var s = GetPhysicalPath(sourceFileName);
            var d = GetPhysicalPath(destFileName);
            var dir = d.Remove(d.LastIndexOf("\\"));
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            if (!overwrite && ExistFile(destFileName)) //如果不可覆盖文件且目标文件已存在,传相对路径
            {
                throw new OSSException(OSSErrorMsg.FileExist.ToDescription());
            }
            else {
                File.Copy(s, d, overwrite);
            }
        }

        public void CreateDir(string dirName) {
            var path = GetPhysicalPath(dirName);

            Directory.CreateDirectory(path);
        }

        public void CreateFile(string fileName, string content, FileCreateType fileCreateType = FileCreateType.CreateNew) {
            byte[] fileContent = System.Text.Encoding.UTF8.GetBytes(content);
            var path = GetPhysicalPath(fileName);
            var dir = path.Remove(path.LastIndexOf("\\"));
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            if (fileCreateType == FileCreateType.CreateNew) {
                if (!File.Exists(path)) {
                    FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                    //将内容写入文件流
                    fs.Write(fileContent, 0, fileContent.Length);
                    //必须关闭文件流，否则得到的文本什么内容都没有
                    fs.Close();//必须关闭
                }
                else {
                    throw new OSSException(OSSErrorMsg.FileExist.ToDescription());
                }
            }
            else {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                //将内容写入文件流
                fs.Write(fileContent, 0, fileContent.Length);
                //必须关闭文件流，否则得到的文本什么内容都没有
                fs.Close();//必须关闭
            }
        }

        public void CreateFile(string fileName, System.IO.Stream stream, FileCreateType fileCreateType = FileCreateType.CreateNew) {
            var path = GetPhysicalPath(fileName);
            var dir = path.Remove(path.LastIndexOf("\\"));
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            if (fileCreateType == FileCreateType.CreateNew) {
                if (!File.Exists(path)) {
                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                    //将内容写入文件流
                    var fileContent = StreamToBytes(stream);
                    fs.Write(fileContent, 0, fileContent.Length);
                    //必须关闭文件流，否则得到的文本什么内容都没有
                    fs.Close();//必须关闭
                }
                else {
                    throw new OSSException(OSSErrorMsg.FileExist.ToDescription());
                }
            }
            else {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                //将内容写入文件流
                var fileContent = StreamToBytes(stream);
                fs.Write(fileContent, 0, fileContent.Length);
                //必须关闭文件流，否则得到的文本什么内容都没有
                fs.Close();//必须关闭
            }
        }

        public void DeleteDir(string dirName, bool recursive = false) {
            var path = GetPhysicalPath(dirName);
            if (Directory.Exists(path)) {
                Directory.Delete(path, recursive);
            }
            else {
                throw new OSSException(OSSErrorMsg.DirNotExist.ToDescription());
            }
        }

        public void DeleteFile(string fileName) {
            var file = GetPhysicalPath(fileName);
            if (ExistFile(fileName)) {
                File.Delete(file);
            }
        }

        public void DeleteFiles(List<string> fileNames) {

            foreach (var file in fileNames) {
                var pfile = GetPhysicalPath(file);
                if (ExistFile(file)) {
                    File.Delete(pfile);
                }
            }
        }

        public bool ExistDir(string dirName) {
            var file = GetPhysicalPath(dirName);
            var result = Directory.Exists(file);
            return result;
        }

        public bool ExistFile(string fileName) {
            var file = GetPhysicalPath(fileName);
            var result = File.Exists(file);
            return result;
        }


        public List<string> GetDirAndFiles(string dirName, bool self = false) {
            List<string> files = new List<string>();
            var path = GetPhysicalPath(dirName);
            if (self) {
                files.Add(path);
            }
            files.AddRange(GetDirAndFiles(path));
            return files;
        }

        public MetaInfo GetDirMetaInfo(string dirName) {
            var path = GetPhysicalPath(dirName);
            MetaInfo info = new MetaInfo();
            info.LastModifiedTime = Directory.GetLastWriteTime(path);
            info.ContentLength = IOHelper.GetDirectoryLength(path);
            return info;
        }

        public byte[] GetFileContent(string fileName) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                throw new OSSException(OSSErrorMsg.FileNotExist.ToDescription());
            }
            var f = GetPhysicalPath(fileName);
            FileStream fs = new FileStream(f, FileMode.Open);
            byte[] byteData = new byte[fs.Length];
            fs.Read(byteData, 0, byteData.Length);
            fs.Close();
            return byteData;
        }

        public MetaInfo GetFileMetaInfo(string fileName) {
            MetaInfo minfo = new MetaInfo();
            //var file = GetPhysicalPath(fileName);
            //FileInfo finfo = new FileInfo(file);
            //minfo.ContentLength = finfo.Length;
            //var contentType = MimeMapping.GetMimeMapping(file);
            //minfo.ContentType = contentType;
            //minfo.LastModifiedTime = finfo.LastWriteTime;
            //// minfo.ObjectType
            return minfo;
        }

        public string GetFilePath(string fileName) {
            return string.Format("{0}/{1}", GetHttpUrl(), fileName);
        }

        public List<string> GetFiles(string dirName, bool self = false) {
            List<string> files = new List<string>();
            var path = GetPhysicalPath(dirName);
            if (self) {
                files.Add(path);
            }
            files.AddRange(GetAllFiles(path));
            return files;
        }

        private List<string> GetAllFiles(string path) {
            List<string> f = new List<string>();
            var files = Directory.GetFiles(path);
            f.AddRange(files);
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs) {
                f.AddRange(GetAllFiles(dir));
            }
            return f;
        }

        private List<string> GetDirAndFiles(string path) {
            List<string> f = new List<string>();
            var files = Directory.GetFiles(path);
            f.AddRange(files);
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs) {
                f.Add(dir);
                f.AddRange(GetAllFiles(dir));
            }
            return f;
        }

        public string GetImagePath(string imageName, string styleName = null) {
            if (string.IsNullOrWhiteSpace(Config.ImageServerDomain)|| imageName.ToLower().StartsWith("http")) {
                return imageName;
            }
            var _imgDomain = Config.ImageServerDomain;
            if (!_imgDomain.ToLower().StartsWith("http")) {
                _imgDomain = "https://" + _imgDomain;
            }
            imageName = imageName.TrimStart('/');
            return String.Format("{0}/{1}",_imgDomain,imageName.TrimStart('/'));
        }

        public void MoveFile(string sourceFileName, string destFileName, bool overwrite = false) {
            if (string.IsNullOrWhiteSpace(sourceFileName)) {
                throw new OSSException(OSSErrorMsg.FileNotExist.ToDescription());
            }
            var s = GetPhysicalPath(sourceFileName);
            var d = GetPhysicalPath(destFileName);
            var dir = d.Remove(d.LastIndexOf("\\"));
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            if (!overwrite && ExistFile(d)) //如果不可覆盖文件且目标文件已存在
            {
                throw new OSSException(OSSErrorMsg.FileExist.ToDescription());
            }
            else {
                File.Move(s, d);
            }

        }

        private string GetHttpUrl() {
            //string host = WebHelper.GetHost();
            //var port = WebHelper.GetPort();
            //var portPre = port == "80" ? "" : ":" + port;
            //return "http://" + host + portPre + "/";
            return "";
        }

        private string GetFileName(string fileName) {
            if (!string.IsNullOrWhiteSpace(fileName) && fileName.StartsWith("/")) {
                fileName = fileName.Substring(1);
            }
            return fileName;
        }

        private string GetPhysicalPath(string fileName) {
            if (!string.IsNullOrWhiteSpace(fileName)) {
                if (fileName.StartsWith("http://") || fileName.StartsWith("https://"))
                    fileName = fileName.Substring(fileName.IndexOf("/", fileName.IndexOf("//") + 2) + 1);

                var index = fileName.LastIndexOf("@");
                if (index > 0)
                    fileName = fileName.Substring(0, index);
            }

            return IOHelper.GetMapPath(fileName);
        }

        private string GetDirName(string dirName) {
            if (dirName.StartsWith("/")) {
                dirName = dirName.Substring(1);
            }
            if (!dirName.EndsWith("/")) {
                dirName = dirName + "/";
            }
            return dirName;
        }


        public void CreateThumbnail(string sourceFilename, string destFilename, int width, int height) {
            var s = GetPhysicalPath(sourceFilename);
            var d = GetPhysicalPath(destFilename);
            CreateThumbnail(s, d, width, height);
        }
        /// <summary>
        /// 生成缩略图 GenerateThumb
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="thumbPath">缩略图路径</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>   
        private static void CreateThumbnail2(string imagePath, string thumbPath, int width, int height) {
            string mode = "Cut";
            Image image = Image.FromFile(imagePath);

            string extension = imagePath.Substring(imagePath.LastIndexOf(".")).ToLower();
            ImageFormat imageFormat = null;
            switch (extension) {
                case ".jpg":
                case ".jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                case ".png":
                    imageFormat = ImageFormat.Png;
                    break;
                case ".gif":
                    imageFormat = ImageFormat.Gif;
                    break;
                default:
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }

            int toWidth = width > 0 ? width : image.Width;
            int toHeight = height > 0 ? height : image.Height;

            int x = 0;
            int y = 0;
            int ow = image.Width;
            int oh = image.Height;

            switch (mode) {
                case "HW"://指定高宽缩放（可能变形）           
                    break;
                case "W"://指定宽，高按比例             
                    toHeight = image.Height * width / image.Width;
                    break;
                case "H"://指定高，宽按比例
                    toWidth = image.Width * height / image.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）           
                    if ((double)image.Width / (double)image.Height > (double)toWidth / (double)toHeight) {
                        oh = image.Height;
                        ow = image.Height * toWidth / toHeight;
                        y = 0;
                        x = (image.Width - ow) / 2;
                    }
                    else {
                        ow = image.Width;
                        oh = image.Width * height / toWidth;
                        x = 0;
                        y = (image.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp
            Image bitmap = new Bitmap(toWidth, toHeight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //配置JPEG压缩引擎
            EncoderParameters encoderParams = new EncoderParameters();
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            encoderParams.Param[0] = encoderParam;

            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;

            for (int xx = 0; xx < arrayICI.Length; xx++) {
                if (arrayICI[xx].FormatDescription.Equals("JPEG")) {
                    jpegICI = arrayICI[xx];
                    break;
                }
            }

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(image,
                        new Rectangle(0, 0, toWidth, toHeight),
                        new Rectangle(x, y, ow, oh),
                        GraphicsUnit.Pixel);

            try {
                //bitmap.Save(thumbPath, imageFormat);
                bitmap.Save(thumbPath, jpegICI, encoderParams);
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                if (g != null)
                    g.Dispose();
                if (bitmap != null)
                    bitmap.Dispose();
                if (image != null)
                    image.Dispose();
            }
        }

        public string GetProductSizeImage(string productPath, int index, int width = 0) {
            // string image = GetFileName(productPath);

            if (!string.IsNullOrEmpty(productPath)) {
                if (string.IsNullOrEmpty(Path.GetExtension(productPath)) && !productPath.EndsWith("/")) {
                    productPath = productPath + "/";
                }
                productPath = Path.GetDirectoryName(productPath).Replace("\\", "/");

                string url = string.Empty;
                if (width == 0) {
                    url = string.Format("{0}/{1}.png", productPath, index);
                }

                if (width != 0) {

                    url = string.Format(productPath + "/{0}_{1}.png", index, width);
                }
                return url;
            }
            else {
                return productPath;
            }
        }

        public bool CopyFolder(string fromDirName, string toDirName, bool includeFile) {
            throw new NotImplementedException();
        }

        public string CreateFileByUrl(string ordinalUrl, string fileName) {
            var returnUrl = "";
            try {
                //WebRequest imgRequest = WebRequest.Create(ordinalUrl);

                //using (HttpWebResponse res = (HttpWebResponse)imgRequest.GetResponse()) {
                //    if (res.StatusCode.ToString() == "OK") {
                //        using (var stream = res.GetResponseStream()) {
                //            if (stream != null) {
                //                fileName = GetFileName(fileName);
                //                CreateFile(fileName, stream);
                //                returnUrl = GetImagePath(fileName);
                //            }
                //        }
                //    }
                //}

                using (WebClient client = new WebClient()) {
                    fileName = GetFileName(fileName);
                    client.DownloadFile(new Uri(ordinalUrl), fileName);
                    returnUrl = GetImagePath(fileName);
                }

            }
            catch (Exception ex) {
            }
            return returnUrl;
        }
    }
}
