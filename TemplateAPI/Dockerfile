FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore
COPY TemplateAPI/TemplateAPI.csproj ./TemplateAPI/TemplateAPI.csproj
RUN dotnet restore TemplateAPI/TemplateAPI.csproj

# Copy only the API project files into the build context (avoid copying host data_repo)
# This keeps build artifacts small and prevents baking local `data_repo` into the image.
COPY TemplateAPI/. ./TemplateAPI/

# Publish the app
RUN dotnet publish TemplateAPI/TemplateAPI.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/out .

# Expect `data_repo` to be mounted at runtime rather than baked into the image.
# Declare the mount point so Docker users know where to attach the host folder.
VOLUME ["/data_repo"]

ENTRYPOINT ["dotnet", "TemplateAPI.dll"]