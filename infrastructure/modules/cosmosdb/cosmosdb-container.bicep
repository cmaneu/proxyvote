@description('The name of your Cosmos DB Account')
param accountName string

@description('The name of your database')
param databaseName string

@description('The Azure region where all resources in this example should be created')
param location string

@description('The name of the Cosmos collection.')
param containerName string

@description('The path to the Partition key.')
param partitionKeyPath string = '/PartitionKey'

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2021-10-15-preview' existing = {
  name: accountName
}

resource  sqlDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-04-15' existing = {
  parent: databaseAccount
  name: databaseName
}

resource registrationsCollection 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-07-01-preview' = {
  name: containerName
  location: location
  parent: sqlDatabase
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          partitionKeyPath
        ]
        kind: 'Hash'
      }
    }
  }
}
