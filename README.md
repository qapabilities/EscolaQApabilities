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
   - Edite o arquivo `src/EscolaQApabilities.StudentService.API/appsettings.json`
   - Ajuste a connection string conforme seu ambiente

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

## 🧪 Executando os Testes

```bash
# Executa todos os testes
dotnet test

# Executa testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📚 Endpoints da API

### Alunos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/students` | Cria um novo aluno |
| GET | `/api/students/{id}` | Busca aluno por ID |
| GET | `/api/students` | Lista todos os alunos |
| GET | `/api/students/search` | Busca alunos com filtros |
| PUT | `/api/students/{id}` | Atualiza dados pessoais |
| PUT | `/api/students/{id}/contact` | Atualiza informações de contato |
| PUT | `/api/students/{id}/status` | Atualiza status do aluno |
| DELETE | `/api/students/{id}` | Remove um aluno |

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

## 👨‍💻 Autor

Desenvolvido para a Escola QApabilities seguindo as melhores práticas de desenvolvimento de software. 