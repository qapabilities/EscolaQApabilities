# Script de Deploy para Azure Kubernetes Service (AKS)
# Escola QApabilities - Student Service

param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$ClusterName,
    
    [Parameter(Mandatory=$true)]
    [string]$Location = "Brazil South",
    
    [Parameter(Mandatory=$false)]
    [string]$NodeCount = "3",
    
    [Parameter(Mandatory=$false)]
    [string]$NodeSize = "Standard_B2s",
    
    [Parameter(Mandatory=$false)]
    [string]$KubernetesVersion = "1.28"
)

Write-Host "🚀 Iniciando deploy do Escola QApabilities no AKS..." -ForegroundColor Green

# Verificar se Azure CLI está instalado
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Error "Azure CLI não está instalado. Instale em: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
}

# Verificar se kubectl está instalado
if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) {
    Write-Host "📦 Instalando kubectl..." -ForegroundColor Yellow
    az aks install-cli
}

# Verificar se Docker está instalado
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker não está instalado. Instale em: https://docs.docker.com/desktop/install/windows-install/"
    exit 1
}

Write-Host "1️⃣ Fazendo login no Azure..." -ForegroundColor Blue
az login

Write-Host "2️⃣ Criando Resource Group..." -ForegroundColor Blue
az group create --name $ResourceGroupName --location $Location

Write-Host "3️⃣ Criando cluster AKS..." -ForegroundColor Blue
az aks create `
    --resource-group $ResourceGroupName `
    --name $ClusterName `
    --node-count $NodeCount `
    --node-vm-size $NodeSize `
    --kubernetes-version $KubernetesVersion `
    --enable-addons monitoring,http_application_routing `
    --enable-managed-identity `
    --generate-ssh-keys `
    --enable-cluster-autoscaler `
    --min-count 1 `
    --max-count 5

Write-Host "4️⃣ Configurando credenciais do kubectl..." -ForegroundColor Blue
az aks get-credentials --resource-group $ResourceGroupName --name $ClusterName --overwrite-existing

Write-Host "5️⃣ Instalando NGINX Ingress Controller..." -ForegroundColor Blue
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml

Write-Host "6️⃣ Instalando cert-manager para SSL..." -ForegroundColor Blue
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.1/cert-manager.yaml

Write-Host "7️⃣ Aguardando NGINX Ingress Controller..." -ForegroundColor Blue
kubectl wait --namespace ingress-nginx --for=condition=ready pod --selector=app.kubernetes.io/component=controller --timeout=300s

Write-Host "8️⃣ Buildando e fazendo push da imagem Docker..." -ForegroundColor Blue
$registryName = "$($ClusterName)registry"
$imageTag = "latest"

# Criar Azure Container Registry
az acr create --resource-group $ResourceGroupName --name $registryName --sku Basic

# Fazer login no ACR
az acr login --name $registryName

# Build e push da imagem
$imageName = "$registryName.azurecr.io/student-service:$imageTag"
docker build -t $imageName .
docker push $imageName

# Anexar ACR ao AKS
az aks update -n $ClusterName -g $ResourceGroupName --attach-acr $registryName

Write-Host "9️⃣ Aplicando manifestos Kubernetes..." -ForegroundColor Blue

# Atualizar imagem no manifesto
(Get-Content k8s/student-service.yaml) -replace 'escolaqapabilities/student-service:latest', $imageName | Set-Content k8s/student-service.yaml

# Aplicar manifestos
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/sql-server.yaml

Write-Host "⏳ Aguardando SQL Server estar pronto..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=sql-server -n escola-qapabilities --timeout=300s

# Criar secrets (você deve substituir os valores)
Write-Host "🔐 Criando secrets..." -ForegroundColor Blue
Write-Host "⚠️  ATENÇÃO: Substitua os valores dos secrets antes do deploy em produção!" -ForegroundColor Red

$dbConnectionString = "Server=sql-server-service.escola-qapabilities.svc.cluster.local,1433;Database=EscolaQApabilitiesStudentService;User Id=SA;Password=EscolaQApabilities123!;TrustServerCertificate=true;Encrypt=false;"
$jwtKey = "escola-qapabilities-super-secret-jwt-key-for-production-32-chars-minimum-key"

kubectl create secret generic student-service-secret `
    --from-literal=STUDENT_DB_CONNECTION_STRING="$dbConnectionString" `
    --from-literal=JWT_KEY="$jwtKey" `
    -n escola-qapabilities `
    --dry-run=client -o yaml | kubectl apply -f -

kubectl apply -f k8s/student-service.yaml
kubectl apply -f k8s/ingress.yaml

Write-Host "🔍 Verificando status do deployment..." -ForegroundColor Blue
kubectl get pods -n escola-qapabilities
kubectl get services -n escola-qapabilities
kubectl get ingress -n escola-qapabilities

Write-Host "📊 Obtendo IP externo..." -ForegroundColor Blue
Write-Host "Aguardando IP externo do Load Balancer..."
$externalIP = ""
do {
    Start-Sleep 10
    $externalIP = kubectl get service ingress-nginx-controller -n ingress-nginx -o jsonpath='{.status.loadBalancer.ingress[0].ip}'
    Write-Host "Aguardando..." -NoNewline
} while ([string]::IsNullOrEmpty($externalIP))

Write-Host ""
Write-Host "✅ Deploy concluído com sucesso!" -ForegroundColor Green
Write-Host "🌐 IP Externo: $externalIP" -ForegroundColor Cyan
Write-Host "📋 Para testar a API:" -ForegroundColor Yellow
Write-Host "   curl http://$externalIP/health" -ForegroundColor White
Write-Host "   curl http://$externalIP/api/auth/login" -ForegroundColor White
Write-Host ""
Write-Host "🔧 Comandos úteis:" -ForegroundColor Yellow
Write-Host "   kubectl get pods -n escola-qapabilities" -ForegroundColor White
Write-Host "   kubectl logs -f deployment/student-service -n escola-qapabilities" -ForegroundColor White
Write-Host "   kubectl describe ingress student-service-ingress -n escola-qapabilities" -ForegroundColor White

