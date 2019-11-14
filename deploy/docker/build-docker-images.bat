cd ..\..\src\Services\Identity\Biz.BrightOnion.Identity.API
dotnet publish -c Release -o bin\Release\netcoreapp3.0\publish
docker image build -t biz_onion_identity_api .

cd ..\Biz.BrightOnion.Identity.BackgroundTasks
dotnet publish -c Release -o bin\Release\netcoreapp3.0\publish
docker image build -t biz_onion_identity_bgt .

cd ..\Catalog\Biz.BrightOnion.Catalog.API
dotnet publish -c Release -o bin\Release\netcoreapp3.0\publish
docker image build -t biz_onion_catalog_api .

cd ..\Biz.BrightOnion.Catalog.BackgroundTasks
dotnet publish -c Release -o bin\Release\netcoreapp3.0\publish
docker image build -t biz_onion_catalog_bgt .
