// Resource name
param name string

// Provisioning environment
param env string {
    allowed: [
        'common'
    ]
    default: 'common'
}

// Resource location
param location string = resourceGroup().location

// Resource location code
param locationCode string = 'krc'

// Storage CosmosAccount
param storageAccountSku string = 'Standard_LRS'

// Function App
param functionAppWorkerRuntime string = 'dotnet'
param functionAppEnvironment string {
    allowed: [
        'Production'
    ]
    default: 'Production'
}
param functionAppTimezone string = 'Korea Standard Time'

// ACMEbot
param acmebotEmailAddress string
param acmebotEndpoint string = 'https://acme-v02.api.letsencrypt.org/'
param acmebotArtifactUrl string = 'https://shibayan.blob.${environment().suffixes.storage}/azure-keyvault-letsencrypt/v3/latest.zip'

// Key Vault
param keyVaultSku string {
    allowed: [
        'Standard'
        'Premium'
    ]
    default: 'Standard'
}

var metadata = {
    longName: '{0}-${name}-${env}-${locationCode}'
    shortName: '{0}${name}${env}${locationCode}'
}

var storage = {
    name: format(metadata.shortName, 'st')
    location: location
    sku: storageAccountSku
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

var workspace = {
    name: format(metadata.longName, 'wrkspc')
    location: location
}

resource wrkspc 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
    name: workspace.name
    location: workspace.location
    properties: {
        sku: {
            name: 'PerGB2018'
        }
        retentionInDays: 30
        workspaceCapping: {
            dailyQuotaGb: -1
        }
        publicNetworkAccessForIngestion: 'Enabled'
        publicNetworkAccessForQuery: 'Enabled'
    }
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
        Flow_Type: 'Bluefield'
        Application_Type: 'web'
        Request_Source: 'rest'
        WorkspaceResourceId: wrkspc.id
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
        reserved: true
    }
}

var functionApp = {
    name: format(metadata.longName, 'fncapp')
    location: location
    environment: functionAppEnvironment
    runtime: functionAppWorkerRuntime
    timezone: functionAppTimezone
    acmebot: {
        email: acmebotEmailAddress
        endpoint: acmebotEndpoint
        artifact: acmebotArtifactUrl
    }
}

resource fncapp 'Microsoft.Web/sites@2020-06-01' = {
    name: functionApp.name
    location: functionApp.location
    kind: 'functionapp'
    identity: {
        type: 'SystemAssigned'
    }
    properties: {
        serverFarmId: csplan.id
        httpsOnly: true
        siteConfig: {
            appSettings: [
                {
                    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                    value: '${reference(appins.id, '2020-02-02-preview', 'Full').properties.InstrumentationKey}'
                }
                {
                    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
                    value: '${reference(appins.id, '2020-02-02-preview', 'Full').properties.connectionString}'
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
                // ACMEbot specific
                {
                    name: 'WEBSITE_RUN_FROM_PACKAGE'
                    value: functionApp.acmebot.artifact
                }
                {
                    name: 'Acmebot:AzureDns:SubscriptionId'
                    value: subscription().subscriptionId
                }
                {
                    name: 'Acmebot:Contacts'
                    value: functionApp.acmebot.email
                }
                {
                    name: 'Acmebot:Endpoint'
                    value: functionApp.acmebot.endpoint
                }
                {
                    name: 'Acmebot:VaultBaseUrl'
                    value: 'https://${keyVault.name}${environment().suffixes.keyvaultDns}'
                }
                {
                    name: 'Acmebot:Environment'
                    value: environment().name
                }
            ]
        }
    }
}

// resource funcappMetadata 'Microsoft.Web/sites/config@2020-06-01' = {
//     name: '${fncapp.name}/metadata'
//     properties: {
//         synctriggersstatus: listsyncfunctiontriggerstatus(fncapp.id, '2020-06-01').status
//     }
// }

var keyVault = {
    name: format(metadata.shortName, 'kv')
    location: location
    sku: {
        name: keyVaultSku
        family: 'A'
    }
}

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' = {
    name: keyVault.name
    location: keyVault.location
    properties: {
        sku: keyVault.sku
        tenantId: subscription().tenantId
        accessPolicies: [
            {
                tenantId: subscription().tenantId
                objectId: '${reference(fncapp.id, '2020-06-01', 'Full').identity.principalId}'
                permissions: {
                    certificates: [
                        'get'
                        'list'
                        'create'
                        'update'
                    ]
                }
            }
        ]
    }
}
