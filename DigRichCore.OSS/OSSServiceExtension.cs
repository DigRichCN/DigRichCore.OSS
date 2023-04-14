using DigRichCore.OSS.Aliyun;
using DigRichCore.OSS.Local;
using DigRichCore.OSS.Providers.Local;
using DigRichCore.OSS.Tencent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigRichCore.OSS {
    public static class OSSServiceExtension {
        public static void AddAliyunOSS(this IServiceCollection services,Action<AliyunOSSConfig> ossConfig) {
            services.Configure<AliyunOSSConfig>(ossConfig);
            services.AddSingleton<IOSSProvider, AliyunOSSProvider>();
        }
        public static void AddLocalOSS(this IServiceCollection services, Action<LocalOSSConfig> ossConfig) {
            services.Configure<LocalOSSConfig>(ossConfig);
            services.AddSingleton<IOSSProvider, LocalOSSProvider>();
        }
        public static void AddTencentOSS(this IServiceCollection services, Action<TencentOSSConfig> ossConfig) {
            services.Configure<TencentOSSConfig>(ossConfig);
            services.AddSingleton<IOSSProvider, TencentOSSProvider>();
        }
        public static void AddAliyunOSS(this IServiceCollection services, IConfigurationSection ossConfig) {
            services.Configure<AliyunOSSConfig>(ossConfig);
            services.AddScoped<IOSSProvider, AliyunOSSProvider>();
        }
        public static void AddLocalOSS(this IServiceCollection services, IConfigurationSection ossConfig) {
            services.Configure<LocalOSSConfig>(ossConfig);
            services.AddScoped<IOSSProvider, LocalOSSProvider>();
        }
        public static void AddTencentOSS(this IServiceCollection services, IConfigurationSection ossConfig) {
            services.Configure<TencentOSSConfig>(ossConfig);
            services.AddScoped<IOSSProvider, TencentOSSProvider>();
        }
    }
}
