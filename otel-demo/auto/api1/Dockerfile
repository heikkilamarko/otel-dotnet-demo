FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish auto/api1/ -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ARG OTEL_VERSION=1.11.0
ENV OTEL_DOTNET_AUTO_HOME=/otel-dotnet-auto
ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v${OTEL_VERSION}/otel-dotnet-auto-install.sh otel-dotnet-auto-install.sh
RUN apt-get update && apt-get install -y unzip curl && \
    sh otel-dotnet-auto-install.sh
RUN chmod +x /otel-dotnet-auto/instrument.sh
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["/otel-dotnet-auto/instrument.sh", "dotnet", "OtelDemo.Api1.dll"]
