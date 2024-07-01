Write-Output "Llamas.Abstractions"
dotnet build src/Llamas.Abstractions/Llamas.Abstractions.csproj -c Release
dotnet pack src/Llamas.Abstractions/Llamas.Abstractions.csproj --include-source -c Release -o packed

Write-Output "Llamas"
dotnet build src/Llamas/Llamas.csproj -c Release
dotnet pack src/Llamas/Llamas.csproj --include-source -c Release -o packed

Write-Output "Llamas.Container"
dotnet build src/Llamas.Container/Llamas.Container.csproj -c Release
dotnet pack src/Llamas.Container/Llamas.Container.csproj --include-source -c Release -o packed