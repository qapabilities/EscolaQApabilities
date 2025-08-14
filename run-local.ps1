# Script para rodar localmente com Docker Compose
# Escola QApabilities - Student Service

param(
    [Parameter(Mandatory=$false)]
    [switch]$Build,
    
    [Parameter(Mandatory=$false)]
    [switch]$Down,
    
    [Parameter(Mandatory=$false)]
    [switch]$Logs
)

Write-Host "🐳 Escola QApabilities - Docker Local" -ForegroundColor Green

if ($Down) {
    Write-Host "⬇️ Parando containers..." -ForegroundColor Yellow
    docker-compose down -v
    docker system prune -f
    Write-Host "✅ Containers parados e volumes removidos!" -ForegroundColor Green
    exit 0
}

if ($Build) {
    Write-Host "🔨 Fazendo build das imagens..." -ForegroundColor Blue
    docker-compose build --no-cache
}

Write-Host "🚀 Iniciando aplicação..." -ForegroundColor Blue
docker-compose up -d

if ($Logs) {
    Write-Host "📋 Mostrando logs..." -ForegroundColor Yellow
    docker-compose logs -f
} else {
    Write-Host "⏳ Aguardando aplicação estar pronta..." -ForegroundColor Yellow
    
    $maxAttempts = 30
    $attempt = 0
    
    do {
        Start-Sleep 5
        $attempt++
        Write-Host "Tentativa $attempt/$maxAttempts..." -NoNewline
        
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:8080/health" -TimeoutSec 5 -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
                Write-Host " ✅" -ForegroundColor Green
                break
            }
        } catch {
            Write-Host " ❌" -ForegroundColor Red
        }
    } while ($attempt -lt $maxAttempts)
    
    if ($attempt -ge $maxAttempts) {
        Write-Host "❌ Aplicação não respondeu a tempo. Verificando logs..." -ForegroundColor Red
        docker-compose logs student-service
        exit 1
    }
    
    Write-Host ""
    Write-Host "🎉 Aplicação está rodando!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📋 URLs disponíveis:" -ForegroundColor Yellow
    Write-Host "   🏥 Health Check: http://localhost:8080/health" -ForegroundColor White
    Write-Host "   📚 Swagger API: http://localhost:8080/swagger" -ForegroundColor White
    Write-Host "   🔐 Login Admin: POST http://localhost:8080/api/auth/login" -ForegroundColor White
    Write-Host "   👨‍🎓 Students API: http://localhost:8080/api/students" -ForegroundColor White
    Write-Host ""
    Write-Host "🔑 Credenciais de teste:" -ForegroundColor Yellow
    Write-Host "   Admin: admin@qapabilities.com / admin123" -ForegroundColor White
    Write-Host "   Teacher: teacher@qapabilities.com / teacher123" -ForegroundColor White
    Write-Host ""
    Write-Host "🛠️ Comandos úteis:" -ForegroundColor Yellow
    Write-Host "   docker-compose logs -f                    # Ver logs" -ForegroundColor White
    Write-Host "   docker-compose exec student-service bash  # Conectar no container" -ForegroundColor White
    Write-Host "   docker-compose down -v                    # Parar e remover volumes" -ForegroundColor White
    Write-Host ""
    Write-Host "💾 Banco de dados:" -ForegroundColor Yellow
    Write-Host "   Host: localhost:1433" -ForegroundColor White
    Write-Host "   User: SA" -ForegroundColor White
    Write-Host "   Password: EscolaQApabilities123!" -ForegroundColor White
    Write-Host "   Database: EscolaQApabilitiesStudentService" -ForegroundColor White
}

