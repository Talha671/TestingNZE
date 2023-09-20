app_info = {
  "app-nz-nzejobservices-stc-dev-aae" = {
    name                = "app-nz-nzejobservices-stc-dev-aae"
    resource_group_name = "rg-netzero-nonprod-stc-dev-aae"
    location            = "AustraliaEast"
    asp_name            = "asp-nz-platform-stc-nonprod-aae"
    asp_rg_name         = "rg-netzero-platform-stc-nonprod-aae"
    app_settings = {
      minTlsVersion = "1.2"
    }

    app_vnet_integration_info = {
      app_integration_network_subnet_name = "nonprod-stc-asp-subnet"
      app_integration_network_vnet_name   = "vnet-netzero-lz01-nonprod-spoke-australiaeast"
      app_integration_network_vnet_rgname = "rg-netzero-lz01-nonprod-network-australiaeast"
    }

    ip_restrictions = {
      # Dont use priority 100 its used for allow_apim -- via locals
    }
  }
}

tags = {
  Owner       = "Hus"
  BillingCode = "TBA"
  AppName     = "NZEJobServices"
  Environment = "dev"
}
apim_config = {
  apim_name   = "apm-nz-platform-np-aae"
  apim_rgname = "rg-netzero-platform-nonprod-aae"
}

devops_vm_cidr = "10.50.96.0/28"