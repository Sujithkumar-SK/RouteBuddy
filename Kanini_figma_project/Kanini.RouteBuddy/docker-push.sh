#!/bin/bash

# Docker Hub Push Script

# Load environment variables
source .env

# Login to Docker Hub
echo "Logging into Docker Hub..."
echo $DOCKERHUB_PASSWORD | docker login -u $DOCKERHUB_USERNAME --password-stdin

# Build and Push Backend
echo "Building Backend..."
cd backend
docker build -t $DOCKERHUB_USERNAME/routebuddy-backend:latest .
docker tag $DOCKERHUB_USERNAME/routebuddy-backend:latest $DOCKERHUB_USERNAME/routebuddy-backend:v1.0
echo "Pushing Backend..."
docker push $DOCKERHUB_USERNAME/routebuddy-backend:latest
docker push $DOCKERHUB_USERNAME/routebuddy-backend:v1.0

# Build and Push Frontend
echo "Building Frontend..."
cd ../frontend/routebuddy
docker build -t $DOCKERHUB_USERNAME/routebuddy-frontend:latest .
docker tag $DOCKERHUB_USERNAME/routebuddy-frontend:latest $DOCKERHUB_USERNAME/routebuddy-frontend:v1.0
echo "Pushing Frontend..."
docker push $DOCKERHUB_USERNAME/routebuddy-frontend:latest
docker push $DOCKERHUB_USERNAME/routebuddy-frontend:v1.0

echo "✅ Images pushed to Docker Hub successfully!"
docker logout
