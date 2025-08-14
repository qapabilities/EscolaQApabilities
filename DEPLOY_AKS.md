# 🚀 Deploy no Azure Kubernetes Service (AKS)

Este guia detalha como fazer o deploy do microsserviço **Escola QApabilities Student Service** no Azure Kubernetes Service.

## 📋 **Pré-requisitos**

### **Ferramentas Necessárias**
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Docker Desktop](https://docs.docker.com/desktop/install/windows-install/)
- [kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl-windows/)
- PowerShell 5.1+

### **Conta Azure**
- Assinatura do Azure ativa
- Permissões para criar recursos
- Azure Container Registry habilitado

## 🎯 **Deploy Automático**

### **Opção 1: Script Completo**
```powershell
# Execute o script de deploy automático
.\deploy-aks.ps1 -ResourceGroupName "escola-qapabilities-rg" -ClusterName "escola-aks" -Location "Brazil South"
```

### **Opção 2: Desenvolvimento Local**
```powershell
# Para testar localmente primeiro
.\run-local.ps1 -Build -Logs
```

## 🔧 **Deploy Manual Passo a Passo**

### **1. Configuração Inicial**

```bash
# Login no Azure
az login

# Criar Resource Group
az group create --name escola-qapabilities-rg --location "Brazil South"

# Criar cluster AKS
az aks create \
  --resource-group escola-qapabilities-rg \
  --name escola-aks \
  --node-count 3 \
  --node-vm-size Standard_B2s \
  --enable-addons monitoring,http_application_routing \
  --enable-managed-identity \
  --generate-ssh-keys \
  --enable-cluster-autoscaler \
  --min-count 1 \
  --max-count 5
```

### **2. Configurar kubectl**

```bash
# Obter credenciais do cluster
az aks get-credentials --resource-group escola-qapabilities-rg --name escola-aks

# Verificar conexão
kubectl get nodes
```

### **3. Instalar Componentes**

```bash
# NGINX Ingress Controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml

# cert-manager para SSL
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.1/cert-manager.yaml

# Aguardar NGINX estar pronto
kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=300s
```

### **4. Container Registry**

```bash
# Criar Azure Container Registry
az acr create --resource-group escola-qapabilities-rg --name escolaaksregistry --sku Basic

# Login no ACR
az acr login --name escolaaksregistry

# Build e push da imagem
docker build -t escolaaksregistry.azurecr.io/student-service:latest .
docker push escolaaksregistry.azurecr.io/student-service:latest

# Anexar ACR ao AKS
az aks update -n escola-aks -g escola-qapabilities-rg --attach-acr escolaaksregistry
```

### **5. Deploy da Aplicação**

```bash
# Aplicar manifestos
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/sql-server.yaml

# Aguardar SQL Server
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=sql-server -n escola-qapabilities --timeout=300s

# Criar secrets (SUBSTITUA OS VALORES!)
kubectl create secret generic student-service-secret \
  --from-literal=STUDENT_DB_CONNECTION_STRING="Server=sql-server-service.escola-qapabilities.svc.cluster.local,1433;Database=EscolaQApabilitiesStudentService;User Id=SA;Password=SUA_SENHA_FORTE;TrustServerCertificate=true;Encrypt=false;" \
  --from-literal=JWT_KEY="sua-chave-jwt-super-secreta-32-caracteres-minimo" \
  -n escola-qapabilities

# Deploy do serviço
kubectl apply -f k8s/student-service.yaml
kubectl apply -f k8s/ingress.yaml
```

## 🔐 **Configuração de Segurança**

### **1. Secrets Importantes**

⚠️ **ATENÇÃO**: Substitua os valores padrão antes do deploy em produção!

```bash
# Senha do SQL Server
SA_PASSWORD: "SuaSenhaForte123!"

# Connection String
STUDENT_DB_CONNECTION_STRING: "Server=sql-server-service.escola-qapabilities.svc.cluster.local,1433;Database=EscolaQApabilitiesStudentService;User Id=SA;Password=SuaSenhaForte123!;TrustServerCertificate=true;Encrypt=false;"

# JWT Key (mínimo 32 caracteres)
JWT_KEY: "sua-chave-jwt-super-secreta-para-producao-32-caracteres-minimo"
```

### **2. Configurar DNS e SSL**

```bash
# Obter IP externo
kubectl get service ingress-nginx-controller -n ingress-nginx

# Configurar DNS A record
# api.escola-qapabilities.com -> IP_EXTERNO

# Configurar cert-manager issuer
kubectl apply -f - <<EOF
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-prod
spec:
  acme:
    server: https://acme-v02.api.letsencrypt.org/directory
    email: admin@escola-qapabilities.com
    privateKeySecretRef:
      name: letsencrypt-prod
    solvers:
    - http01:
        ingress:
          class: nginx
EOF
```

## 📊 **Monitoramento e Troubleshooting**

### **Comandos Úteis**

```bash
# Status dos pods
kubectl get pods -n escola-qapabilities

# Logs da aplicação
kubectl logs -f deployment/student-service -n escola-qapabilities

# Logs do SQL Server
kubectl logs -f deployment/sql-server -n escola-qapabilities

# Status do ingress
kubectl describe ingress student-service-ingress -n escola-qapabilities

# Events do namespace
kubectl get events -n escola-qapabilities --sort-by='.lastTimestamp'

# Conectar no pod para debug
kubectl exec -it deployment/student-service -n escola-qapabilities -- /bin/bash
```

### **Health Checks**

```bash
# Verificar health da API
curl http://IP_EXTERNO/health

# Testar login
curl -X POST http://IP_EXTERNO/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@qapabilities.com", "password": "admin123"}'

# Swagger UI
# Acesse: http://IP_EXTERNO/swagger
```

## 🔄 **Atualizações e CI/CD**

### **Deploy de Nova Versão**

```bash
# Build nova imagem
docker build -t escolaaksregistry.azurecr.io/student-service:v1.1 .
docker push escolaaksregistry.azurecr.io/student-service:v1.1

# Atualizar deployment
kubectl set image deployment/student-service student-service=escolaaksregistry.azurecr.io/student-service:v1.1 -n escola-qapabilities

# Verificar rollout
kubectl rollout status deployment/student-service -n escola-qapabilities
```

### **Rollback**

```bash
# Ver histórico
kubectl rollout history deployment/student-service -n escola-qapabilities

# Fazer rollback
kubectl rollout undo deployment/student-service -n escola-qapabilities
```

## 📈 **Escalabilidade**

### **Auto Scaling**

```yaml
# HPA já configurado no student-service.yaml
# Escala entre 3-10 pods baseado em CPU (70%) e Memória (80%)

# Ver status do HPA
kubectl get hpa -n escola-qapabilities

# Ver métricas
kubectl top pods -n escola-qapabilities
kubectl top nodes
```

## 🗑️ **Limpeza**

```bash
# Remover namespace (remove tudo)
kubectl delete namespace escola-qapabilities

# Remover cluster AKS
az aks delete --resource-group escola-qapabilities-rg --name escola-aks

# Remover resource group completo
az group delete --name escola-qapabilities-rg
```

## 📞 **Suporte**

Para problemas ou dúvidas:
1. Verifique os logs: `kubectl logs -f deployment/student-service -n escola-qapabilities`
2. Verifique events: `kubectl get events -n escola-qapabilities`
3. Teste health check: `curl http://IP_EXTERNO/health`

## 🎯 **URLs Finais**

Após o deploy completo:
- **API Health**: `http://IP_EXTERNO/health`
- **Swagger UI**: `http://IP_EXTERNO/swagger`
- **API Base**: `http://IP_EXTERNO/api`
- **Auth Login**: `POST http://IP_EXTERNO/api/auth/login`
- **Students**: `http://IP_EXTERNO/api/students`

