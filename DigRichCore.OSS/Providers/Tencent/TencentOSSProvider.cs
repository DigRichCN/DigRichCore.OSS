using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using COSXML.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DigRichCore.OSS.Tencent {
    public class TencentOSSProvider : IOSSProvider {
        private readonly TencentOSSConfig Config;
        private readonly CosXmlServer _cosXmlServer;
        public TencentOSSProvider(IOptions<TencentOSSConfig> cfg) {//IOptionsSnapshot<TencentOSSConfig> cfg
            // _cfg = cfg.Value;
            Config = cfg.Value;
            var config = new CosXmlConfig.Builder()
              .SetConnectionTimeoutMs(60000)  //设置连接超时时间，单位毫秒，默认45000ms
              .SetReadWriteTimeoutMs(40000)  //设置读写超时时间，单位毫秒，默认45000ms
              .IsHttps(true)  //设置默认 HTTPS 请求
              .SetAppid(Config.AppId)  //设置腾讯云账户的账户标识 APPID
              .SetRegion(Config.Region)  //设置一个默认的存储桶地域
              .SetDebugLog(true)  //显示日志
              .Build();  //创建 CosXmlConfig 对象

            //初始化 QCloudCredentialProvider，COS SDK 中提供了3种方式：永久密钥、临时密钥、自定义
            QCloudCredentialProvider cosCredentialProvider = new DefaultQCloudCredentialProvider(Config.SecretId, Config.SecretKey, 600);

            //初始化 CosXmlServer
            _cosXmlServer = new CosXmlServer(config, cosCredentialProvider);
        }
        public void AppendFile(string fileName, Stream stream) {
            throw new NotImplementedException();
        }

        public void AppendFile(string fileName, string content) {
            throw new NotImplementedException();
        }

        public void CopyFile(string sourceFileName, string destFileName, bool overwrite = false) {
            throw new NotImplementedException();
        }

        public bool CopyFolder(string fromDirName, string toDirName, bool includeFile) {
            throw new NotImplementedException();
        }

        public void CreateDir(string dirName) {
            throw new NotImplementedException();
        }

        public void CreateFile(string fileName, Stream stream, FileCreateType fileCreateType = FileCreateType.CreateNew) {
            byte[] bytes;
            using (var ms = new MemoryStream()) {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }

            var request = new PutObjectRequest(Config.BucketName, fileName, bytes) { };
            var response = _cosXmlServer.PutObject(request);

            response.HandlerError("上传对象出错!");
        }

        public void CreateFile(string fileName, string content, FileCreateType fileCreateType = FileCreateType.CreateNew) {
            throw new NotImplementedException();
        }

        public string CreateFileByUrl(string ordinalUrl, string fileName) {
            var returnUrl = "";
            try {
                WebRequest imgRequest = WebRequest.Create(ordinalUrl);
                using (HttpWebResponse res = (HttpWebResponse)imgRequest.GetResponse()) {
                    if (res.StatusCode.ToString() == "OK") {
                        using (var stream = res.GetResponseStream()) {
                            if (stream != null) {
                                CreateFile(fileName, stream, FileCreateType.CreateNew);
                                returnUrl = GetImagePath(fileName);
                            }
                        }
                    }
                }

            }
            catch (Exception ex) {
            }
            return returnUrl;
        }

        public void CreateThumbnail(string sourceFilename, string destFilename, int width, int height) {
            throw new NotImplementedException();
        }

        public void DeleteDir(string dirName, bool recursive = false) {
            throw new NotImplementedException();
        }

        public void DeleteFile(string fileName) {
            var request = new DeleteObjectRequest(Config.BucketName, fileName);
            //设置签名有效时长
            request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.Seconds), 600);
            var response = _cosXmlServer.DeleteObject(request);
            response.HandlerError("删除对象出错!");
        }

        public void DeleteFiles(List<string> fileNames) {
            throw new NotImplementedException();
        }

        public bool ExistDir(string dirName) {
            throw new NotImplementedException();
        }

        public bool ExistFile(string fileName) {
            var request = new DoesObjectExistRequest(Config.BucketName,fileName);
            return _cosXmlServer.DoesObjectExist(request);
        }

        public List<string> GetDirAndFiles(string dirName, bool self = false) {
            throw new NotImplementedException();
        }

        public MetaInfo GetDirMetaInfo(string dirName) {
            throw new NotImplementedException();
        }

        public byte[] GetFileContent(string fileName) {
            throw new NotImplementedException();
        }

        public MetaInfo GetFileMetaInfo(string fileName) {
            throw new NotImplementedException();
        }

        public string GetFilePath(string fileName) {
            throw new NotImplementedException();
        }

        public List<string> GetFiles(string dirName, bool self = false) {
            throw new NotImplementedException();
        }
        private string GetFileName(string fileName) {
            if (!string.IsNullOrWhiteSpace(fileName)) {
                if (fileName.StartsWith("http://") || fileName.StartsWith("https://"))
                    fileName = fileName.Substring(fileName.IndexOf("/", fileName.IndexOf("//") + 2) + 1);

                var index = fileName.LastIndexOf("@");
                if (index > 0)
                    fileName = fileName.Substring(0, index);

                fileName = fileName.TrimStart('/');
            }
            return fileName;
        }

        public string GetImagePath(string imageName, string styleName = null) {
            if (string.IsNullOrWhiteSpace(Config.ImageServerDomain))
                throw new OSSException("调用获取图片路径接口必须配置ImageServerDomain节点");
            imageName = GetFileName(imageName);
            if (string.IsNullOrEmpty(imageName)) {
                return imageName;
            }
            if (imageName.StartsWith("http")) {
                return imageName;
            }
            if (!string.IsNullOrEmpty(imageName)) {
                imageName = imageName.TrimStart('/');
                string url = string.Empty;
                var _imgDomain = Config.ImageServerDomain;
                if (!_imgDomain.ToLower().StartsWith("http")) {
                    _imgDomain = "https://" + _imgDomain;
                }
                if (string.IsNullOrWhiteSpace(styleName))
                    url = string.Format("{0}/{1}", _imgDomain, imageName);
                else {
                    url = string.Format("{0}/{1}/{2}", _imgDomain, imageName, styleName);
                }
                return url;
            }
            else {
                return imageName;
            }
        }

        public string GetProductSizeImage(string productPath, int index, int width = 0) {
            throw new NotImplementedException();
        }

        public void MoveFile(string sourceFileName, string destFileName, bool overwrite = false) {
            throw new NotImplementedException();
        }

        List<string> IOSSProvider.GetDirAndFiles(string dirName, bool self) {
            throw new NotImplementedException();
        }

        List<string> IOSSProvider.GetFiles(string dirName, bool self) {
            throw new NotImplementedException();
        }
        private string GetUrlByKey(string key) => $"https://{Config.BucketName}.cos.{Config.Region}.myqcloud.com/{key}";
    }
}
