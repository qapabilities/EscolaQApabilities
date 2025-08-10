# Escola QApabilities - Microsserviço de Alunos

Este é um microsserviço para o cadastro e gerenciamento de alunos da Escola QApabilities, desenvolvido seguindo as melhores práticas de Clean Architecture, Clean Code e SOLID principles.

## 🏗️ Arquitetura

O projeto segue a Clean Architecture com as seguintes camadas:

- **Domain**: Entidades, enums, exceções e interfaces de repositório
- **Application**: Casos de uso, DTOs, comandos e queries (CQRS)
- **Infrastructure**: Implementação de repositórios, contexto do EF Core
- **API**: Controllers, configuração da aplicação e endpoints REST

## 🚀 Tecnologias Utilizadas

- **.NET 8.0**
- **Entity Framework Core 8.0**
- **SQL Server**
- **MediatR** (CQRS)
- **Swagger/OpenAPI**
- **JWT Authentication**
- **xUnit** (Testes)
- **FluentAssertions** (Testes)
- **Moq** (Testes)

## 📋 Funcionalidades

### Gestão de Alunos
- ✅ Cadastro de novos alunos
- ✅ Consulta de alunos por ID
- ✅ Listagem de todos os alunos
- ✅ Busca de alunos com filtros e paginação
- ✅ Atualização de dados pessoais
- ✅ Atualização de informações de contato
- ✅ Gerenciamento de status (Ativo, Inativo, Suspenso)
- ✅ Remoção de alunos

### Segurança
- ✅ Autenticação JWT
- ✅ Autorização baseada em roles (Admin/Teacher)
- ✅ CORS restritivo
- ✅ Rate limiting
- ✅ Headers de segurança
- ✅ Swagger protegido

### Validações
- ✅ Validação de email único
- ✅ Validação de dados obrigatórios
- ✅ Validação de formato de email
- ✅ Validação de data de nascimento
- ✅ Validação de tamanhos de campos

## 🛠️ Como Executar

### Pré-requisitos
- .NET 8.0 SDK
- SQL Server (LocalDB ou SQL Server Express)
- Visual Studio 2022 ou VS Code

### Passos para Execução

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd EscolaQApabilities
   ```

2. **Restaura as dependências**
   ```bash
   dotnet restore
   ```

3. **Configura a string de conexão**
   
   **Opção 1 - Usando appsettings.json:**
   - Edite o arquivo `src/EscolaQApabilities.StudentService.API/appsettings.json`
   - Ajuste a connection string conforme seu ambiente
   
   **Opção 2 - Usando variáveis de ambiente (Recomendado para produção):**
   ```bash
   # Windows PowerShell
   $env:STUDENT_DB_CONNECTION_STRING="Server=localhost\\SQLEXPRESS;Database=EscolaQApabilitiesStudentService;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True"
   
   # Linux/macOS
   export STUDENT_DB_CONNECTION_STRING="Server=localhost\\SQLEXPRESS;Database=EscolaQApabilitiesStudentService;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True"
   ```
   
   📖 **Veja mais detalhes em**: [ENVIRONMENT_VARIABLES.md](ENVIRONMENT_VARIABLES.md)

4. **Executa as migrações do banco de dados**
   ```bash
   cd src/EscolaQApabilities.StudentService.API
   dotnet ef database update
   ```

5. **Executa a aplicação**
   ```bash
   dotnet run
   ```

6. **Acessa a documentação da API**
   - Abra o navegador e acesse: `https://localhost:7001/swagger`

## 🔐 Autenticação

O microsserviço possui autenticação JWT implementada. Para acessar os endpoints protegidos:

### Credenciais de Teste
- **Admin**: `admin@qapabilities.com` / `admin123`
- **Teacher**: `teacher@qapabilities.com` / `teacher123`

### Como usar
1. Faça login via `POST /api/auth/login`
2. Use o token retornado no header `Authorization: Bearer <token>`
3. No Swagger, clique em "Authorize" e insira o token

### Endpoints Protegidos
- **Admin Only**: `GET /api/students`, `POST /api/students`, `DELETE /api/students/{id}`
- **Teacher/Admin**: `GET /api/students/{id}`, `PUT /api/students/{id}`, `GET /api/students/search`

