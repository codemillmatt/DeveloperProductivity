param location string
param principalId string = ''
param resourceToken string
param tags object
param storageSku string = 'Standard_RAGRS' 
param appServiceSku string = 'S1'

resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: '${resourceToken}storage'
  kind: 'StorageV2'
  location: location
  sku: {
    name: storageSku
  }
  tags: tags
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  location: location
  name: '${resourceToken}-app-plan'
  tags: tags
  sku: {
    name: appServiceSku
  }
  kind: 'windows'
}

resource web 'Microsoft.Web/sites@2022-03-01' = {
  name: '${resourceToken}-web'
  location: location
  kind: 'app'
  tags: union(tags, {
    'azd-service-name': 'dev-productivity-api'
  })
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v6.0'
    }
  }

  resource appSettings 'config' = {
    name: 'appsettings'
    properties: {
      'SCM_DO_BUILD_DURING_DEPLOYMENT': 'true'
      'storage-connection-string:queue': 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storage.id, storage.apiVersion).keys[0].value}'
      'storage-connection-string:blob': 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storage.id, storage.apiVersion).keys[0].value}'
    }
  }
}

output WEB_URI string = 'https://${web.properties.defaultHostName}'
