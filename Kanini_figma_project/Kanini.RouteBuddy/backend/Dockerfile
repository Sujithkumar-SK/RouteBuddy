FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Kanini.RouteBuddy.Api/Kanini.RouteBuddy.Api.csproj", "Kanini.RouteBuddy.Api/"]
COPY ["Kanini.RouteBuddy.Application/Kanini.RouteBuddy.Application.csproj", "Kanini.RouteBuddy.Application/"]
COPY ["Kanini.RouteBuddy.Domain/Kanini.RouteBuddy.Domain.csproj", "Kanini.RouteBuddy.Domain/"]
COPY ["Kanini.RouteBuddy.Data/Kanini.RouteBuddy.Data.csproj", "Kanini.RouteBuddy.Data/"]
COPY ["Kanini.RouteBuddy.Common/Kanini.RouteBuddy.Common.csproj", "Kanini.RouteBuddy.Common/"]
COPY ["Directory.Packages.props", "./"]
RUN dotnet restore "Kanini.RouteBuddy.Api/Kanini.RouteBuddy.Api.csproj"
COPY . .
WORKDIR "/src/Kanini.RouteBuddy.Api"
RUN dotnet build "Kanini.RouteBuddy.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kanini.RouteBuddy.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kanini.RouteBuddy.Api.dll"]
