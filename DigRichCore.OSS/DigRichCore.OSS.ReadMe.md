## 使用说明
#### 一、添加腾讯云存储
services.AddTencentOSS(configuration.GetSection("TencentOSSConfig"));
或
services.AddTencentOSS((cfg) => {
    cfg.AppId = "1200001";
    cfg.SecretId = "AKID0mIacO5MXT9yYtQy1P7cjlZBYgBBCCDD";
    cfg.SecretKey = "GeliZEF4hDeplLE1tb6VeXBccXFyfdsod";
    cfg.Region = "ap-chengdu";
    cfg.BucketName = "bucket-01";
    cfg.ImageServerDomain = "https://bucket-01.cos.ap-chengdu.myqcloud.com";
});
#### 二、阿里云存储
services.AddAliyunOSS(configuration.GetSection("AliyunOSSConfig"));
或
services.AddAliyunOSS((x) => {
    x.AccessKeyId = "YTAI4DDZJfCFeq1jg2ptRu9b";
    x.AccessKeySecret = "3Z1DOWDRjRk7rClDsy2swmA8TxJ8wLPC";
    x.PrivateEndpoint = "oss-cn-shanghai.aliyuncs.com";
    x.FileServerDomain = "bucket-01.oss-cn-shanghai.aliyuncs.com";
    x.ImageServerDomain = "bucket-01.oss-cn-shanghai.aliyuncs.com";
    x.BucketName = "bucket-01";
});
#### 三、本地存储
services.AddLocalOSS(configuration.GetSection("LocalOSSConfig"));
或
services.AddLocalOSS(x => { x.ImageServerDomain = "yourdomain.com"; });


上传图片案例：
services.AddAliyunOSS(configuration.GetSection("AliyunOSSConfig"));

var oss = services.BuildServiceProvider().GetRequiredService<IOSSProvider>();
            var imgUrl = oss.CreateFileByUrl("http://www.haoyunshi.com/Uploads/Thumbnail/20220218164115.jpg", "/douyin/upload/" + Guid.NewGuid() + ".jpg");
