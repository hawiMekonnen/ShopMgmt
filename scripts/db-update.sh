#!/usr/bin/env bash
set -e
cd "$(dirname "$0")/.."
source scripts/setup-dotnet-path.sh

if ! dotnet ef --version >/dev/null 2>&1; then
  echo "Installing dotnet-ef tool..."
  dotnet tool install --global dotnet-ef
  export PATH="$HOME/.dotnet/tools:$PATH"
fi

dotnet ef database update --project ShopMgmt.Infrastructure/ShopMgmt.Infrastructure.csproj --startup-project ShopMgmt.WebAPI/ShopMgmt.WebAPI.csproj
