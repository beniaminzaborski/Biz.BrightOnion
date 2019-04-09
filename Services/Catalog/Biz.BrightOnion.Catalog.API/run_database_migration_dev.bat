rem ### Install global tool: ###
rem ### dotnet tool install --global FluentMigrator.DotNet.Cli --version 3.1.2 ###

dotnet publish
cd .\bin\Debug\netcoreapp2.2\publish\
rem dotnet fm migrate -p SqlServer2014 -c "Server=.\SQLEXPRESS;Database=BrightOnionCatalog;Trusted_Connection=True;" -a Biz.BrightOnion.Catalog.API.dll
dotnet Biz.BrightOnion.Catalog.API.dll dm
cd ..\..\..\