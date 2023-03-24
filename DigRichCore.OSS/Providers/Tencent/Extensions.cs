using COSXML.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DigRichCore.OSS.Tencent {
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extentions {
        /// <summary>
        /// 根据错误类型返回错误异常
        /// </summary>
        /// <returns></returns>
        public static void HandlerError(this CosResult response, string friendlyMessage = null) {
            var code = (int)response.httpCode;
            if (code < 300 || code >= 600) return;

            var message = response.httpMessage;
            throw new OSSException(
                new OSSError { Code = code, Message = friendlyMessage ?? message, ProviderMessage = message },
                new Exception($"腾讯云存储错误！"));
        }
    }
}
