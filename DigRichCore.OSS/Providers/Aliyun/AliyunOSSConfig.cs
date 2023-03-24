using System;
using System.Collections.Generic;
using System.Text;

namespace DigRichCore.OSS.Aliyun {
    public class AliyunOSSConfig {
        /// <summary>
        /// OSS 内网Endpoint地址
        /// </summary>
        public string PrivateEndpoint { get; set; }

        /// <summary>
        /// 文件服务器域名（即OSS外网域名）
        /// </summary>
        public string FileServerDomain { get; set; }

        /// <summary>
        /// 阿里云AccessKeyId
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// 阿里云AccessKeySecret
        /// </summary>
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// OSS BucketName
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// 图片服务器域名（开通OSS图片服务后给出的图片服务域名）
        /// </summary>
        public string ImageServerDomain { get; set; }
    }
}
