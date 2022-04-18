targetScope = 'subscription'

// If an environment is set up (dev, test, prod...), it is used in the application name
param environment string = 'dev'
param applicationName string = 'proxyvote'
param location string = 'francecentral'
var instanceNumber = '001'

var defaultTags = {
  'environment': environment
  'application': applicationName
  'nubesgen-version': '0.8.1'
}

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'rg-${applicationName}-${environment}-${instanceNumber}'
  location: location
  tags: defaultTags
}

module cosmos 'modules/cosmosdb/cosmosdb.bicep' = {
  name: 'cosmosdb'
  scope: resourceGroup(rg.name)
  params: {
    location: location
    applicationName: applicationName
    environment: environment
    tags: defaultTags
    instanceNumber: instanceNumber
    containerName: 'Registrations'
    indexingPolicy: {
      indexingMode: 'consistent'
      excludedPaths: [
        {
          path: '/*'
        }
      ]
      includedPaths: [
        { 
          path: '/RegistrationId/*'
        }
        { 
          path: '/Applicant/LastName/*'
        }
      ]
    }
  }
}

module cosmosRegistrationContainer 'modules/cosmosdb/cosmosdb-container.bicep' = {
  name: 'Registrations'
  scope: resourceGroup(rg.name)
  dependsOn: [
    cosmos
  ]
  params: {
    accountName: cosmos.outputs.AccountName
    location: location
    containerName: 'Registrations'
    databaseName: applicationName
  }
}

var applicationEnvironmentVariables = [
  {
    name: 'CosmosDb__Endpoint'
    value: cosmos.outputs.Endpoint
  }
  {
    name: 'CosmosDb__AccessKey'
    value: cosmos.outputs.AccountKey
  }
]

module function 'modules/function/function.bicep' = {
  name: 'function'
  scope: resourceGroup(rg.name)
  params: {
    location: location
    applicationName: applicationName
    environment: environment
    resourceTags: defaultTags
    instanceNumber: instanceNumber
    environmentVariables: applicationEnvironmentVariables
  }
}

output functionUrl string = function.outputs.application_url
output functionName string = function.outputs.application_name
