// Resource name
param name string

// Provisioning environment
param env string {
    allowed: [
        'dev'
        'test'
        'prod'
    ]
    default: 'dev'
}

// Resource location
param location string = resourceGroup().location

// Resource location code
param locationCode string = 'krc'

// Service Principal
param servicePrincipalClientId string {
    secure: true
}
param servicePrincipalClientSecret string {
    secure: true
}

// Cosmos DB
param cosmosDbDefaultConsistencyLevel string = 'Session'
param cosmosDbPrimaryRegion string = 'Korea Central'
param cosmosDbAutomaticFailover bool = true
param cosmosDbName string
param cosmosDbContainerName string
param cosmosDbPartitionKeyPath string

// Storage CosmosAccount
param storageAccountSku string = 'Standard_LRS'
param storageContainerLetsEncryptChallenge string = 'letsencrypt-challenge'

// Function App
param functionAppWorkerRuntime string = 'dotnet'
param functionAppEnvironment string {
    allowed: [
        'Development'
        'Staging'
        'Production'
    ]
    default: 'Development'
}
param functionAppTimezone string = 'Korea Standard Time'
param functionAppCustomDomain string

// dvrl.kr
param dvrlDefaultRedirectUrl string
param dvrlFilesToBeIgnored string = 'favicon.ico'
param dvrlGoogleAnalyticsCode string {
    secure: true
}
param dvrlUrlShortenerLength int = 10

var metadata = {
    longName: '{0}-${name}-${env}-${locationCode}'
    shortName: '{0}${name}${env}${locationCode}'
}

var cosmosDb = {
    name: format(metadata.longName, 'cosdba')
    location: location
    enableAutomaticFailover: cosmosDbAutomaticFailover
    consistencyPolicy: {
        defaultConsistencyLevel: cosmosDbDefaultConsistencyLevel
    }
    region: {
        primary: cosmosDbPrimaryRegion
    }
    dbName: cosmosDbName
    containerName: cosmosDbContainerName
    partitionKeyPath: cosmosDbPartitionKeyPath
}

resource cosdba 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
    name: cosmosDb.name
    location: cosmosDb.location
    kind: 'GlobalDocumentDB'
    tags: {
        defaultExperience: 'Core (SQL)'
        CosmosAccountType: 'Non-Production'
    }
    properties: {
        databaseAccountOfferType: 'Standard'
        enableAutomaticFailover: cosmosDb.enableAutomaticFailover
        consistencyPolicy: {
            defaultConsistencyLevel: cosmosDb.consistencyPolicy.defaultConsistencyLevel
            maxIntervalInSeconds: 5
            maxStalenessPrefix: 100
        }
        locations: [
			{
				locationName: cosmosDb.region.primary
				failoverPriority: 0
				isZoneRedundant: false
            }
        ]
        capabilities: [
            {
                name: 'EnableServerless'
            }
        ]
        backupPolicy: {
            type: 'Periodic'
            periodicModeProperties: {
                backupIntervalInMinutes: 240
                backupRetentionIntervalInHours: 8
            }
        }
    }
}

resource cosdbaSqlDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2020-06-01-preview' = {
    name: '${cosdba.name}/${cosmosDb.dbName}'
    location: cosmosDb.location
    properties: {
        resource: {
            id: cosmosDb.dbName
        }
    }
}

resource cosdbaContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2020-06-01-preview' = {
    name: '${cosdbaSqlDb.name}/${cosmosDb.containerName}'
    location: cosmosDb.location
    properties: {
        resource: {
            id: cosmosDb.containerName
            partitionKey: {
                kind: 'Hash'
                paths: [
                    cosmosDb.partitionKeyPath
                ]
            }
        }
    }
}

var storage = {
    name: format(metadata.shortName, 'st')
    location: location
    sku: storageAccountSku
    letsEncryptChallenge: storageContainerLetsEncryptChallenge
}

resource st 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storage.name
    location: storage.location
    kind: 'StorageV2'
    sku: {
        name: storage.sku
    }
    properties: {
        supportsHttpsTrafficOnly: true
    }
}

resource stBlob 'Microsoft.Storage/storageAccounts/blobServices@2019-06-01' = {
    name: '${st.name}/default'
}

resource stBlobLetsEncrypt 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
    name: '${stBlob.name}/${storage.letsEncryptChallenge}'
}

var appInsights = {
    name: format(metadata.longName, 'appins')
    location: location
}

resource appins 'Microsoft.Insights/components@2020-02-02-preview' = {
    name: appInsights.name
    location: appInsights.location
    kind: 'web'
    properties: {
        Application_Type: 'web'
        Request_Source: 'IbizaWebAppExtensionCreate'
    }
}

var servicePlan = {
    name: format(metadata.longName, 'csplan')
    location: location
}

resource csplan 'Microsoft.Web/serverfarms@2019-08-01' = {
    name: servicePlan.name
    location: servicePlan.location
    sku: {
        name: 'Y1'
        tier: 'Dynamic'
    }
    properties: {
        name: servicePlan.name
        computeMode: 'Dynamic'
    }
}

