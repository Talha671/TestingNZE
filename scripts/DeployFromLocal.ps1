#test pipeline access
Param(

    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [ValidateSet('dev', 'test', 'preprod', 'prod')]
    [string]$environmentName = "dev",

    [ValidateSet('Plan', 'Apply')]
    [string]$Plan_Or_Apply = "Plan",

    [string]$StorageSKUName = "Standard_ZRS",

    [string]$StorageLocation = "AustraliaEast",

    [string]$module = "NZEJobServices-iac")

Set-Location "$module"

switch ($environmentName) {
    "dev" {
        $ENV:ARM_SUBSCRIPTION_ID = "db3a7319-e4d7-4d72-afd9-f23cb2f79c5b"
        $env:TF_BACKEND_SUBSCRIPTION_ID = "6009765e-2b9a-4d58-9886-eaab5afa4dcd"
    }
    "test" {
        $ENV:ARM_SUBSCRIPTION_ID = "db3a7319-e4d7-4d72-afd9-f23cb2f79c5b"
        $env:TF_BACKEND_SUBSCRIPTION_ID = "6009765e-2b9a-4d58-9886-eaab5afa4dcd"
    }
    
    "preprod" {
        $ENV:ARM_SUBSCRIPTION_ID = "c1425426-f14a-4c74-82de-f2fa048d503d"
        $env:TF_BACKEND_SUBSCRIPTION_ID = "8c61d7a5-1ac8-4322-a8ca-f5177159a2d2"
    }    
    "prod" {
        $ENV:ARM_SUBSCRIPTION_ID = "c1425426-f14a-4c74-82de-f2fa048d503d"
        $env:TF_BACKEND_SUBSCRIPTION_ID = "8c61d7a5-1ac8-4322-a8ca-f5177159a2d2"
    }
    
}
$TFVARSFILEPATH = "./environments/" + "$environmentName" + ".auto.tfvars"
if ( (Get-Module -ListAvailable | where { $_.name -like "*yaml*" } | Measure-Object).count -eq 0) {
    Install-Module PowerShell-yaml -Force
}
Select-AzSubscription -Subscription "$env:TF_BACKEND_SUBSCRIPTION_ID" #| Out-Null

$BackendInfoYaml = get-content ..\variables\pipeline-variables-$environmentName.yml | ConvertFrom-Yaml

$env:backendAzureRmResourceGroupName = ($BackendInfoYaml | ConvertTo-Json | ConvertFrom-Json | Select-Object Variables).variables.backendAzureRmResourceGroupName
$env:backendAzureRmStorageAccountName = ($BackendInfoYaml | ConvertTo-Json | ConvertFrom-Json | Select-Object Variables).variables.backendAzureRmStorageAccountName
$env:backendAzureRmKey = ($BackendInfoYaml | ConvertTo-Json | ConvertFrom-Json | Select-Object Variables).variables.backendAzureRmKey
$env:backendAzureRmContainerName = ($BackendInfoYaml | ConvertTo-Json | ConvertFrom-Json | Select-Object Variables).variables.backendAzureRmContainerName
$env:backendAzureRmKeyUpd = $module + "_" + $environmentName + "_" + "$env:backendAzureRmKey"

if ((Get-AzStorageAccount -Name $env:BACKENDAZURERMSTORAGEACCOUNTNAME -ResourceGroupName $env:BACKENDAZURERMRESOURCEGROUPNAME -ErrorAction SilentlyContinue | Measure).Count -eq 0) {
    New-AzStorageAccount -ResourceGroupName $env:BACKENDAZURERMRESOURCEGROUPNAME -Name $env:BACKENDAZURERMSTORAGEACCOUNTNAME -SkuName $StorageSKUName  -Location $StorageLocation
}

Set-AzCurrentStorageAccount -Name $env:BACKENDAZURERMSTORAGEACCOUNTNAME -ResourceGroupName $env:BACKENDAZURERMRESOURCEGROUPNAME
if ((Get-AzStorageContainer -Name $env:backendAzureRmContainerName -ErrorAction SilentlyContinue | Measure).Count -eq 0) {
    New-AzStorageContainer -Name $env:backendAzureRmContainerName
}

$env:ARM_ACCESS_KEY = (Get-AzStorageAccountKey  -ResourceGroupName $env:BACKENDAZURERMRESOURCEGROUPNAME -Name $env:BACKENDAZURERMSTORAGEACCOUNTNAME | Select Value).Value[0]
terraform version

terraform init `
    -backend-config="storage_account_name=$env:backendAzureRmStorageAccountName" `
    -backend-config="container_name=$env:backendAzureRmContainerName" `
    -backend-config="key=$env:backendAzureRmKeyUpd" `
    -backend-config="resource_group_name=$env:backendAzureRmResourceGroupName" -reconfigure

$PlanPath = "./main.tfplan"
$ApplyPath = "./main.tfplan"
if ($Plan_Or_Apply -eq "Plan") {
    terraform plan -var-file="$TFVARSFILEPATH" -out $PlanPath
}
elseif ($Plan_Or_Apply -eq "Apply") {
    #terraform apply "$ApplyPath"
    terraform apply -auto-approve -var-file="$TFVARSFILEPATH"
}

Remove-Item *.tfplan -Force
Set-Location ..