FROM mcr.microsoft.com/dotnet/sdk:9.0-noble AS build
WORKDIR /src
COPY . .
RUN dotnet publish manual/api2/ -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled-extra AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "OtelDemo.Api2.dll"]
