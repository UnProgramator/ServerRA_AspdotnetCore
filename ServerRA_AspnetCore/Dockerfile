#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ServerRA_AspnetCore/ServerRA_AspnetCore.csproj", "ServerRA_AspnetCore/"]
RUN dotnet restore "ServerRA_AspnetCore/ServerRA_AspnetCore.csproj"
COPY . .
WORKDIR "/src/ServerRA_AspnetCore"
RUN dotnet build "ServerRA_AspnetCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServerRA_AspnetCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerRA_AspnetCore.dll"]