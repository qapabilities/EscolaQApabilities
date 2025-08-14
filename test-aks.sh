#!/bin/bash
# Script de Teste para AKS
# Escola QApabilities - Student Service

set -e

echo "🧪 Testando aplicação no AKS..."

# Obter IP externo
echo "1️⃣ Obtendo IP externo do Load Balancer..."
EXTERNAL_IP=""
while [ -z $EXTERNAL_IP ]; do
  echo "Aguardando IP externo..."
  EXTERNAL_IP=$(kubectl get service ingress-nginx-controller -n ingress-nginx -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>/dev/null)
  [ -z "$EXTERNAL_IP" ] && sleep 10
done

echo "✅ IP externo obtido: $EXTERNAL_IP"

# Aguardar aplicação estar pronta
echo "2️⃣ Aguardando aplicação estar pronta..."
kubectl wait --for=condition=ready pod -l app=student-service -n escola-qapabilities --timeout=300s

echo "3️⃣ Testando endpoints..."

# Teste Health Check
echo "🏥 Testando Health Check..."
if curl -f -s "http://$EXTERNAL_IP/health" > /dev/null; then
  echo "✅ Health Check OK"
  HEALTH_RESPONSE=$(curl -s "http://$EXTERNAL_IP/health")
  echo "   Response: $HEALTH_RESPONSE"
else
  echo "❌ Health Check FAILED"
fi

# Teste Swagger
echo "📚 Testando Swagger UI..."
if curl -f -s "http://$EXTERNAL_IP/swagger/index.html" > /dev/null; then
  echo "✅ Swagger UI OK"
else
  echo "❌ Swagger UI FAILED"
fi

# Teste Autenticação
echo "🔐 Testando Autenticação..."
LOGIN_RESPONSE=$(curl -s -X POST "http://$EXTERNAL_IP/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@qapabilities.com", "password": "admin123"}')

if echo "$LOGIN_RESPONSE" | grep -q "token"; then
  echo "✅ Login OK"
  TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
  echo "   Token obtido: ${TOKEN:0:50}..."
  
  # Teste API com token
  echo "📋 Testando API de Estudantes..."
  STUDENTS_RESPONSE=$(curl -s -H "Authorization: Bearer $TOKEN" "http://$EXTERNAL_IP/api/students/all")
  if echo "$STUDENTS_RESPONSE" | grep -q "\["; then
    echo "✅ API Estudantes OK"
    STUDENT_COUNT=$(echo "$STUDENTS_RESPONSE" | grep -o '\[' | wc -l)
    echo "   Estudantes encontrados: $STUDENT_COUNT"
  else
    echo "❌ API Estudantes FAILED"
    echo "   Response: $STUDENTS_RESPONSE"
  fi
  
else
  echo "❌ Login FAILED"
  echo "   Response: $LOGIN_RESPONSE"
fi

echo ""
echo "🎯 URLs da aplicação:"
echo "   Health Check: http://$EXTERNAL_IP/health"
echo "   Swagger UI: http://$EXTERNAL_IP/swagger"
echo "   API Login: POST http://$EXTERNAL_IP/api/auth/login"
echo "   API Students: GET http://$EXTERNAL_IP/api/students/all"
echo ""
echo "🔑 Credenciais de teste:"
echo "   Admin: admin@qapabilities.com / admin123"
echo "   Teacher: teacher@qapabilities.com / teacher123"
echo ""
echo "🎉 Teste concluído!"

