# 🚀 Comandos para Azure Cloud Shell

## 📋 **Passo a Passo no Azure Cloud Shell**

### 1️⃣ **Upload dos Arquivos**

No Azure Cloud Shell, faça upload dos seguintes arquivos:
- `Dockerfile`
- `docker-compose.yml` (referência)
- `deploy-cloudshell.sh`
- `deploy-k8s.yaml`
- `test-aks.sh`
- Todo o diretório `src/` com o código fonte

**Como fazer upload:**
1. Clique no ícone de upload (📁) no Cloud Shell
2. Selecione todos os arquivos necessários
3. Aguarde o upload concluir

### 2️⃣ **Executar Deploy da Infraestrutura**

```bash
# Dar permissão de execução
chmod +x deploy-cloudshell.sh

# Executar deploy da infraestrutura
./deploy-cloudshell.sh
```

### 3️⃣ **Build e Push da Imagem Docker**

```bash
# Fazer build da imagem
docker build -t escolaaksregistry.azurecr.io/student-service:latest .

# Fazer push para o registry
docker push escolaaksregistry.azurecr.io/student-service:latest
```

### 4️⃣ **Deploy da Aplicação**

```bash
# Aplicar manifesto Kubernetes
kubectl apply -f deploy-k8s.yaml

# Verificar status dos pods
kubectl get pods -n escola-qapabilities

# Verificar logs (se necessário)
kubectl logs -f deployment/student-service -n escola-qapabilities
```

### 5️⃣ **Testar a Aplicação**

```bash
# Dar permissão de execução
chmod +x test-aks.sh

# Executar testes
./test-aks.sh
```

## 🔍 **Comandos de Monitoramento**

```bash
# Status geral
kubectl get all -n escola-qapabilities

# IP externo do Load Balancer
kubectl get service ingress-nginx-controller -n ingress-nginx

# Logs da aplicação
kubectl logs -f deployment/student-service -n escola-qapabilities

# Descrição detalhada dos pods
kubectl describe pods -l app=student-service -n escola-qapabilities

# Events do namespace
kubectl get events -n escola-qapabilities --sort-by='.lastTimestamp'

# Status do HPA (autoscaling)
kubectl get hpa -n escola-qapabilities

# Métricas de recursos
kubectl top pods -n escola-qapabilities
kubectl top nodes
```

## 🧪 **Testes Manuais**

```bash
# Obter IP externo
EXTERNAL_IP=$(kubectl get service ingress-nginx-controller -n ingress-nginx -o jsonpath='{.status.loadBalancer.ingress[0].ip}')
echo "IP Externo: $EXTERNAL_IP"

# Testar health check
curl "http://$EXTERNAL_IP/health"

# Testar login
curl -X POST "http://$EXTERNAL_IP/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@qapabilities.com", "password": "admin123"}'

# Salvar token (substitua pelo token retornado)
TOKEN="seu_token_aqui"

# Testar API de estudantes
curl -H "Authorization: Bearer $TOKEN" "http://$EXTERNAL_IP/api/students/all"
```

## 🎯 **URLs Finais**

Após o deploy bem-sucedido:
- **Health Check**: `http://EXTERNAL_IP/health`
- **Swagger UI**: `http://EXTERNAL_IP/swagger`
- **API Login**: `POST http://EXTERNAL_IP/api/auth/login`
- **API Students**: `GET http://EXTERNAL_IP/api/students/all`

## 🔧 **Troubleshooting**

### Se o pod não iniciar:
```bash
kubectl describe pod -l app=student-service -n escola-qapabilities
kubectl logs deployment/student-service -n escola-qapabilities
```

### Se não conseguir acessar externamente:
```bash
kubectl get ingress -n escola-qapabilities
kubectl describe ingress student-service-ingress -n escola-qapabilities
```

### Se a imagem não for encontrada:
```bash
az acr repository list --name escolaaksregistry
az acr repository show-tags --name escolaaksregistry --repository student-service
```

## 🧹 **Limpeza (quando terminar)**

```bash
# Remover apenas a aplicação
kubectl delete namespace escola-qapabilities

# Remover cluster completo (cuidado!)
az aks delete --resource-group escola-qapabilities-rg --name escola-aks

# Remover resource group completo (cuidado!)
az group delete --name escola-qapabilities-rg
```

## 📊 **Recursos Criados**

- **Resource Group**: `escola-qapabilities-rg`
- **AKS Cluster**: `escola-aks` (3 nodes, auto-scaling 1-5)
- **Container Registry**: `escolaaksregistry`
- **Namespace**: `escola-qapabilities`
- **Deployment**: `student-service` (3 replicas, HPA habilitado)
- **Service**: `student-service` (ClusterIP)
- **Ingress**: `student-service-ingress` (NGINX)
- **ConfigMap**: `student-service-config`
- **Secret**: `student-service-secret`

---

**Sucesso!** 🎉 Siga estes passos no Azure Cloud Shell para fazer o deploy completo!

