
variable "app_info" {
  description = "APP info such as a name, ASP etc."
}

variable "tags" {
  description = "Tags for the Platform Service Components"
}

variable "git_tag" {
  default = "develop"
}

variable "git_repo_name" {
  default = "Netzero-Environmental-Group/NZEJobServices"
}
variable "apim_config" {

}

variable "devops_vm_cidr" {

}