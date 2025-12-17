# RouteBuddy Azure Deployment Guide

## Prerequisites
- Azure Subscription
- GitHub Repository
- Jenkins Server (Azure VM or local)
- Azure CLI installed on Jenkins

## Step 1: Setup Azure Resources

### 1.1 Create Resource Group
```bash
az group create --name routebuddy-rg --location eastus
```

### 1.2 Create Azure Container Registry
```bash
az acr create \
  --resource-group routebuddy-rg \
  --name routebuddyacr \
  --sku Basic \
  --admin-enabled true

# Get credentials
az acr credential show --name routebuddyacr
```

### 1.3 Create App Service Plan
```bash
az appservice plan create \
  --name routebuddy-plan \
  --resource-group routebuddy-rg \
  --is-linux \
  --sku B2
```

### 1.4 Create Backend Web App
```bash
az webapp create \
  --resource-group routebuddy-rg \
  --plan routebuddy-plan \
  --name routebuddy-api \
  --deployment-container-image-name routebuddyacr.azurecr.io/routebuddy-backend:latest

# Configure ACR
az webapp config container set \
  --name routebuddy-api \
  --resource-group routebuddy-rg \
  --docker-registry-server-url https://routebuddyacr.azurecr.io \
  --docker-registry-server-user <ACR_USERNAME> \
  --docker-registry-server-password <ACR_PASSWORD>

# Set environment variables
az webapp config appsettings set \
  --resource-group routebuddy-rg \
  --name routebuddy-api \
  --settings \
    ConnectionStrings__DatabaseConnectionString="Server=tcp:routebuddy.database.windows.net,1433;Database=RouteBuddy;User Id=routebuddy_team;Password=Route@123;TrustServerCertificate=True;" \
    TokenKey="RouteBuddy-Super-Secret-JWT-Key-Minimum-32-Characters-Long-2024" \
    EmailSettings__SmtpServer="smtp.gmail.com" \
    EmailSettings__SmtpPort="587" \
    EmailSettings__SenderEmail="sujinano777@gmail.com" \
    EmailSettings__Username="sujinano777@gmail.com" \
    EmailSettings__AppPassword="djxw gchg kahe zqnt" \
    RazorpaySettings__KeyId="rzp_test_Re7bTcqTQv7BOq" \
    RazorpaySettings__KeySecret="Fu20IWQbQ40Q02NmI9l6DQLF" \
    AzureBlob__ConnectionString="DefaultEndpointsProtocol=https;AccountName=busimages;AccountKey=3RaMxry83wnAB40DXqObK2wnKDw1gPbi0hXt/wNoJ87X/QqihTUX95R6PuK27c0XfObDYTI7IDr6+AStSPVbBQ==;EndpointSuffix=core.windows.net" \
    AzureBlob__ContainerName="busimages"
```

### 1.5 Create Frontend Web App
```bash
az webapp create \
  --resource-group routebuddy-rg \
  --plan routebuddy-plan \
  --name routebuddy-frontend \
  --deployment-container-image-name routebuddyacr.azurecr.io/routebuddy-frontend:latest

# Configure ACR
az webapp config container set \
  --name routebuddy-frontend \
  --resource-group routebuddy-rg \
  --docker-registry-server-url https://routebuddyacr.azurecr.io \
  --docker-registry-server-user <ACR_USERNAME> \
  --docker-registry-server-password <ACR_PASSWORD>
```

## Step 2: Setup Jenkins on Azure VM

### 2.1 Create Ubuntu VM
```bash
az vm create \
  --resource-group routebuddy-rg \
  --name jenkins-vm \
  --image Ubuntu2204 \
  --size Standard_B2s \
  --admin-username azureuser \
  --generate-ssh-keys \
  --public-ip-sku Standard

# Open port 8080 for Jenkins
az vm open-port --port 8080 --resource-group routebuddy-rg --name jenkins-vm
```

