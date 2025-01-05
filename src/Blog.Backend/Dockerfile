FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 5000

ARG JWT_KEY
ENV JWT__KEY=$JWT_KEY
ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Blog.Domain/Blog.Domain.csproj", "Blog.Domain/Blog.Domain.csproj"]
COPY ["Blog.Application/Blog.Application.csproj", "Blog.Application/Blog.Application.csproj"]
COPY ["Blog.Persistence/Blog.Persistence.csproj", "Blog.Persistence/Blog.Persistence.csproj"]
COPY ["Blog.API/Blog.API.csproj", "Blog.API/Blog.API.csproj"]
RUN dotnet restore "Blog.API/Blog.API.csproj"
COPY . ./
WORKDIR "/src/Blog.API"
RUN dotnet build "Blog.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Blog.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog.API.dll"]