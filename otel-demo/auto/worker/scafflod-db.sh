#!/bin/bash
set -euo pipefail

dotnet ef dbcontext scaffold \
  Name=ConnectionStrings:Demo \
  Npgsql.EntityFrameworkCore.PostgreSQL \
  --context DemoContext \
  --context-dir Data \
  --output-dir Data \
  --use-database-names \
  --no-build \
  --no-onconfiguring \
  --no-pluralize \
  --force \
  --table demo.messages
