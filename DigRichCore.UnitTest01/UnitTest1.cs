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

            services.AddLocalOSS(x => { x.ImageServerDomain = "baidu.com"; });

            var serviceProvider = services.BuildServiceProvider();
            var oss = serviceProvider.GetRequiredService<IOSSProvider>();
            var imgUrl = oss.CreateFileByUrl("http://www.haoyunshi.com/Uploads/Thumbnail/20220218164115.jpg", "/douyin/upload/" + Guid.NewGuid() + ".jpg");

            Console.WriteLine("ossUrl:"+imgUrl);
        }
    }
}