var functionApp = {
    name: format(metadata.longName, 'fncapp')
    location: location
    environment: functionAppEnvironment
    runtime: functionAppWorkerRuntime
    timezone: functionAppTimezone
    hostname: functionAppCustomDomain
}

var servicePrincipal = {
    clientId: servicePrincipalClientId
    clientSecret: servicePrincipalClientSecret
}

var dvrlkr = {
    defaultRedirectUrl: dvrlDefaultRedirectUrl
    filesToBeIgnored: dvrlFilesToBeIgnored
    googleAnalyticsCode: dvrlGoogleAnalyticsCode
    urlShortenerLength: dvrlUrlShortenerLength
}

resource fncapp 'Microsoft.Web/sites@2020-06-01' = {
    name: functionApp.name
    location: functionApp.location
    kind: 'functionapp'
    properties: {
        serverFarmId: csplan.id
        httpsOnly: true
        alwaysOn: true
        siteConfig: {
            appSettings: [
                {
                    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                    value: '${reference(appins.id, '2018-05-01-preview', 'Full').properties.InstrumentationKey}'
                }
                {
                    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
                    value: '${reference(appins.id, '2018-05-01-preview', 'Full').properties.connectionString}'
                }
                {
                    name: 'AZURE_FUNCTIONS_ENVIRONMENT'
                    value: functionApp.environment
                }
                {
                    name: 'AzureWebJobsStorage'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'FUNCTIONS_EXTENSION_VERSION'
                    value: '~3'
                }
                {
                    name: 'FUNCTION_APP_EDIT_MODE'
                    value: 'readonly'
                }
                {
                    name: 'FUNCTIONS_WORKER_RUNTIME'
                    value: functionApp.runtime
                }
                {
                    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'WEBSITE_CONTENTSHARE'
                    value: functionApp.name
                }
                {
                    name: 'WEBSITE_NODE_DEFAULT_VERSION'
                    value: '~10'
                }
                {
                    name: 'WEBSITE_TIME_ZONE'
                    value: functionApp.timezone
                }
                {
                    name: 'CosmosDBConnection'
                    value: 'AccountEndpoint=https://${cosdba.name}.documents.azure.com:443/;AccountKey=${listKeys(cosdba.id, '2020-06-01-preview').primaryMasterKey};'
                }
                // Let's Encrypt related settings
                {
                    name: 'AzureWebJobsDashboard'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'AzureWebJobsStorageLetsEncryptChallengeUrl'
                    value: 'https://${st.name}.blob.${environment().suffixes.storage}/${storage.letsEncryptChallenge}'
                }
                {
                    name: 'letsencrypt:Tenant'
                    value: '${subscription().tenantId}'
                }
                {
                    name: 'letsencrypt:SubscriptionId'
                    value: '${subscription().subscriptionId}'
                }
                {
                    name: 'letsencrypt:ClientId'
                    value: servicePrincipal.clientId
                }
                {
                    name: 'letsencrypt:ClientSecret'
                    value: servicePrincipal.clientSecret
                }
                {
                    name: 'letsencrypt:ResourceGroupName'
                    value: '${resourceGroup().name}'
                }
                {
                    name: 'letsencrypt:ServicePlanResourceGroupName'
                    value: '${resourceGroup().name}'
                }
                {
                    name: 'letsencrypt:UseIPBasedSSL'
                    value: false
                }
                {
                    name: 'letsencrypt:AuthorizationChallengeBlobStorageAccount'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'letsencrypt:AuthorizationChallengeBlobStorageContainer'
                    value: storage.letsEncryptChallenge
                }
                // dvrl.kr specific settings
                {
                    name: 'DefaultRedirectUrl'
                    value: dvrlkr.defaultRedirectUrl
                }
                {
                    name: 'FilesToBeIgnored'
                    value: dvrlkr.filesToBeIgnored
                }
                {
                    name: 'GoogleAnalyticsCode'
                    value: dvrlkr.googleAnalyticsCode
                }
                {
                    name: 'ShortenUrl__Hostname'
                    value: functionApp.hostname
                }
                {
                    name: 'ShortenUrl__Length'
                    value: dvrlkr.urlShortenerLength
                }
                {
                    name: 'CosmosDb__DatabaseName'
                    value: cosmosDb.dbName
                }
                {
                    name: 'CosmosDb__ContainerName'
                    value: cosmosDb.containerName
                }
                {
                    name: 'CosmosDb__PartitionKeyPath'
                    value: cosmosDb.partitionKeyPath
                }
            ]
        }
    }
}

resource fncappHostname 'Microsoft.Web/sites/hostNameBindings@2020-06-01' = {
    name: '${fncapp.name}/${functionApp.hostname}'
    location: functionApp.location
}

resource fncappLetsencrypt 'Microsoft.Web/sites/siteextensions@2020-06-01' = {
    name: '${fncapp.name}/letsencrypt'
    location: functionApp.location
    properties: {
        key1: 'value1'
    }
}
