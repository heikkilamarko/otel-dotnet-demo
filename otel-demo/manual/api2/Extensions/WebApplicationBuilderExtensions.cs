using Messaging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OtelDemo.Api2.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddOtel(this WebApplicationBuilder builder)
    {
        var serviceName = builder.Configuration["OTEL_SERVICE_NAME"];

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;

            options
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddOtlpExporter();
        });

        builder.Services.AddOpenTelemetry()
              .ConfigureResource(resource => resource.AddService(serviceName))
              .WithTracing(tracing => tracing
                  .AddSource("OtelDemo.Api2")
                  .AddMessagingInstrumentation()
                  .AddHttpClientInstrumentation()
                  .AddAspNetCoreInstrumentation()
                  .AddOtlpExporter())
              .WithMetrics(metrics => metrics
                  .AddHttpClientInstrumentation()
                  .AddAspNetCoreInstrumentation()
                  .AddOtlpExporter());

        builder.Services.AddSingleton<AppActivitySource>();

        return builder;
    }
}