### 2.2 Install Jenkins on VM
```bash
# SSH into VM
ssh azureuser@<VM_PUBLIC_IP>

# Install Java
sudo apt update
sudo apt install -y openjdk-17-jdk

# Install Jenkins
curl -fsSL https://pkg.jenkins.io/debian-stable/jenkins.io-2023.key | sudo tee \
  /usr/share/keyrings/jenkins-keyring.asc > /dev/null
echo deb [signed-by=/usr/share/keyrings/jenkins-keyring.asc] \
  https://pkg.jenkins.io/debian-stable binary/ | sudo tee \
  /etc/apt/sources.list.d/jenkins.list > /dev/null
sudo apt update
sudo apt install -y jenkins

# Install Docker
sudo apt install -y docker.io
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins

# Install Azure CLI
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Get initial admin password
sudo cat /var/lib/jenkins/secrets/initialAdminPassword
```

### 2.3 Configure Jenkins
1. Access Jenkins: `http://<VM_PUBLIC_IP>:8080`
2. Install suggested plugins
3. Install additional plugins:
   - Docker Pipeline
   - Azure CLI Plugin
   - GitHub Integration

### 2.4 Add Credentials in Jenkins
1. Go to: Manage Jenkins → Credentials → System → Global credentials
2. Add:
   - **GitHub credentials** (Username with password or Personal Access Token)
   - **Azure credentials** (Username with password - Service Principal)
     - Username: Service Principal App ID
     - Password: Service Principal Password

### 2.5 Create Service Principal
```bash
az ad sp create-for-rbac --name jenkins-sp --role contributor \
  --scopes /subscriptions/<SUBSCRIPTION_ID>/resourceGroups/routebuddy-rg
```
Save the output (appId, password, tenant)

## Step 3: Configure Jenkins Pipeline

1. Create New Item → Pipeline
2. Configure:
   - **Pipeline script from SCM**
   - **SCM**: Git
   - **Repository URL**: Your GitHub repo
   - **Branch**: main
   - **Script Path**: Jenkinsfile

3. Update Jenkinsfile with:
   - Your GitHub repo URL
   - Your Azure Tenant ID
   - Correct credential IDs

## Step 4: Update Frontend API URL

Update `frontend/routebuddy/src/utils/constants.ts`:
```typescript
export const API_BASE_URL = 'https://routebuddy-api.azurewebsites.net';
```

Update `backend/Kanini.RouteBuddy.Api/Program.cs` CORS:
```csharp
policy.WithOrigins(
    "http://localhost:5173",
    "https://routebuddy-frontend.azurewebsites.net"
)
```

## Step 5: Push to GitHub

```bash
git add .
git commit -m "Add Docker and Jenkins configuration"
git push origin main
```

## Step 6: Run Jenkins Pipeline

1. Go to Jenkins Dashboard
2. Click on your pipeline
3. Click "Build Now"
4. Monitor the build progress

## Step 7: Verify Deployment

- Backend API: `https://routebuddy-api.azurewebsites.net/swagger`
- Frontend: `https://routebuddy-frontend.azurewebsites.net`

## Troubleshooting

### Check App Service Logs
```bash
az webapp log tail --name routebuddy-api --resource-group routebuddy-rg
az webapp log tail --name routebuddy-frontend --resource-group routebuddy-rg
```

### Enable Continuous Deployment
```bash
az webapp deployment container config \
  --name routebuddy-api \
  --resource-group routebuddy-rg \
  --enable-cd true
```

### Restart Apps
```bash
az webapp restart --name routebuddy-api --resource-group routebuddy-rg
az webapp restart --name routebuddy-frontend --resource-group routebuddy-rg
```

## Cost Optimization

- Use B1 tier for App Service Plan (development)
- Use Basic tier for ACR
- Stop Jenkins VM when not in use
- Consider Azure DevOps instead of Jenkins VM

## Security Best Practices

1. Move secrets to Azure Key Vault
2. Use Managed Identity for Azure resources
3. Enable HTTPS only
4. Configure firewall rules for SQL Database
5. Use private endpoints for production
