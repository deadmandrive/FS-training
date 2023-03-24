#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["dotnetcore5kafka/dotnetcore5kafka.csproj", "dotnetcore5kafka/"]
RUN dotnet restore "dotnetcore5kafka/dotnetcore5kafka.csproj"
COPY . .
WORKDIR "/src/dotnetcore5kafka"
RUN dotnet build "dotnetcore5kafka.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotnetcore5kafka.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnetcore5kafka.dll"]