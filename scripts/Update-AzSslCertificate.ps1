Param(
    [string] [Parameter(Mandatory=$true)] $ApiEndpoint,
    [string[]] [Parameter(Mandatory=$true)] $DnsNames
)

# Issue new SSL certificate
$body = @{ DnsNames = $DnsNames }
Invoke-RestMethod -Method Post -Uri $ApiEndpoint -ContentType "application/json" -Body ($body | ConvertTo-Json)

Write-Output "New SSL certificate has been issued to $($DnsNames -join ', ')"
