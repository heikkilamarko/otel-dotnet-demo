using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OtelDemo.Worker.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddOtel(this WebApplicationBuilder builder)
    {
        var appName = builder.Configuration.GetAppName();
        var appVersion = builder.Configuration.GetAppVersion();
        var otelOtlpEndpoint = builder.Configuration.GetOtelOtlpEndpoint();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;

            options
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(appName, serviceVersion: appVersion))
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = otelOtlpEndpoint;
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                });
        });

        builder.Services.AddOpenTelemetry()
              .ConfigureResource(resource => resource.AddService(appName, serviceVersion: appVersion))
              .WithTracing(tracing => tracing
                  .AddSource(appName)
                  .AddHttpClientInstrumentation()
                  .AddAspNetCoreInstrumentation()
                  .AddOtlpExporter(options =>
                  {
                      options.Endpoint = otelOtlpEndpoint;
                      options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                  }))
              .WithMetrics(metrics => metrics
                  .AddHttpClientInstrumentation()
                  .AddAspNetCoreInstrumentation()
                  .AddOtlpExporter(options =>
                  {
                      options.Endpoint = otelOtlpEndpoint;
                      options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                  }));

        builder.Services.AddSingleton<OtelInstrumentation>();

        return builder;
    }
}
