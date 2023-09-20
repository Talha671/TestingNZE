# Configure Terraform to set the required AzureRM provider
# version and features{} block.

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.35.0"
    }
  }
  backend "azurerm" {

  }
}

provider "azurerm" {
  features {}
}

data "azurerm_service_plan" "asp" {
  for_each = var.app_info

  name                = each.value.asp_name
  resource_group_name = each.value.asp_rg_name
}

module "apps" {
  for_each = var.app_info

  source = "./modules/azurerm_linux_web_app"

  location                  = each.value.location
  name                      = each.value.name
  resource_group_name       = each.value.resource_group_name
  app_service_plan_id       = data.azurerm_service_plan.asp[each.value.name].id
  app_settings              = each.value.app_settings
  tags                      = merge(var.tags, local.additional_tags)
  app_vnet_integration_info = each.value.app_vnet_integration_info
  ip_restrictions           = merge(each.value.ip_restrictions, local.apim_ip_restrictions)
}


data "azurerm_api_management" "apim" {
  name                = var.apim_config.apim_name
  resource_group_name = var.apim_config.apim_rgname
}

locals {
  additional_tags = {
    gitTagNameIAC     = var.git_tag
    gitRepositoryName = var.git_repo_name
  }
  apim_public_ip = data.azurerm_api_management.apim.public_ip_addresses.0
  apim_ip_restrictions = {
    "allow_apim" = {
      ipAddress   = "${local.apim_public_ip}/32"
      action      = "Allow"
      tag         = "Default"
      priority    = 100
      name        = "allow_apim"
      description = "Allow from APIM"
    }
    "allow_devopsvm" = {
      ipAddress   = "${var.devops_vm_cidr}"
      action      = "Allow"
      tag         = "Default"
      priority    = 110
      name        = "allow_devopsvm"
      description = "Allow from DevOps VM"
    }
  }
}
