targetScope = 'subscription'

@minLength(1)
@maxLength(50)
@description('Name of the environment - used to create a short unique hash for all resources.')
param name string

@minLength(1)
@description('Primary location for all resources')
param location string

@description('Id of the user or app to assign roles')
param principalId string = ''

var resourceToken = toLower(uniqueString(subscription().id, name))
var tags = {
    'azd-env-name': name
}

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${name}-rg'
  location: location
  tags: tags
}

module resources './resources.bicep' = {
  name: 'resources-${resourceToken}'
  scope: resourceGroup
  params: {
    location: location
    principalId: principalId
    resourceToken: resourceToken
    tags: tags
  }
}

output AZURE_LOCATION string = location
output APP_WEB_BASE_URL string = resources.outputs.WEB_URI
