#!/bin/bash
set -euo pipefail

applicationinsights_connection_string="$(terraform -chdir=infra output -raw application_insights_connection_string)"

sed -i "" -e "s|APPLICATIONINSIGHTS_CONNECTION_STRING=.*|APPLICATIONINSIGHTS_CONNECTION_STRING=$applicationinsights_connection_string|" "env/otel-collector.env"
