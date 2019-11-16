docker pull mcr.microsoft.com/mssql/server:2019-GA-ubuntu-16.04
docker pull rabbitmq:3-management

cd ..\..\src\Services\Identity\Biz.BrightOnion.Identity.API
dotnet publish -c Release -o bin\Release\netcoreapp3.0\publish
docker image build -t biz_onion_identity_api .

cd ..\Biz.BrightOnion.Identity.BackgroundTasks
dotnet publish -c Release -o bin\Release\netcoreapp3.0\publish
docker image build -t biz_onion_identity_bgt .

cd ..\..\Catalog\Biz.BrightOnion.Catalog.API
dotnet publish -c Release -o bin\Release\netcoreapp3.0\publish
docker image build -t biz_onion_catalog_api .

cd ..\Biz.BrightOnion.Catalog.BackgroundTasks
dotnet publish -c Release -o bin\Release\netcoreapp3.0\publish
docker image build -t biz_onion_catalog_bgt .

cd ..\..\Ordering\Biz.BrightOnion.Ordering.API
dotnet publish -c Release -o bin\Release\netcoreapp2.2\publish
docker image build -t biz_onion_order_api .

cd ..\Biz.BrightOnion.Ordering.SignalrHub
dotnet publish -c Release -o bin\Release\netcoreapp2.2\publish
docker image build -t biz_onion_order_hub .

cd ..\..\..\ApiGateways\Biz.BrightOnion.ApiGateway
dotnet publish -c Release -o bin\Release\netcoreapp2.2\publish
docker image build -t biz_onion_gw .

cd ..\Ordering\Biz.BrightOnion.ApiGateway.OrderingAggregator
dotnet publish -c Release -o bin\Release\netcoreapp2.2\publish
docker image build -t biz_onion_order_gw .

cd ..\..\..\Web\Biz.BrightOnion.Web
dotnet publish -c Release -o bin\Release\netcoreapp2.2\publish
docker image build -t biz_onion_web .
