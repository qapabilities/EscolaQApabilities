# Teste Simples da Aplicação Escola QApabilities
Write-Host "🧪 Teste Simples - Escola QApabilities" -ForegroundColor Green

# 1. Verificar se a aplicação está rodando
Write-Host "`n1️⃣ Testando conexão..." -ForegroundColor Cyan

try {
    $health = Invoke-RestMethod -Uri "http://localhost:8080/health" -TimeoutSec 5
    Write-Host "✅ API está respondendo: $health" -ForegroundColor Green
} catch {
    Write-Host "❌ API não está respondendo: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "📋 Para iniciar a aplicação:" -ForegroundColor Yellow
    Write-Host "   docker-compose up -d" -ForegroundColor White
    Write-Host ""
    Write-Host "📋 Para verificar logs:" -ForegroundColor Yellow  
    Write-Host "   docker-compose logs student-service" -ForegroundColor White
    exit 1
}

# 2. Testar Swagger
Write-Host "`n2️⃣ Testando Swagger UI..." -ForegroundColor Cyan
try {
    $swagger = Invoke-WebRequest -Uri "http://localhost:8080/swagger/index.html" -TimeoutSec 5
    if ($swagger.StatusCode -eq 200) {
        Write-Host "✅ Swagger UI está disponível" -ForegroundColor Green
    }
} catch {
    Write-Host "⚠️ Swagger UI não está acessível" -ForegroundColor Yellow
}

# 3. Testar endpoint sem autenticação (deve retornar 401)
Write-Host "`n3️⃣ Testando segurança..." -ForegroundColor Cyan
try {
    $students = Invoke-RestMethod -Uri "http://localhost:8080/api/students" -TimeoutSec 5
    Write-Host "❌ API deveria retornar 401 Unauthorized" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode -eq 401) {
        Write-Host "✅ API corretamente protegida (401 Unauthorized)" -ForegroundColor Green
    } else {
        Write-Host "⚠️ Erro inesperado: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "🎯 Teste Básico Concluído!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 URLs disponíveis:" -ForegroundColor Yellow
Write-Host "   🏥 Health Check: http://localhost:8080/health" -ForegroundColor White
Write-Host "   📚 Swagger API: http://localhost:8080/swagger" -ForegroundColor White
Write-Host "   🔐 API Base: http://localhost:8080/api" -ForegroundColor White
Write-Host ""
Write-Host "💡 Próximos passos:" -ForegroundColor Yellow
Write-Host "   1. Configurar banco de dados (SQL Server ou SQLite)" -ForegroundColor White
Write-Host "   2. Testar autenticação e endpoints" -ForegroundColor White
Write-Host "   3. Deploy no Azure Kubernetes Service (AKS)" -ForegroundColor White
Write-Host ""
Write-Host "🛠️ Comandos úteis:" -ForegroundColor Yellow
Write-Host "   docker-compose logs student-service    # Ver logs" -ForegroundColor White
Write-Host "   docker-compose ps                      # Status dos containers" -ForegroundColor White
Write-Host "   docker-compose down                    # Parar aplicação" -ForegroundColor White

