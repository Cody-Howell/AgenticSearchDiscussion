FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore
COPY TemplateAPI/TemplateAPI.csproj ./TemplateAPI/TemplateAPI.csproj
RUN dotnet restore TemplateAPI/TemplateAPI.csproj

# Copy the rest of the repository (including data_repo) into the build context
COPY . .

# Publish the app
RUN dotnet publish TemplateAPI/TemplateAPI.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/out .

# Ensure data_repo is present in the runtime image
COPY --from=build /src/data_repo ./data_repo

ENTRYPOINT ["dotnet", "TemplateAPI.dll"]