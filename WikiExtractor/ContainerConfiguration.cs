using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process;
using WikiExtractor.Repository;

namespace WikiExtractor
{
    internal static class ContainerConfiguration
    {
        public static ServiceProvider Configure()
        {
            return new ServiceCollection()
                //.AddLogging(l => l.AddConsole())
                //.Configure<LoggerFilterOptions>(c => c.MinLevel = LogLevel.Trace)
                .AddSingleton<IWikiDatabase, WikiDatabase>()
                //.AddSingleton<IConsolePrinter, ConsolePrinter>()
                .AddSingleton<WikiAppController>()
                .BuildServiceProvider();
        }
    }
}
