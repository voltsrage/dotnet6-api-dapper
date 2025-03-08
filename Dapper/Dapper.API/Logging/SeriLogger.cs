using Serilog;

namespace Dapper.API.Logging
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure => (ctx, lc) =>
        {
            var elasticUri = ctx.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            var SeqUrl = ctx.Configuration.GetValue<string>("SeqConfiguration:Uri");
            lc
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Filter.ByExcluding("HostingRequestStartingLog <> ''")
            .WriteTo.Seq(SeqUrl) // Used to log to Seq
            .WriteTo.Console() // Used to log to Console
            .Destructure.ToMaximumCollectionCount(16)
            .Destructure.ToMaximumDepth(1)
            //.WriteTo.Elasticsearch(
            //    new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(ctx.Configuration["ElasticConfiguration:Uri"]))
            //    {
            //        IndexFormat = $"applogs-{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}--{ctx.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
            //        AutoRegisterTemplate = true,
            //        NumberOfShards = 2,
            //        NumberOfReplicas = 1,
            //    })
            .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
            .Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
            .ReadFrom.Configuration(ctx.Configuration);
        };

    }
}
