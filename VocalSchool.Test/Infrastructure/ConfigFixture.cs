using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace VocalSchool.Test.Infrastructure
{
    public class ConfigFixture : IDisposable
    {
        public bool IsLazyLoading;
        
        
        public ConfigFixture()
        {
            var dir = Directory.GetCurrentDirectory();
            var path = Path.GetDirectoryName(dir
                .Substring(0, dir.IndexOf("VocalSchool.Test")));
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{path}/VocalSchool/appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            IConfigurationRoot configuration = builder.Build();
            IsLazyLoading = configuration.GetValue<bool>("LazyLoading");
        }

        public void Dispose()
        {
            IsLazyLoading = false;
        }
    }
}