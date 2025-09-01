# Azure Deployment Setup Guide

This guide will help you set up automated deployment from GitHub to your Azure App Service.

## Prerequisites

1. Your Azure App Service is already created: `PinballCalendar20170829102021`
2. Your code is in a GitHub repository
3. You have admin access to both Azure and GitHub

## Setup Steps

### 1. Get Azure Publish Profile

1. Go to your Azure App Service in the Azure Portal
2. Navigate to **Overview** ? **Get publish profile**
3. Download the `.PublishSettings` file
4. Open the file and copy the entire XML content

### 2. Configure GitHub Secrets

In your GitHub repository:

1. Go to **Settings** ? **Secrets and variables** ? **Actions**
2. Click **New repository secret**
3. Create the following secrets:

#### Required Secret:
- **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE`
- **Value**: Paste the entire content of the `.PublishSettings` file

### 3. Configure Azure App Service Settings

In your Azure App Service, go to **Settings** ? **Environment variables** and add:

#### Required Application Settings:
- **Name**: `WPPRKey`
- **Value**: Your actual WPPR API key
- **Deployment slot setting**: ? (checked if available)

#### Optional but Recommended:
- **Name**: `ASPNETCORE_ENVIRONMENT`
- **Value**: `Production`
- **Deployment slot setting**: ? (checked if available)

**Alternative locations to find configuration settings:**
- **Settings** ? **Configuration** (if available in your Azure Portal view)
- **Settings** ? **Application settings** (older portal versions)

### 4. Verify Deployment

1. Push changes to the `main` branch
2. Go to GitHub **Actions** tab to monitor the deployment
3. Once successful, visit: `https://pinballcalendar20170829102021.azurewebsites.net/swagger`

## Workflow Details

### Triggers
- **Automatic deployment**: Push to `main` branch
- **Build only**: Pull requests to `main` (for testing)

### Build Process
1. Checkout code
2. Setup .NET 9
3. Restore dependencies
4. Build application
5. Run tests (if any)
6. Publish application

### Deployment Process
1. Download build artifacts
2. Deploy to Azure App Service using publish profile
3. Application starts with production configuration

## Monitoring

### Check Deployment Status
- GitHub Actions: `https://github.com/YOUR-USERNAME/YOUR-REPO/actions`
- Azure Deployment Center: Azure Portal ? Your App Service ? Deployment Center

### Check Application Logs
- Azure Portal ? Your App Service ? Log stream
- Application Insights (if configured)

## Troubleshooting

### Common Issues

1. **Deployment fails with 403/401 error**
   - Verify publish profile is correct and not expired
   - Re-download publish profile from Azure

2. **Application starts but API calls fail**
   - Check if `WPPRKey` is set in Azure App Service Environment variables
   - Verify the API key is valid

3. **Build fails**
   - Check GitHub Actions logs for specific error
   - Ensure all NuGet packages are compatible with .NET 9

4. **Can't find Configuration/Environment variables in Azure Portal**
   - Try: **Settings** ? **Environment variables**
   - Try: **Settings** ? **Configuration** 
   - Try: **Settings** ? **Application settings**
   - The exact menu name may vary by Azure Portal version

### Testing the Deployment

```bash
# Test the deployed API
curl https://pinballcalendar20170829102021.azurewebsites.net/api/calendar/chicago/50

# Check Swagger UI
# Visit: https://pinballcalendar20170829102021.azurewebsites.net/swagger
```

## Security Notes

- Never commit API keys or secrets to the repository
- Use Azure App Service Environment variables for production secrets
- Enable deployment slot settings for configuration values
- Consider using Azure Key Vault for highly sensitive data

## Next Steps

1. Set up custom domain (optional)
2. Configure Application Insights for monitoring
3. Set up staging slots for blue-green deployments
4. Configure auto-scaling rules if needed