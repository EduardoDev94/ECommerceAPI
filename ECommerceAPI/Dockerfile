# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER root

# Utilitários base
RUN apt-get update && apt-get install -y --no-install-recommends curl ca-certificates \
    && rm -rf /var/lib/apt/lists/*

# New Relic .NET agent — instalação via .deb direto (evita apt-key deprecated/DSA1024)
ARG NR_AGENT_VERSION=10.50.0
RUN curl -fsSL "https://download.newrelic.com/dot_net_agent/latest_release/newrelic-dotnet-agent_${NR_AGENT_VERSION}_amd64.deb" \
    -o /tmp/newrelic-agent.deb \
    && dpkg -i /tmp/newrelic-agent.deb \
    && rm /tmp/newrelic-agent.deb

# Habilita o profiler do agente (chave injetada em runtime pelo docker-compose)
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
    NEW_RELIC_APP_NAME="ECommerceAPI"

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar todos os arquivos .csproj
COPY ["ECommerceAPI/ECommerceAPI.csproj", "ECommerceAPI/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["CrossCutting/CrossCutting.csproj", "CrossCutting/"]

# Restaurar dependências
RUN dotnet restore "ECommerceAPI/ECommerceAPI.csproj"

# Copiar código-fonte
COPY . .

# Build
WORKDIR "/src/ECommerceAPI"
RUN dotnet build "ECommerceAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ECommerceAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerceAPI.dll"]

