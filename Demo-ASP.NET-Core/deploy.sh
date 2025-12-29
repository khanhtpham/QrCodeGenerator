#!/bin/bash

# Script để build, tag và push Docker image lên PECC2 registry
# Sau đó deploy lên Kubernetes

set -e  # Exit on error

# Cấu hình
REGISTRY="registry.pecc2.vn"
USERNAME="pecc2"
PASSWORD="pecc2pa"
IMAGE_NAME="qrcode-generator"
VERSION="v1.0.0"

echo "========================================="
echo "QR Code Generator - Build & Deploy"
echo "========================================="

# 1. Build Docker image
echo ""
echo "=== Step 1: Building Docker image ==="
docker build -t ${IMAGE_NAME}:latest .

# 2. Tag image
echo ""
echo "=== Step 2: Tagging images ==="
docker tag ${IMAGE_NAME}:latest ${REGISTRY}/${IMAGE_NAME}:latest
docker tag ${IMAGE_NAME}:latest ${REGISTRY}/${IMAGE_NAME}:${VERSION}

# 3. Login to registry
echo ""
echo "=== Step 3: Logging in to PECC2 registry ==="
echo ${PASSWORD} | docker login ${REGISTRY} --username ${USERNAME} --password-stdin

# 4. Push images
echo ""
echo "=== Step 4: Pushing images to registry ==="
docker push ${REGISTRY}/${IMAGE_NAME}:latest
docker push ${REGISTRY}/${IMAGE_NAME}:${VERSION}

# 5. Deploy to Kubernetes
echo ""
echo "=== Step 5: Deploying to Kubernetes ==="

# Apply namespace
echo "Creating namespace..."
kubectl apply -f k8s/namespace.yaml

# Apply secrets
echo "Creating secrets..."
kubectl apply -f k8s/registry-secret.yaml
kubectl apply -f k8s/secrect.yaml

# Apply deployment
echo "Deploying application..."
kubectl apply -f k8s/deployment.yaml

# Wait for deployment to be ready
echo ""
echo "Waiting for deployment to be ready..."
kubectl rollout status deployment/qrcode-generator -n qrgenerator --timeout=300s

# Show deployment status
echo ""
echo "=== Deployment Status ==="
kubectl get pods -n qrgenerator
kubectl get svc -n qrgenerator
kubectl get ingress -n qrgenerator

echo ""
echo "========================================="
echo "✅ Deployment completed successfully!"
echo "========================================="
echo ""
echo "Images pushed:"
echo "  - ${REGISTRY}/${IMAGE_NAME}:latest"
echo "  - ${REGISTRY}/${IMAGE_NAME}:${VERSION}"
echo ""
echo "Access your application at:"
echo "  - https://qrcode.pecc2.vn"
echo ""
echo "To view logs:"
echo "  kubectl logs -f deployment/qrcode-generator -n qrgenerator"
echo ""
