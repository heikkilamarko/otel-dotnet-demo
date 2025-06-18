variable "azure_subscription_id" {
  description = "Azure Subscription ID"
  type        = string
}

variable "location" {
  description = "The Azure region to deploy resources in. Example: West Europe"
  type        = string
}

variable "owner" {
  description = "Owner of the resources, used for tagging"
  type        = string
}

provider "azurerm" {
  features {
    log_analytics_workspace {
      permanently_delete_on_destroy = true
    }
  }
  subscription_id = var.azure_subscription_id
}

resource "azurerm_resource_group" "demo" {
  name     = "rg-otel-demo"
  location = var.location

  tags = {
    owner = var.owner
  }
}

resource "azurerm_log_analytics_workspace" "demo" {
  name                = "log-otel-demo"
  resource_group_name = azurerm_resource_group.demo.name
  location            = azurerm_resource_group.demo.location
}

resource "azurerm_application_insights" "demo" {
  name                = "appi-otel-demo"
  resource_group_name = azurerm_resource_group.demo.name
  location            = azurerm_resource_group.demo.location
  workspace_id        = azurerm_log_analytics_workspace.demo.id
  application_type    = "other"
  retention_in_days   = 30
}

output "application_insights_connection_string" {
  value     = azurerm_application_insights.demo.connection_string
  sensitive = true
}
