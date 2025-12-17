#!/bin/bash

# Azure RouteBuddy Deployment Setup Script

# Variables
RESOURCE_GROUP="routebuddy-rg"
LOCATION="eastus"
ACR_NAME="routebuddyacr"
APP_PLAN="routebuddy-plan"
BACKEND_APP="routebuddy-api"
FRONTEND_APP="routebuddy-frontend"

echo "Creating Resource Group..."
az group create --name $RESOURCE_GROUP --location $LOCATION

echo "Creating Azure Container Registry..."
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Basic \
  --admin-enabled true

echo "Getting ACR credentials..."
ACR_USERNAME=$(az acr credential show --name $ACR_NAME --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name $ACR_NAME --query passwords[0].value -o tsv)

echo "Creating App Service Plan..."
az appservice plan create \
  --name $APP_PLAN \
  --resource-group $RESOURCE_GROUP \
  --is-linux \
  --sku B2

echo "Creating Backend Web App..."
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_PLAN \
  --name $BACKEND_APP \
  --deployment-container-image-name $ACR_NAME.azurecr.io/routebuddy-backend:latest

echo "Configuring Backend ACR..."
az webapp config container set \
  --name $BACKEND_APP \
  --resource-group $RESOURCE_GROUP \
  --docker-registry-server-url https://$ACR_NAME.azurecr.io \
  --docker-registry-server-user $ACR_USERNAME \
  --docker-registry-server-password $ACR_PASSWORD

echo "Creating Frontend Web App..."
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_PLAN \
  --name $FRONTEND_APP \
  --deployment-container-image-name $ACR_NAME.azurecr.io/routebuddy-frontend:latest

echo "Configuring Frontend ACR..."
az webapp config container set \
  --name $FRONTEND_APP \
  --resource-group $RESOURCE_GROUP \
  --docker-registry-server-url https://$ACR_NAME.azurecr.io \
  --docker-registry-server-user $ACR_USERNAME \
  --docker-registry-server-password $ACR_PASSWORD

echo "Setup complete!"
echo "Backend URL: https://$BACKEND_APP.azurewebsites.net"
echo "Frontend URL: https://$FRONTEND_APP.azurewebsites.net"
echo "ACR Username: $ACR_USERNAME"
echo "ACR Password: $ACR_PASSWORD"
