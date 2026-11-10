#!/usr/bin/env bash
set -euo pipefail
dotnet new sln -n MIS704.ArchitectureLab --format sln --force
find src tests -name '*.csproj' -print0 | while IFS= read -r -d '' f; do dotnet sln MIS704.ArchitectureLab.sln add "$f"; done
dotnet restore MIS704.ArchitectureLab.sln
dotnet build MIS704.ArchitectureLab.sln
