# Terraform doesnt support API app and deploying as webapp cant be handled from APIM. 

resource "azurerm_resource_group_template_deployment" "apiapp" {
  name                = "apiapp"
  resource_group_name = var.resource_group_name
  deployment_mode     = "Incremental"
  parameters_content = jsonencode({
    "app_name" = {
      value = var.name
    }
    "app_asp_id" = {
      value = var.app_service_plan_id
    }
    "location" = {
      value = var.location
    }
    "kind" = {
      value = var.kind
    }
    "linuxFxVersion" = {
      value = var.linuxFxVersion
    }
    "publicNetworkAccess" = {
      value = var.publicNetworkAccess
    }
    "ipSecurityRestrictions" = {
      value = local.ip_restrictions_list
    }
    "minTlsVersion" = {
      value = var.app_settings.minTlsVersion
    }
    "tags" = {
      value = var.tags
    }
  })
  template_content = file("${path.module}/../arm/template.json")

}

data "azurerm_resource_group" "vnet_rg" {
  name = var.app_vnet_integration_info.app_integration_network_vnet_rgname
}

locals {
  private_subnet_id_vnet_integration = "${data.azurerm_resource_group.vnet_rg.id}/providers/Microsoft.Network/virtualNetworks/${var.app_vnet_integration_info.app_integration_network_vnet_name}/subnets/${var.app_vnet_integration_info.app_integration_network_subnet_name}"
  ip_restrictions_list = [for r in var.ip_restrictions :
    {
      ipAddress   = "${r.ipAddress}"
      action      = "${r.action}"
      tag         = "${r.tag}"
      priority    = "${r.priority}"
      name        = "${r.name}"
      description = "${r.description}"
    }
  ]

}

