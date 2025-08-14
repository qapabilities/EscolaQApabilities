#!/bin/bash
# Script de Deploy para Azure Cloud Shell
# Escola QApabilities - Student Service

set -e

echo "🚀 Iniciando deploy no Azure Kubernetes Service..."

# Definir variáveis
RESOURCE_GROUP="escola-qapabilities-rg"
CLUSTER_NAME="escola-aks"
LOCATION="Brazil South"
REGISTRY_NAME="escolaaksregistry"

echo "📋 Configurações:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Cluster: $CLUSTER_NAME"
echo "  Location: $LOCATION"
echo "  Registry: $REGISTRY_NAME"

# 1. Criar Resource Group
echo "1️⃣ Criando Resource Group..."
az group create --name $RESOURCE_GROUP --location "$LOCATION"

# 2. Criar Azure Container Registry
echo "2️⃣ Criando Azure Container Registry..."
az acr create --resource-group $RESOURCE_GROUP --name $REGISTRY_NAME --sku Basic

# 3. Login no ACR
echo "3️⃣ Fazendo login no ACR..."
az acr login --name $REGISTRY_NAME

# 4. Criar cluster AKS
echo "4️⃣ Criando cluster AKS (isso pode demorar alguns minutos)..."
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
  --max-count 5 \
  --kubernetes-version 1.28

# 5. Conectar ACR ao AKS
echo "5️⃣ Conectando ACR ao AKS..."
az aks update -n $CLUSTER_NAME -g $RESOURCE_GROUP --attach-acr $REGISTRY_NAME

# 6. Obter credenciais do kubectl
echo "6️⃣ Configurando kubectl..."
az aks get-credentials --resource-group $RESOURCE_GROUP --name $CLUSTER_NAME --overwrite-existing

# 7. Instalar NGINX Ingress Controller
echo "7️⃣ Instalando NGINX Ingress Controller..."
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml

# 8. Aguardar NGINX estar pronto
echo "8️⃣ Aguardando NGINX Ingress Controller..."
kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=300s

echo "✅ Infraestrutura criada com sucesso!"
echo ""
echo "📋 Próximos passos:"
echo "1. Fazer build e push da imagem Docker"
echo "2. Aplicar manifesto Kubernetes: kubectl apply -f deploy-k8s.yaml"
echo "3. Verificar status: kubectl get pods -n escola-qapabilities"
echo ""
echo "🔍 Para obter o IP externo:"
echo "kubectl get service ingress-nginx-controller -n ingress-nginx"

