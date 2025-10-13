#!/bin/bash
# Gera uma chave JWT segura (base64) para a variável de ambiente JWT_SECRET_KEY

KEY=$(openssl rand -base64 48)
echo "🚀 JWT Secret Key gerada:"
echo $KEY
echo ""
echo "Copie e cole no seu .env como JWT_SECRET_KEY=$KEY"

# Utilização:
# - navegue para ./Api e rode:
# chmod +x generate_jwt_secret.sh # para setar as permissões
# ./generate_jwt_secret.sh # para gerar a chave segura