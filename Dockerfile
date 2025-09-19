#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
#WORKDIR /app
#COPY published/ ./
#EXPOSE 8080
#ENTRYPOINT ["dotnet", "LudoAPI.dll"]

# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ./LudoAPI.csproj ./LudoAPI/
RUN dotnet restore ./LudoAPI/LudoAPI.csproj

# copy everything else and build app
COPY . ./LudoAPI/
WORKDIR /source/LudoAPI
RUN dotnet publish LudoAPI.csproj -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "LudoAPI.dll"]