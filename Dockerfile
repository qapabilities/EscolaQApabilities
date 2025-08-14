# Use a imagem oficial do .NET 8 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar arquivos de projeto e restaurar dependências
COPY src/EscolaQApabilities.StudentService.Domain/*.csproj ./src/EscolaQApabilities.StudentService.Domain/
COPY src/EscolaQApabilities.StudentService.Application/*.csproj ./src/EscolaQApabilities.StudentService.Application/
COPY src/EscolaQApabilities.StudentService.Infrastructure/*.csproj ./src/EscolaQApabilities.StudentService.Infrastructure/
COPY src/EscolaQApabilities.StudentService.API/*.csproj ./src/EscolaQApabilities.StudentService.API/

# Restaurar dependências apenas dos projetos de produção
RUN dotnet restore src/EscolaQApabilities.StudentService.API/EscolaQApabilities.StudentService.API.csproj

# Copiar todo o código fonte
COPY . ./

# Build da aplicação
RUN dotnet publish src/EscolaQApabilities.StudentService.API -c Release -o out

# Usar imagem runtime otimizada
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Instalar ferramentas necessárias para troubleshooting (opcional)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copiar artefatos do build
COPY --from=build-env /app/out .

# Criar usuário não-root por segurança
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Configurar variáveis de ambiente
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Expor porta
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Comando de inicialização
ENTRYPOINT ["dotnet", "EscolaQApabilities.StudentService.API.dll"]
