using System;
using System.Collections.Generic;
using System.Text;

namespace DigRichCore.OSS.Tencent {
    public class TencentOSSConfig {
        /// <summary>
        ///     应用ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        ///     秘钥id
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        ///     秘钥Key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        ///     区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        ///    存储桶名称
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// 访问地址
        /// </summary>
        public string ImageServerDomain { get; set; }
    }
}
