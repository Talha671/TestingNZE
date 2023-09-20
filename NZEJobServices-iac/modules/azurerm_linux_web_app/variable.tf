# Use variables to customize the deployment

variable "name" {
  type        = string
  description = "Specifies the name of the App Service. Changing this forces a new resource to be created."
}

variable "location" {
  description = "Specifies the supported Azure location where the resource exists. Changing this forces a new resource to be created."
  default     = "AustraliaEast"
}

variable "resource_group_name" {
  description = "The name of the resource group in which to create the App Service. Changing this forces a new resource to be created."
}

variable "tags" {
  description = "Tag information for the platform resources"
}

variable "app_service_plan_id" {
  description = "The ID of the App Service Plan within which to create this App Service."
}

variable "app_settings" {
  description = " A map of key-value pairs of App Settings."
  default     = {}
}

variable "app_vnet_integration_info" {

}

variable "vnet_route_all_enabled" {
  default = true
}
variable "ip_restrictions" {
  default = {}
}

variable "kind" {
  default = "linux,api"
}
variable "linuxFxVersion" {
  default = "DOTNETCORE|6.0"
}

variable "publicNetworkAccess" {
  default = "Enabled"
}