# 🏗️ Arquitetura C4 - Escola QApabilities

Este documento apresenta a arquitetura da solução usando o modelo C4 (Context, Container, Component, Code), fornecendo diferentes níveis de abstração para compreender o sistema.

## 📋 **Modelo C4 - Visão Geral**

O modelo C4 divide a arquitetura em 4 níveis:

1. **Context** - Visão de alto nível do sistema e seus usuários
2. **Container** - Aplicações e serviços que compõem o sistema  
3. **Component** - Componentes dentro de cada container
4. **Code** - Classes e interfaces (representado através da Clean Architecture)

---

## 🌍 **Nível 1: Context Diagram**

**Mostra o sistema em seu contexto, incluindo usuários e sistemas externos.**

### **Atores:**
- 👤 **Administrador**: Gerencia usuários e configurações do sistema
- 👨‍🏫 **Professor**: Gerencia informações dos alunos
- 👨‍💻 **Desenvolvedor**: Responsável por deploy e manutenção

### **Sistemas Externos:**
- ☁️ **Azure Cloud**: Plataforma de infraestrutura e hosting
- 🐙 **GitHub**: Repositório de código e pipeline CI/CD

---

## 📦 **Nível 2: Container Diagram** 

**Mostra as aplicações e serviços de alto nível que compõem o sistema.**

### **Containers Principais:**

#### **🌐 Frontend/Client**
- **Web Browser**: Acesso via Swagger UI para teste e documentação
- **API Clients**: Aplicações externas que consomem a API

#### **☁️ Azure Kubernetes Service (AKS)**
- **Load Balancer**: Distribuição de tráfego (Azure Load Balancer)
- **Ingress Controller**: Roteamento HTTP/HTTPS (NGINX)
- **API Pods**: Múltiplas instâncias do Student Service (.NET 8)
- **Database**: Armazenamento SQLite em volumes persistentes
- **Configuration**: ConfigMaps e Secrets para configuração

#### **🔄 CI/CD Pipeline**
- **GitHub Actions CI**: Build, testes e validação de qualidade
- **GitHub Actions CD**: Deploy automático para AKS
- **Azure Container Registry**: Repositório de imagens Docker

---

## 🧩 **Nível 3: Component Diagram**

**Mostra os componentes dentro do Student Service API seguindo Clean Architecture.**

### **🌐 API Layer**
- **AuthController**: Autenticação JWT (`/api/auth/login`, `/api/auth/me`)
- **StudentsController**: CRUD de estudantes (`/api/students/*`)
- **Health Check**: Monitoring endpoint (`/health`)
- **Swagger UI**: Documentação interativa da API

### **🧠 Application Layer**
- **MediatR**: Command/Query bus (CQRS pattern)
- **Commands**: AuthenticateUser, CreateStudent, UpdateStudent, DeleteStudent
- **Queries**: GetStudent, GetStudents, SearchStudents
- **Handlers**: Processamento de comandos e queries
- **Services**: JWT Service, Password Hasher

### **🏛️ Domain Layer**
- **Entities**: User (email, role, password), Student (informações pessoais)
- **Repository Interfaces**: IUserRepository, IStudentRepository
- **Domain Logic**: Validações de negócio, regras de domínio

### **🏗️ Infrastructure Layer**
- **Entity Framework Context**: StudentDbContext
- **Repository Implementations**: UserRepository, StudentRepository  
- **Database**: SQLite com tabelas Users e Students

### **⚙️ Cross-Cutting Concerns**
- **Configuration**: JWT, CORS, Rate Limiting
- **Logging**: Structured logging para observabilidade
- **Middleware**: Authentication, Authorization, Error Handling

---

## 🔄 **Pipeline Architecture**

**Fluxo completo de CI/CD automatizado:**

### **🔧 Development Flow**
1. **Developer** → Code Changes → Pull Request
2. **CI Pipeline** → Build → Tests → Quality → Security
3. **Merge to Main** → Triggers CD Pipeline

### **🚀 Deployment Flow**
1. **Docker Build** → Push to Azure Container Registry
2. **Kubernetes Deploy** → Update AKS cluster
3. **Health Checks** → Verify deployment success
4. **Auto Rollback** → Se health checks falharem

---

## 🏛️ **Padrões Arquiteturais Utilizados**

### **Clean Architecture** 🎯
- **Separação de responsabilidades** entre camadas
- **Inversão de dependência** (interfaces no Domain)
- **Testabilidade** alta com baixo acoplamento

### **CQRS (Command Query Responsibility Segregation)** 📋
- **Commands** para operações de escrita
- **Queries** para operações de leitura
- **MediatR** como mediador entre controllers e handlers

### **Repository Pattern** 🗄️
- **Abstração** do acesso a dados
- **Interfaces** no Domain, implementação na Infrastructure
- **Testabilidade** com mocks

### **Microservices** 🔄
- **Single Responsibility** - Foco em gestão de estudantes
- **Independently Deployable** - Pipeline própria
- **Technology Agnostic** - Comunicação via HTTP/REST

---

## 🔒 **Aspectos de Segurança**

### **Authentication & Authorization** 🔐
- **JWT Tokens** com roles (Admin/Teacher)
- **Password Hashing** seguro
- **Rate Limiting** para prevenir abuso

### **Infrastructure Security** 🛡️
- **Azure Managed Identity** para AKS
- **Container Registry** privado
- **Network Security Groups** no Azure
- **HTTPS** via Ingress Controller

### **Pipeline Security** 🔍
- **Security Scanning** com Trivy
- **Secret Management** via GitHub Secrets
- **Vulnerability Assessment** automático

---

## 📊 **Observabilidade**

### **Monitoring** 👀
- **Health Checks** nativos do Kubernetes
- **Azure Monitor** integrado ao AKS
- **Structured Logging** para troubleshooting

### **CI/CD Visibility** 📈
- **GitHub Actions** logs detalhados
- **Build Status** badges no README
- **Deployment Notifications** automáticas

---

## 🎯 **Benefícios da Arquitetura**

### **Escalabilidade** 📈
- **Horizontal scaling** via Kubernetes HPA
- **Load balancing** automático
- **Resource management** otimizado

### **Maintainability** 🔧
- **Clean Architecture** facilita mudanças
- **Automated Testing** garante qualidade
- **CI/CD** reduz erro humano

### **Reliability** 🛡️
- **Health checks** automáticos
- **Auto-rollback** em falhas
- **Multiple replicas** para alta disponibilidade

### **Developer Experience** 👨‍💻
- **Local development** com Docker
- **Automated deployments** via pipeline
- **Comprehensive documentation** (Swagger, README, diagramas)

---

## 🚀 **Próximos Passos de Evolução**

1. **Monitoring Avançado**: Prometheus + Grafana
2. **Service Mesh**: Istio para comunicação entre serviços
3. **Event Sourcing**: Para auditoria completa
4. **API Gateway**: Azure API Management
5. **Distributed Tracing**: Para observabilidade completa

**Esta arquitetura fornece uma base sólida, escalável e mantível para o sistema de gestão de estudantes da Escola QApabilities! 🎓**
