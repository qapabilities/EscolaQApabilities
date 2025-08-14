# 🔐 Setup dos Secrets para GitHub Actions

Este arquivo contém as instruções para configurar os secrets necessários para a pipeline de CI/CD funcionar corretamente.

## 📋 Secrets Necessários

### 1. **AZURE_CREDENTIALS** (OBRIGATÓRIO)

Este é o secret mais importante. Contém as credenciais do Service Principal que permite ao GitHub Actions acessar o Azure.

#### Como Criar:

1. **Abra o Azure Cloud Shell** ou instale o Azure CLI localmente
2. **Execute o comando para criar o Service Principal:**

```bash
# Substitua YOUR_SUBSCRIPTION_ID pela sua subscription ID
az ad sp create-for-rbac \
  --name "escola-qapabilities-github-actions" \
  --role contributor \
  --scopes /subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/rg-escola-qapabilities \
  --sdk-auth
```

3. **O resultado será um JSON como este:**

```json
{
  "clientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "clientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "subscriptionId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "tenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

4. **Copie TODO o JSON (incluindo as chaves)** 

#### Configurar no GitHub:

1. Vá para seu repositório no GitHub
2. Clique em **Settings** → **Secrets and variables** → **Actions**
3. Clique em **New repository secret**
4. **Name**: `AZURE_CREDENTIALS`
5. **Value**: Cole o JSON completo que você copiou
6. Clique em **Add secret**

## 🔐 Permissões Adicionais (Se Necessário)

Se você encontrar erros de permissão, pode ser necessário dar permissões específicas:

### Para Azure Container Registry:

```bash
# Obtenha o CLIENT_ID do Service Principal
CLIENT_ID=$(az ad sp list --display-name "escola-qapabilities-github-actions" --query "[0].appId" -o tsv)

# Dê permissão de push para o ACR
az role assignment create \
  --assignee $CLIENT_ID \
  --role AcrPush \
  --scope /subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/rg-escola-qapabilities/providers/Microsoft.ContainerRegistry/registries/crescolaqapabilities
```

### Para AKS:

```bash
# Dê permissão de usuário do cluster AKS
az role assignment create \
  --assignee $CLIENT_ID \
  --role "Azure Kubernetes Service Cluster User Role" \
  --scope /subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/rg-escola-qapabilities/providers/Microsoft.ContainerService/managedClusters/aks-escola-qapabilities
```

## 🧪 Testando as Credenciais

Você pode testar se as credenciais funcionam:

```bash
# Login usando o Service Principal
az login --service-principal \
  -u YOUR_CLIENT_ID \
  -p YOUR_CLIENT_SECRET \
  --tenant YOUR_TENANT_ID

# Testar acesso ao resource group
az group show --name rg-escola-qapabilities

# Testar acesso ao AKS
az aks show --resource-group rg-escola-qapabilities --name aks-escola-qapabilities

# Testar acesso ao ACR
az acr show --name crescolaqapabilities --resource-group rg-escola-qapabilities
```

## 🚀 Validação da Pipeline

Após configurar os secrets:

1. **Faça um commit simples** para testar a CI:
```bash
git add .
git commit -m "test: Trigger CI pipeline"
git push origin feature/test-ci
```

2. **Crie um Pull Request** para testar a validação de PR

3. **Faça merge para main** para testar o deploy completo

## 🔍 Troubleshooting

### Erro: "Azure login failed"
- Verifique se o JSON do AZURE_CREDENTIALS está correto
- Certifique-se de que não há espaços extras ou caracteres especiais

### Erro: "ACR access denied"
- Execute o comando de permissão do ACR mostrado acima

### Erro: "AKS access denied"  
- Execute o comando de permissão do AKS mostrado acima

### Erro: "Resource group not found"
- Certifique-se de que o resource group "rg-escola-qapabilities" existe
- Verifique se está na subscription correta

## 📊 Monitoramento

Após configurar tudo:

- Vá em **Actions** no GitHub para ver os workflows rodando
- Monitore os badges no README.md
- Acompanhe os logs detalhados de cada job

## ✅ Checklist Final

- [ ] Secret `AZURE_CREDENTIALS` configurado
- [ ] Service Principal tem permissões no Resource Group
- [ ] Service Principal tem permissões no ACR
- [ ] Service Principal tem permissões no AKS
- [ ] Resource Group `rg-escola-qapabilities` existe
- [ ] AKS `aks-escola-qapabilities` existe e está rodando
- [ ] ACR `crescolaqapabilities` existe
- [ ] Primeiro commit/PR executado com sucesso

**🎉 Com isso, sua pipeline de CI/CD estará 100% funcional!**
