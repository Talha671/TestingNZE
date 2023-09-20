#test pipeline access
Param(

    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [ValidateSet('dev', 'test', 'preprod', 'prod')]
    [string]$environmentName =  "dev",

    [string]$StorageSKUName = "Standard_ZRS",

    [string]$StorageLocation = "AustraliaEast",

    [string]$ARM_SKIP_PROVIDER_REGISTRATION = "false",
    
    [string]$TFWORKSPACE = "NZFrontEndUI",

    [ValidateSet('Plan', 'Apply')]
    [string]$Plan_Or_Apply = "Plan",
    
    [string]$pool_name)

Set-Location "$env:BUILD_SOURCESDIRECTORY/$TFWORKSPACE"

dir env:
if ($pool_name -notlike "*EE-*") {
    $RequiredPSModules = @("Az.Accounts", "Az.Storage", "Az.Resources")

    Foreach ($item in $RequiredPSModules) {
        if ((get-module -ListAvailable | Where-Object { $_.Name -eq 'Az.Accounts' } | Measure-Object).count -eq 0) {
            Find-Module Az.Accounts | Install-Module -Force -Scope CurrentUser
            Find-Module Az.Storage | Install-Module -Force -Scope CurrentUser
            Find-Module Az.Resources | Install-Module -Force -Scope CurrentUser
        }
    }
}

#Core module doesnt have NP

$TFVARSFILEPATH = "$env:BUILD_SOURCESDIRECTORY" + "/" + "$TFWORKSPACE" + "/environments/" + "$environmentName" + ".auto.tfvars"

Import-Module Az.Accounts, Az.Resources
$SecurePassword = ConvertTo-SecureString “$ENV:ARM_CLIENT_SECRET" -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential("$ENV:ARM_CLIENT_ID", $SecurePassword)
Login-AzAccount -ServicePrincipal -Credential $credential -Tenant "$env:ARM_TENANT_ID" | Out-Null
Select-AzSubscription -Subscription "$env:TF_BACKEND_SUBSCRIPTION_ID" | Out-Null


$env:backendAzureRmKey = "$TFWORKSPACE" + "_" + $environmentName + "_" + "terraform.tfstate"
# https://learn.microsoft.com/en-us/azure/developer/terraform/store-state-in-azure-storage?tabs=azure-cli
$env:ARM_ACCESS_KEY = (Get-AzStorageAccountKey  -ResourceGroupName $env:BACKENDAZURERMRESOURCEGROUPNAME -Name $env:BACKENDAZURERMSTORAGEACCOUNTNAME | Select Value).Value[0]

Write-Host "Current Directory is $pwd"
terraform version
terraform init `
    -backend-config="storage_account_name=$env:BACKENDAZURERMSTORAGEACCOUNTNAME" `
    -backend-config="container_name=$env:BACKENDAZURERMCONTAINERNAME" `
    -backend-config="key=$env:backendAzureRmKey" `
    -backend-config="resource_group_name=$env:BACKENDAZURERMRESOURCEGROUPNAME"
$PlanPath = "$env:BUILD_ARTIFACTSTAGINGDIRECTORY" + "/main.tfplan"
$ApplyPath = "$env:BUILD_ARTIFACTSTAGINGDIRECTORY" + "/tfArtifact" + "/main.tfplan"

if($env:BUILD_SOURCEBRANCHNAME -EQ 'merge')
{
    $env:BUILD_SOURCEBRANCHNAME = 'merge-commit-' + $env:BUILD_SOURCEVERSION

}

if ($Plan_Or_Apply -eq "Plan") {
    terraform plan -var-file="$TFVARSFILEPATH" -var="git_tag=$env:BUILD_SOURCEBRANCHNAME" -var="git_repo_name=$env:BUILD_REPOSITORY_NAME"  `
    -out $PlanPath
}
elseif ($Plan_Or_Apply -eq "Apply") {
    terraform apply "$ApplyPath"
}