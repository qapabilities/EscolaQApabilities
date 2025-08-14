# Script de Teste Simples - Escola QApabilities API
param(
    [Parameter(Mandatory=$true)]
    [string]$BaseUrl = "http://localhost:8080"
)

Write-Host "🧪 Testando API Escola QApabilities" -ForegroundColor Green
Write-Host "🌐 Base URL: $BaseUrl" -ForegroundColor Blue

# 1. Health Check
Write-Host "`n1️⃣ Health Check" -ForegroundColor Cyan
try {
    $healthResponse = Invoke-RestMethod -Uri "$BaseUrl/health" -TimeoutSec 10
    Write-Host "✅ Health Check - OK" -ForegroundColor Green
    Write-Host "   Response: $healthResponse" -ForegroundColor Gray
} catch {
    Write-Host "❌ Health Check - FAILED: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 2. Teste de Login
Write-Host "`n2️⃣ Teste de Autenticação" -ForegroundColor Cyan
try {
    $loginBody = @{
        email = "admin@qapabilities.com"
        password = "admin123"
    } | ConvertTo-Json
    
    $loginResponse = Invoke-RestMethod -Uri "$BaseUrl/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json" -TimeoutSec 10
    $token = $loginResponse.token
    Write-Host "✅ Login Admin - OK" -ForegroundColor Green
    Write-Host "🔑 Token obtido com sucesso" -ForegroundColor Green
} catch {
    Write-Host "❌ Login Admin - FAILED: $($_.Exception.Message)" -ForegroundColor Red
    $token = $null
}

if ($token) {
    # 3. Teste de Estudantes
    Write-Host "`n3️⃣ Teste da API de Estudantes" -ForegroundColor Cyan
    $headers = @{ Authorization = "Bearer $token" }
    
    # Listar estudantes
    try {
        $students = Invoke-RestMethod -Uri "$BaseUrl/api/students" -Headers $headers -TimeoutSec 10
        Write-Host "✅ Listar Estudantes - OK" -ForegroundColor Green
        Write-Host "   Encontrados: $($students.Count) estudantes" -ForegroundColor Gray
    } catch {
        Write-Host "❌ Listar Estudantes - FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    # Criar estudante
    try {
        $newStudentBody = @{
            name = "João Silva Teste"
            email = "joao.teste.$(Get-Date -Format 'yyyyMMddHHmmss')@escola.com"
            birthDate = "2000-01-15T00:00:00Z"
            phone = "+5511999887766"
            address = "Rua das Flores, 123 - Teste"
            status = "Active"
        } | ConvertTo-Json
        
        $newStudent = Invoke-RestMethod -Uri "$BaseUrl/api/students" -Method POST -Body $newStudentBody -ContentType "application/json" -Headers $headers -TimeoutSec 10
        Write-Host "✅ Criar Estudante - OK" -ForegroundColor Green
        Write-Host "👨‍🎓 Estudante criado com ID: $($newStudent.id)" -ForegroundColor Green
        $studentId = $newStudent.id
    } catch {
        Write-Host "❌ Criar Estudante - FAILED: $($_.Exception.Message)" -ForegroundColor Red
        $studentId = $null
    }
    
    # Buscar estudante por ID
    if ($studentId) {
        try {
            $student = Invoke-RestMethod -Uri "$BaseUrl/api/students/$studentId" -Headers $headers -TimeoutSec 10
            Write-Host "✅ Buscar Estudante por ID - OK" -ForegroundColor Green
            Write-Host "   Nome: $($student.name)" -ForegroundColor Gray
        } catch {
            Write-Host "❌ Buscar Estudante por ID - FAILED: $($_.Exception.Message)" -ForegroundColor Red
        }
        
        # Deletar estudante
        try {
            Invoke-RestMethod -Uri "$BaseUrl/api/students/$studentId" -Method DELETE -Headers $headers -TimeoutSec 10
            Write-Host "✅ Deletar Estudante - OK" -ForegroundColor Green
        } catch {
            Write-Host "❌ Deletar Estudante - FAILED: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    # 4. Teste sem autorização
    Write-Host "`n4️⃣ Teste de Autorização" -ForegroundColor Cyan
    try {
        Invoke-RestMethod -Uri "$BaseUrl/api/students" -TimeoutSec 10
        Write-Host "❌ Acesso sem Token - FALHOU (deveria retornar 401)" -ForegroundColor Red
    } catch {
        if ($_.Exception.Response.StatusCode -eq 401) {
            Write-Host "✅ Acesso sem Token - OK (401 Unauthorized como esperado)" -ForegroundColor Green
        } else {
            Write-Host "❌ Acesso sem Token - ERRO inesperado: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

# 5. Teste Swagger
Write-Host "`n5️⃣ Teste Swagger UI" -ForegroundColor Cyan
try {
    $swaggerResponse = Invoke-WebRequest -Uri "$BaseUrl/swagger/index.html" -TimeoutSec 10
    if ($swaggerResponse.StatusCode -eq 200) {
        Write-Host "✅ Swagger UI - OK" -ForegroundColor Green
    }
} catch {
    Write-Host "❌ Swagger UI - FAILED: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎯 Testes concluídos!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 URLs disponíveis:" -ForegroundColor Yellow
Write-Host "   🏥 Health Check: $BaseUrl/health" -ForegroundColor White
Write-Host "   📚 Swagger API: $BaseUrl/swagger" -ForegroundColor White
Write-Host "   🔐 Login Admin: POST $BaseUrl/api/auth/login" -ForegroundColor White
Write-Host "   👨‍🎓 Students API: $BaseUrl/api/students" -ForegroundColor White
Write-Host ""
Write-Host "🔑 Credenciais de teste:" -ForegroundColor Yellow
Write-Host "   Admin: admin@qapabilities.com / admin123" -ForegroundColor White
Write-Host "   Teacher: teacher@qapabilities.com / teacher123" -ForegroundColor White