$SOLUTION="aspiresbmessaging"              # Solution name
$LOCATION="westeurope"                     # Azure location
$RESOURCE_GROUP="aspiresbdemo-rg"          # Resource Group name, e.g. eshopliterg
$CONTAINER_REGISTRY="rburksregistry"       # Azure Container Registry name, e.g. eshoplitecr
$IMAGE_PREFIX="aspire-servicebusmessaging"     # Container image name prefix, e.g. eshoplite
$IDENTITY="$($SOLUTION.ToLower())-id"      # Azure Managed Identity, e.g. eshopliteid
$ENVIRONMENT="$($SOLUTION.ToLower())-env"  # Azure Container Apps Environment name

# Required Extensions and Providers
az extension add --name containerapp --upgrade
az provider register --namespace Microsoft.App
az provider register --namespace Microsoft.OperationalInsights --wait

# # Login to ACR instance & store server URL
az acr login --name $CONTAINER_REGISTRY
$loginServer = (az acr show --name $CONTAINER_REGISTRY --query loginServer --output tsv)

# # Create resource group
# az group create --location $LOCATION --name $RESOURCE_GROUP

# # Create the container apps environment
az containerapp env create --name $ENVIRONMENT --resource-group $RESOURCE_GROUP --location $LOCATION

# # Publish the projects as container images to ACR
# dotnet publish -r linux-x64 -p:PublishProfile=DefaultContainer -p:ContainerRegistry=$loginServer

# Create managed identity
az identity create --name $IDENTITY --resource-group $RESOURCE_GROUP --location $LOCATION
$identityId = (az identity show --name $IDENTITY --resource-group $RESOURCE_GROUP --query id --output tsv)

# These logger configuration values will adjust the apps log format to be better suited for the Azure Container Apps environment
$loggerFormat = "Logging__Console__FormatterName=json"
$loggerSingleLine = "Logging__Console__FormatterOptions__SingleLine=true"
$loggerIncludeScopes = "Logging__Console__FormatterOptions__IncludeScopes=true"

# Create the proxyapi
az containerapp create --name $SOLUTION-proxyapi-aca --resource-group $RESOURCE_GROUP --environment $ENVIRONMENT `
    --image $loginServer/$IMAGE_PREFIX-proxyapi --ingress external --target-port 8080 `
    --env-vars $loggerFormat $loggerSingleLine $loggerIncludeScopes --registry-server $loginServer --registry-identity $identityId

az containerapp create --name $SOLUTION-samplesubscriptionservice-aca --resource-group $RESOURCE_GROUP --environment $ENVIRONMENT `
--image $loginServer/$IMAGE_PREFIX-samplesubscriptionservice --ingress external --target-port 8080 `
--env-vars $loggerFormat $loggerSingleLine $loggerIncludeScopes --registry-server $loginServer --registry-identity $identityId

az containerapp create --name $SOLUTION-demosubscriptionservice-aca --resource-group $RESOURCE_GROUP --environment $ENVIRONMENT `
--image $loginServer/$IMAGE_PREFIX-demosubscriptionservice --ingress external --target-port 8080 `
--env-vars $loggerFormat $loggerSingleLine $loggerIncludeScopes --registry-server $loginServer --registry-identity $identityId

az containerapp update --name $SOLUTION-proxyapi-aca --resource-group $RESOURCE_GROUP --set-env-vars `
     'ConnectionStrings__ServiceBusConnection="<service bus Connection for SAS Policy>"'

az containerapp update --name $SOLUTION-samplesubscriptionservice-aca --resource-group $RESOURCE_GROUP --set-env-vars `
'ConnectionStrings__ServiceBusConnection="<service bus Connection for SAS Policy>"'

az containerapp update --name $SOLUTION-demosubscriptionservice-aca --resource-group $RESOURCE_GROUP --set-env-vars `
'ConnectionStrings__ServiceBusConnection="<service bus Connection for SAS Policy>"' `
'WeatherForecastApiBaseUrl="<baseurl forecastapi>"'
