FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Cogtive.DevAssignment.Api.csproj", "./"]
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build and publish
RUN dotnet build "Cogtive.DevAssignment.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Cogtive.DevAssignment.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install wait-for-it script
RUN apt-get update && apt-get install -y wget && \
    wget -O /usr/local/bin/wait-for-it.sh https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh && \
    chmod +x /usr/local/bin/wait-for-it.sh

COPY --from=build /app/publish .

# Create startup script
RUN echo '#!/bin/bash\n\
wait-for-it.sh postgres:5432 -- dotnet Cogtive.DevAssignment.Api.dll' > /app/start.sh && \
    chmod +x /app/start.sh

ENTRYPOINT ["/app/start.sh"]
