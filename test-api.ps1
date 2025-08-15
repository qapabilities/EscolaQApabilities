# Script para testar a API
$baseUrl = "http://20.185.104.20"

# 1. Login
Write-Host "🔐 Fazendo login..." -ForegroundColor Yellow
$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body @'
{
  "email": "admin@qapabilities.com",
  "password": "admin123"
}
'@

$token = $loginResponse.token
Write-Host "✅ Token obtido!" -ForegroundColor Green

# 2. Criar estudante
Write-Host "👥 Criando estudante..." -ForegroundColor Yellow
$createStudentResponse = Invoke-RestMethod -Uri "$baseUrl/api/students" -Method POST -ContentType "application/json" -Headers @{Authorization="Bearer $token"} -Body @'
{
  "name": "João Silva Santos",
  "email": "joao.silva@email.com",
  "phone": "(11) 99999-8888",
  "birthDate": "2005-03-15T00:00:00",
  "address": "Rua das Flores, 123, Apto 45",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01234-567",
  "parentName": "Maria Silva Santos",
  "parentPhone": "(11) 88888-7777",
  "parentEmail": "maria.silva@email.com",
  "emergencyContact": "Pedro Santos",
  "emergencyPhone": "(11) 77777-6666",
  "medicalInformation": "Alergia a amendoim",
  "notes": "Estudante dedicado, interessado em matemática"
}
'@

Write-Host "✅ Estudante criado: $($createStudentResponse.name)" -ForegroundColor Green
Write-Host "📧 Email: $($createStudentResponse.email)" -ForegroundColor Cyan
Write-Host "🆔 ID: $($createStudentResponse.id)" -ForegroundColor Cyan

# 3. Listar todos os estudantes
Write-Host "📋 Listando todos os estudantes..." -ForegroundColor Yellow
$allStudents = Invoke-RestMethod -Uri "$baseUrl/api/students/all" -Method GET -Headers @{Authorization="Bearer $token"}

Write-Host "📊 Total de estudantes: $($allStudents.Count)" -ForegroundColor Green
foreach ($student in $allStudents) {
    Write-Host "   - $($student.name) ($($student.email))" -ForegroundColor White
}

Write-Host "🎉 Testes concluídos com sucesso!" -ForegroundColor Green