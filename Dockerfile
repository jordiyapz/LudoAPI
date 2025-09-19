FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY published/ ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "LudoAPI.dll"]