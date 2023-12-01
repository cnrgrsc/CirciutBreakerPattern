# Kullanýlacak base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# SDK ile build aþamasý
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Project.Circiut.csproj", "./Project.Circiut/"]
RUN dotnet restore "Project.Circiut/Project.Circiut.csproj"
COPY . "Project.Circiut/"
WORKDIR "/src/Project.Circiut"
RUN dotnet build "Project.Circiut.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish aþamasý
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Project.Circiut.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.Circiut.dll"]