## 🧪 Executando os Testes

```bash
# Executa todos os testes
dotnet test

# Executa testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📚 Endpoints da API

### Autenticação

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| POST | `/api/auth/login` | Login de usuário | Não |
| GET | `/api/auth/me` | Informações do usuário atual | Sim |

### Alunos

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| POST | `/api/students` | Cria um novo aluno | Admin |
| GET | `/api/students/{id}` | Busca aluno por ID | Teacher/Admin |
| GET | `/api/students` | Lista todos os alunos | Admin |
| GET | `/api/students/search` | Busca alunos com filtros | Teacher/Admin |
| PUT | `/api/students/{id}` | Atualiza dados pessoais | Teacher/Admin |
| PUT | `/api/students/{id}/contact` | Atualiza informações de contato | Teacher/Admin |
| PUT | `/api/students/{id}/status` | Atualiza status do aluno | Teacher/Admin |
| DELETE | `/api/students/{id}` | Remove um aluno | Admin |

### Exemplo de Criação de Aluno

```json
POST /api/students
{
  "name": "João Silva",
  "email": "joao.silva@email.com",
  "phone": "11987654321",
  "birthDate": "2000-01-01T00:00:00Z",
  "address": "Rua das Flores, 123",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01234567",
  "parentName": "Maria Silva",
  "parentPhone": "11987654322",
  "parentEmail": "maria.silva@email.com",
  "emergencyContact": "José Silva",
  "emergencyPhone": "11987654323",
  "medicalInformation": "Alergia a penicilina",
  "notes": "Aluno dedicado e participativo"
}
```

## 🏛️ Princípios SOLID Aplicados

### Single Responsibility Principle (SRP)
- Cada classe tem uma única responsabilidade
- Entidades focam apenas nas regras de negócio
- Repositórios focam apenas no acesso a dados

### Open/Closed Principle (OCP)
- Sistema aberto para extensão, fechado para modificação
- Novos comportamentos podem ser adicionados sem modificar código existente

### Liskov Substitution Principle (LSP)
- Implementações de repositório podem ser substituídas sem afetar o comportamento

### Interface Segregation Principle (ISP)
- Interfaces específicas para cada necessidade
- `IStudentRepository` contém apenas métodos necessários

### Dependency Inversion Principle (DIP)
- Dependências de alto nível não dependem de baixo nível
- Uso de interfaces para inversão de dependência

## 🧹 Clean Code

- Nomes descritivos e significativos
- Funções pequenas e com responsabilidade única
- Comentários apenas quando necessário
- Código auto-documentado
- Tratamento adequado de exceções

## 🏗️ Clean Architecture

### Camadas
1. **Domain**: Regras de negócio centrais
2. **Application**: Casos de uso e orquestração
3. **Infrastructure**: Implementações técnicas
4. **API**: Interface de comunicação

### Dependências
- As dependências apontam sempre para dentro
- Domain não depende de nenhuma outra camada
- Infrastructure implementa interfaces do Domain

## 📝 Estrutura do Projeto

```
EscolaQApabilities/
├── src/
│   ├── EscolaQApabilities.StudentService.API/           # Camada de apresentação
│   ├── EscolaQApabilities.StudentService.Application/   # Camada de aplicação
│   ├── EscolaQApabilities.StudentService.Domain/        # Camada de domínio
│   └── EscolaQApabilities.StudentService.Infrastructure/# Camada de infraestrutura
├── tests/
│   └── EscolaQApabilities.StudentService.Tests/         # Testes unitários
├── EscolaQApabilities.StudentService.sln               # Solução do Visual Studio
└── README.md                                           # Este arquivo
```

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🔒 Segurança

Para mais detalhes sobre a implementação de segurança, consulte:
- [SECURITY_IMPLEMENTATION.md](SECURITY_IMPLEMENTATION.md) - Documentação completa da implementação de segurança
- [ENVIRONMENT_VARIABLES.md](ENVIRONMENT_VARIABLES.md) - Configuração de variáveis de ambiente

## 👨‍💻 Autor

Desenvolvido para a Escola QApabilities seguindo as melhores práticas de desenvolvimento de software. 