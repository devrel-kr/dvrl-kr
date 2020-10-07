$recordSet = Get-AzDnsRecordSet -name "www" -RecordType A -ZoneName "contoso.com" -ResourceGroupName "MyResourceGroup"
$rs.Records[0].Ipv4Address = "9.8.7.6"
Set-AzDnsRecordSet -RecordSet $rs



$rs = Get-AzDnsRecordSet -ResourceGroupName $ResourceGroupName -ZoneName $ZoneName -Name "@" -RecordType A
$rs.Records[0].Ipv4Address = $newIp4Address
Set-AzDnsRecordSet -RecordSet $rs
