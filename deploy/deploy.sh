#!/usr/bin/env bash
# Uso: deploy.sh <staging|production> <imagem>
# Executado no servidor via SSH pelo pipeline.
set -euo pipefail

ENV_NAME="$1"
IMAGE="$2"
BASE_DIR="$HOME/esg/$ENV_NAME"

cd "$BASE_DIR"
export APP_IMAGE="$IMAGE"
docker compose pull app
docker compose up -d --remove-orphans
docker image prune -f
echo "Deploy de $IMAGE concluido em $ENV_NAME"
