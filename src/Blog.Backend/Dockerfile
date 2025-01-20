FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 5000

ARG AZURE_KEY_VAULT_URL
ARG AZURE_CLIENT_ID
ARG AZURE_DIRECTORY_ID
ARG AZURE_CLIENT_SECRET
ARG DB_HOST

ENV AzureKeyVault__KeyVaultUrl=$AZURE_KEY_VAULT_URL
ENV AzureKeyVault__ClientId=$AZURE_CLIENT_ID
ENV AzureKeyVault__DirectoryId=$AZURE_DIRECTORY_ID
ENV AzureKeyVault__ClientSecret=$AZURE_CLIENT_SECRET
ENV Database__Host=$AZURE_CLIENT_SECRET


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