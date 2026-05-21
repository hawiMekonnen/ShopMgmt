#!/usr/bin/env bash
# Git Bash on Windows often picks up the 32-bit dotnet shim (x86), which has NO SDKs.
# Source this file before running dotnet commands:
#   source scripts/setup-dotnet-path.sh

export DOTNET_ROOT="/c/Program Files/dotnet"
export PATH="$DOTNET_ROOT:$PATH"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "ERROR: dotnet not found. Install .NET SDK 10: https://dotnet.microsoft.com/download"
  return 1 2>/dev/null || exit 1
fi

echo "Using: $(command -v dotnet)"
dotnet --version
dotnet --list-sdks
