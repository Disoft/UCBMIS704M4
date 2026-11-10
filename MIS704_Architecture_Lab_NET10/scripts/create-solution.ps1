$ErrorActionPreference = 'Stop'
dotnet new sln -n MIS704.ArchitectureLab --format sln --force
Get-ChildItem -Path src,tests -Filter *.csproj -Recurse | ForEach-Object { dotnet sln MIS704.ArchitectureLab.sln add $_.FullName }
dotnet restore MIS704.ArchitectureLab.sln
dotnet build MIS704.ArchitectureLab.sln
