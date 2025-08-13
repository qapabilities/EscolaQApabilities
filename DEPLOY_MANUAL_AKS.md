# 🚀 Deploy Manual no Azure Kubernetes Service (AKS)

Como o Azure CLI ainda não está acessível no terminal atual, aqui está um guia completo para deploy manual no AKS.

## 📋 **Opção 1: Azure Cloud Shell (Recomendado)**

1. **Acesse o Azure Portal**: https://portal.azure.com
2. **Abra o Cloud Shell** (ícone >_ no topo)
3. **Clone o repositório** (se estiver no GitHub) ou **upload dos arquivos**
4. **Execute os comandos abaixo**

## 🔧 **Comandos para Azure Cloud Shell**

```bash
# 1. Definir variáveis
RESOURCE_GROUP="escola-qapabilities-rg"
CLUSTER_NAME="escola-aks"
LOCATION="Brazil South"
REGISTRY_NAME="escolaaksregistry"

# 2. Criar Resource Group
az group create --name $RESOURCE_GROUP --location "$LOCATION"

# 3. Criar Azure Container Registry
az acr create --resource-group $RESOURCE_GROUP --name $REGISTRY_NAME --sku Basic

# 4. Criar cluster AKS
az aks create \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --node-count 3 \
  --node-vm-size Standard_B2s \
  --enable-addons monitoring,http_application_routing \
  --enable-managed-identity \
  --generate-ssh-keys \
  --enable-cluster-autoscaler \
  --min-count 1 \
  --max-count 5

# 5. Conectar ACR ao AKS
az aks update -n $CLUSTER_NAME -g $RESOURCE_GROUP --attach-acr $REGISTRY_NAME

# 6. Obter credenciais do kubectl
az aks get-credentials --resource-group $RESOURCE_GROUP --name $CLUSTER_NAME

# 7. Instalar NGINX Ingress Controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml

# 8. Aguardar NGINX estar pronto
kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=300s
```

## 📦 **Preparar a Imagem Docker**

Se você não pode fazer build no Cloud Shell, pode usar Docker Hub:

```bash
# No seu computador (com Docker funcionando)
docker build -t seunome/escola-student-service:latest .
docker push seunome/escola-student-service:latest
```

Ou use a imagem que já buildamos localmente e faça push para Azure Container Registry:

```bash
# Login no ACR (no Cloud Shell)
az acr login --name $REGISTRY_NAME

# Build e push (se tiver os arquivos no Cloud Shell)
docker build -t $REGISTRY_NAME.azurecr.io/student-service:latest .
docker push $REGISTRY_NAME.azurecr.io/student-service:latest
```

## 🔧 **Manifesto Kubernetes Simplificado**

Crie um arquivo `deploy-simple.yaml`:

```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: escola-qapabilities
---
apiVersion: v1
kind: ConfigMap
metadata:
  name: student-service-config
  namespace: escola-qapabilities
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  USE_SQLITE: "true"
  JWT_ISSUER: "EscolaQApabilities"
  JWT_AUDIENCE: "EscolaQApabilitiesAPI"
  JWT_EXPIRY_MINUTES: "60"
---
apiVersion: v1
kind: Secret
metadata:
  name: student-service-secret
  namespace: escola-qapabilities
type: Opaque
stringData:
  STUDENT_DB_CONNECTION_STRING: "Data Source=/tmp/escola.db"
  JWT_KEY: "escola-qapabilities-super-secret-jwt-key-for-production-32-chars-minimum-key"
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: student-service
  namespace: escola-qapabilities
  labels:
    app: student-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: student-service
  template:
    metadata:
      labels:
        app: student-service
    spec:
      containers:
      - name: student-service
        image: escolaaksregistry.azurecr.io/student-service:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          valueFrom:
            configMapKeyRef:
              name: student-service-config
              key: ASPNETCORE_ENVIRONMENT
        - name: USE_SQLITE
          valueFrom:
            configMapKeyRef:
              name: student-service-config
              key: USE_SQLITE
        - name: STUDENT_DB_CONNECTION_STRING
          valueFrom:
            secretKeyRef:
              name: student-service-secret
              key: STUDENT_DB_CONNECTION_STRING
        - name: JWT_KEY
          valueFrom:
            secretKeyRef:
              name: student-service-secret
              key: JWT_KEY
        - name: JWT_ISSUER
          valueFrom:
            configMapKeyRef:
              name: student-service-config
              key: JWT_ISSUER
        - name: JWT_AUDIENCE
          valueFrom:
            configMapKeyRef:
              name: student-service-config
              key: JWT_AUDIENCE
        - name: JWT_EXPIRY_MINUTES
          valueFrom:
            configMapKeyRef:
              name: student-service-config
              key: JWT_EXPIRY_MINUTES
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: student-service
  namespace: escola-qapabilities
spec:
  selector:
    app: student-service
  ports:
  - port: 80
    targetPort: 8080
  type: ClusterIP
---
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: student-service-ingress
  namespace: escola-qapabilities
  annotations:
    kubernetes.io/ingress.class: nginx
    nginx.ingress.kubernetes.io/rewrite-target: /
spec:
  rules:
  - http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: student-service
            port:
              number: 80
```

## 🚀 **Deploy no Kubernetes**

```bash
# Aplicar manifesto
kubectl apply -f deploy-simple.yaml

# Verificar pods
kubectl get pods -n escola-qapabilities

# Verificar serviços
kubectl get services -n escola-qapabilities

# Verificar ingress
kubectl get ingress -n escola-qapabilities

# Obter IP externo
kubectl get service ingress-nginx-controller -n ingress-nginx
```

## 🧪 **Testar a Aplicação**

```bash
# Obter IP externo
EXTERNAL_IP=$(kubectl get service ingress-nginx-controller -n ingress-nginx -o jsonpath='{.status.loadBalancer.ingress[0].ip}')

# Testar health check
curl http://$EXTERNAL_IP/health

# Testar login
curl -X POST http://$EXTERNAL_IP/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@qapabilities.com", "password": "admin123"}'
```

## 📊 **Monitoramento**

```bash
# Logs da aplicação
kubectl logs -f deployment/student-service -n escola-qapabilities

# Status detalhado
kubectl describe pod -l app=student-service -n escola-qapabilities

# Events
kubectl get events -n escola-qapabilities --sort-by='.lastTimestamp'
```

## 🔧 **Troubleshooting**

Se houver problemas:

1. **Verificar se a imagem existe**:
   ```bash
   az acr repository list --name $REGISTRY_NAME
   ```

2. **Verificar logs detalhados**:
   ```bash
   kubectl logs deployment/student-service -n escola-qapabilities --previous
   ```

3. **Verificar configuração do ingress**:
   ```bash
   kubectl describe ingress student-service-ingress -n escola-qapabilities
   ```

## 🧹 **Limpeza (quando terminar os testes)**

```bash
# Remover namespace (remove tudo da aplicação)
kubectl delete namespace escola-qapabilities

# Remover cluster (se quiser)
az aks delete --resource-group $RESOURCE_GROUP --name $CLUSTER_NAME

# Remover resource group completo (se quiser)
az group delete --name $RESOURCE_GROUP
```

---

Este guia permite fazer o deploy mesmo sem ter o Azure CLI funcionando localmente!

