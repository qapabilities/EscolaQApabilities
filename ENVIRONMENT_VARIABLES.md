# Variáveis de Ambiente

Este documento descreve as variáveis de ambiente necessárias para o microsserviço de estudantes.

## Configuração do Banco de Dados

### STUDENT_DB_CONNECTION_STRING
**Descrição**: String de conexão do banco de dados SQL Server  
**Obrigatório**: Sim (em produção) / Não (em desenvolvimento)  
**Formato**: `Server=servidor;Database=banco;Trusted_Connection=True;...`  
**Exemplo**: 
```
# Desenvolvimento
STUDENT_DB_CONNECTION_STRING=Server=localhost\\SQLEXPRESS;Database=EscolaQApabilitiesStudentService;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True

# Produção
STUDENT_DB_CONNECTION_STRING=Server=prod-server;Database=EscolaQApabilitiesStudentService;User Id=appuser;Password=securepassword;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=False
```

**Nota**: Em produção, SEMPRE use variáveis de ambiente. Em desenvolvimento, pode usar appsettings.Development.json.

### ASPNETCORE_ENVIRONMENT
**Descrição**: Ambiente de execução da aplicação  
**Obrigatório**: Não (padrão: Production)  
**Valores**: Development, Staging, Production  
**Exemplo**: 
```
ASPNETCORE_ENVIRONMENT=Development
```

## Configuração para Diferentes Ambientes

### Desenvolvimento Local
```bash
# Windows PowerShell
$env:STUDENT_DB_CONNECTION_STRING="Server=localhost\\SQLEXPRESS;Database=EscolaQApabilitiesStudentService;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True"
$env:ASPNETCORE_ENVIRONMENT="Development"

# Linux/macOS
export STUDENT_DB_CONNECTION_STRING="Server=localhost\\SQLEXPRESS;Database=EscolaQApabilitiesStudentService;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True"
export ASPNETCORE_ENVIRONMENT="Development"
```

### Produção
```bash
# Configure com credenciais seguras e conexão criptografada
export STUDENT_DB_CONNECTION_STRING="Server=prod-server;Database=EscolaQApabilitiesStudentService;User Id=app_user;Password=secure_password;Encrypt=True;TrustServerCertificate=False"
export ASPNETCORE_ENVIRONMENT="Production"
```

## Segurança

⚠️ **Importante**: 
- Nunca commite credenciais reais no controle de versão
- Use variáveis de ambiente ou Azure Key Vault em produção
- Configure conexões criptografadas em ambientes de produção
- Use usuários específicos da aplicação, não sa
