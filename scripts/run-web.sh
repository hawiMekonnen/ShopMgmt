#!/usr/bin/env bash
set -e
cd "$(dirname "$0")/.."
source scripts/setup-dotnet-path.sh
dotnet run --project ShopMgmt.Web/ShopMgmt.Web.csproj
