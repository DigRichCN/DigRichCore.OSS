using Aliyun.OSS;
using DigRichCore.OSS;
using DigRichCore.OSS.Aliyun;
using DigRichCore.OSS.Providers.Local;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DigRichCore.UnitTest01 {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            IServiceCollection services = new ServiceCollection();
            //services.AddTencentOSS((cfg) => {
            //    cfg.AppId = "1251080462";
            //    cfg.SecretId = "AKID0mIacO5MXT9yYtQy1P7cjlZBYgHdHZEc";
            //    cfg.SecretKey = "GeliZEF4hDeplLE1tb6VeXBccWBbrscf";
            //    cfg.Region = "ap-chengdu";
            //    cfg.BucketName = "digrich-1251080462";
            //    cfg.ImageServerDomain = "https://digrich-1251080462.cos.ap-chengdu.myqcloud.com";
            //});
            //services.AddAliyunOSS((x) => {
            //    x.AccessKeyId = "LTAI4GGZJfUNeq1jg2ptRu9b";
            //    x.AccessKeySecret = "3Z1LGYRjRk7rClDsya3mA8TxJ8wICA";
            //    x.PrivateEndpoint = "oss-cn-shanghai.aliyuncs.com";
            //    x.FileServerDomain = "live-tools.oss-cn-shanghai.aliyuncs.com";
            //    x.ImageServerDomain = "live-tools.oss-cn-shanghai.aliyuncs.com";
            //    x.BucketName = "live-tools";
            //});

            services.AddLocalOSS(x => { x.ImageServerDomain = "baidu.com"; });

            var serviceProvider = services.BuildServiceProvider();
            var oss = serviceProvider.GetRequiredService<IOSSProvider>();
            var imgUrl = oss.CreateFileByUrl("http://www.haoyunshi.com/Uploads/Thumbnail/20220218164115.jpg", "/douyin/upload/" + Guid.NewGuid() + ".jpg");

            Console.WriteLine("ossUrl:"+imgUrl);
        }
    }
}
