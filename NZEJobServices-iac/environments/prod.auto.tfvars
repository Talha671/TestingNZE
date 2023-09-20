app_info = {
  "app-nz-nzejobservices-stc-prod-aae" = {
    name                = "app-nz-nzejobservices-stc-prod-aae"
    resource_group_name = "rg-netzero-prod-stc-prod-aae"
    location            = "AustraliaEast"
    asp_name            = "asp-nz-platform-stc-prodsh-aae"
    asp_rg_name         = "rg-netzero-platform-stc-prodsh-aae"
    app_settings = {
      minTlsVersion = "1.2"
    }

    app_vnet_integration_info = {
      app_integration_network_subnet_name = "prod-stc-asp-subnet"
      app_integration_network_vnet_name   = "vnet-netzero-lz01-prod-spoke-australiaeast"
      app_integration_network_vnet_rgname = "rg-netzero-lz01-prod-network-australiaeast"
    }

    ip_restrictions = {
      # Dont use priority 100 its used for allow_apim -- via locals
    }
  }
}

apim_config = {
  apim_name     = "apm-nz-platform-prodsh-aae"
  apim_rgname   = "rg-netzero-platform-prodsh-aae"
  apim_api_name = "stc-job-service-apis"
}
tags = {
  Owner       = "Hus"
  BillingCode = "TBA"
  AppName     = "NZEJobServices"
  Environment = "preprod"
}

devops_vm_cidr = "10.100.96.0/28"