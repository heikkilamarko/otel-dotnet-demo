FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish auto/api1/ -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS otel
ARG OTEL_VERSION=1.11.0
ENV OTEL_DOTNET_AUTO_HOME=/otel-dotnet-auto
ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v${OTEL_VERSION}/otel-dotnet-auto-install.sh otel-dotnet-auto-install.sh
RUN <<EOF
apt-get update
apt-get install -y unzip curl
sh otel-dotnet-auto-install.sh
chmod +x /otel-dotnet-auto/instrument.sh
EOF

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ENV OTEL_DOTNET_AUTO_HOME=/otel-dotnet-auto
WORKDIR /app
COPY --from=build /app .
COPY --from=otel /otel-dotnet-auto /otel-dotnet-auto
EXPOSE 8080
ENTRYPOINT ["/otel-dotnet-auto/instrument.sh", "dotnet", "OtelDemo.Api1.dll"]
