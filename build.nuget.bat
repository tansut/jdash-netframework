rmdir nupkgs /s /q  

cd util
nuget pack ../JDash.NetFramework.Api/JDash.NetFramework.Api.nuspec  -properties Configuration=Release -OutputDirectory ../nupkgs 
nuget pack ../JDash.NetFramework.Models/JDash.NetFramework.Models.nuspec  -properties Configuration=Release -OutputDirectory ../nupkgs
nuget pack ../JDash.NetFramework.Provider.MsSQL/JDash.NetFramework.Provider.MsSQL.nuspec  -properties Configuration=Release -OutputDirectory ../nupkgs 
nuget pack ../JDash.NetFramework.Provider.MySQL/JDash.NetFramework.Provider.MySQL.nuspec  -properties Configuration=Release -OutputDirectory ../nupkgs 

@echo off
nuget.exe setApiKey [e716bcbf-088d-4f61-8fe5-4c16fba27457] -source https://www.nuget.org
@echo on
for %%a in ("..\nupkgs\*symbols.nupkg") do (
	del %%a
)
for %%a in ("..\nupkgs\*.nupkg") do (
	nuget.exe push %%a e716bcbf-088d-4f61-8fe5-4c16fba27457 -Source https://www.nuget.org/api/v2/package
)
pause