$LOCATION="westeurope"                     # Azure location
$RESOURCE_GROUP="common-rg"                # Resource Group name
$CONTAINER_REGISTRY="rburksregistry"       # Azure Container Registry name
$SB_NSP="aspiresbdemos"

az group create -n $RESOURCE_GROUP --location $LOCATION

# Create Servicebus Namespace
az servicebus namespace create -g $RESOURCE_GROUP --name $SB_NSP --location $LOCATION

# Create Topic and subscription for sampletopic
az servicebus topic create -g $RESOURCE_GROUP --namespace-name $SB_NSP --name sampletopic
az servicebus topic subscription create -g $RESOURCE_GROUP --namespace-name $SB_NSP --topic-name sampletopic --name samplesubscription

# Create Topic and subscriptioon for demotopic
az servicebus topic create -g $RESOURCE_GROUP --namespace-name $SB_NSP --name demotopic
az servicebus topic subscription create -g $RESOURCE_GROUP --namespace-name $SB_NSP --topic-name demotopic --name demosubscription

# Create ACR instance
az acr create --location $LOCATION --name $CONTAINER_REGISTRY --resource-group $RESOURCE_GROUP --sku Basic

