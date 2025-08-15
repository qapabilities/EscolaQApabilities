# 🔒 Configuração de Credenciais de Segurança

## ⚠️ **IMPORTANTE - PRODUÇÃO**

**NUNCA** use as credenciais padrão em produção! Este projeto foi configurado com senhas seguras via variáveis de ambiente.

## 🔑 **Credenciais Atuais**

### 📋 **Para Desenvolvimento/Demo:**
- **Admin**: `admin@qapabilities.com` / `SecureAdmin2024!`
- **Teacher**: `teacher@qapabilities.com` / `SecureTeacher2024!`

### 🚨 **Para Produção:**

#### **1. Atualizar Secret no Kubernetes:**
```bash
# Gerar novas senhas seguras
ADMIN_PASS="SuaSenhaAdminMuitoSegura123!"
TEACHER_PASS="SuaSenhaTeacherMuitoSegura123!"

# Atualizar secret
kubectl patch secret student-service-secret -n escola-qapabilities --type='merge' -p="{
  \"data\": {
    \"DEFAULT_ADMIN_PASSWORD\": \"$(echo -n $ADMIN_PASS | base64)\",
    \"DEFAULT_TEACHER_PASSWORD\": \"$(echo -n $TEACHER_PASS | base64)\"
  }
}"
```

#### **2. Reiniciar Deployment:**
```bash
kubectl rollout restart deployment/student-service -n escola-qapabilities
```

## 🛡️ **Outras Configurações de Segurança**

### **JWT Key:**
```bash
# Gerar nova chave JWT (32+ caracteres)
JWT_KEY="sua-super-chave-jwt-de-producao-muito-segura-32-chars+"

kubectl patch secret student-service-secret -n escola-qapabilities --type='merge' -p="{
  \"data\": {
    \"JWT_KEY\": \"$(echo -n $JWT_KEY | base64)\"
  }
}"
```

### **CORS Origins:**
Atualize no ConfigMap para seus domínios reais:
```bash
kubectl patch configmap student-service-config -n escola-qapabilities --type='merge' -p="{
  \"data\": {
    \"SECURITY_CORS_ORIGINS\": \"https://seudominio.com,https://admin.seudominio.com\"
  }
}"
```

## 📋 **Checklist de Segurança**

- [ ] ✅ Senhas hardcoded removidas do código
- [ ] ✅ Variáveis de ambiente configuradas
- [ ] ✅ Secrets no Kubernetes usando base64
- [ ] ✅ JWT key com 32+ caracteres
- [ ] ✅ CORS configurado para domínios específicos
- [ ] ✅ Rate limiting ativo (100 req/min)
- [ ] ✅ Headers de segurança configurados
- [ ] ⚠️ **TODO**: Alterar senhas padrão em produção
- [ ] ⚠️ **TODO**: Configurar HTTPS com certificados válidos
- [ ] ⚠️ **TODO**: Monitoramento e logs de segurança

## 🚀 **Deployment Seguro**

Sempre que fizer deploy em produção:

1. **Nunca** commitar senhas no código
2. **Sempre** usar variáveis de ambiente
3. **Sempre** usar HTTPS em produção
4. **Sempre** alterar senhas padrão
5. **Sempre** usar JWT keys únicos e seguros

---
**⚠️ LEMBRE-SE: Segurança é responsabilidade de todos!** 🔒
