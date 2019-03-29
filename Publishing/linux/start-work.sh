app_env=PresentationWork

cd Web
export ASPNETCORE_ENVIRONMENT=$app_env
export ASPNETCORE_URLS=http://+:5000
export ASPNETCORE_HTTP_PORT=5000
dotnet Biz.BrightOnion.Web.dll &
cd ..

cd ApiGateway
export ASPNETCORE_ENVIRONMENT=$app_env
export ASPNETCORE_URLS=http://+:9000
export ASPNETCORE_HTTP_PORT=9000
dotnet Biz.BrightOnion.ApiGateway.dll &
cd ..

cd ApiGateway.OrderingAggregator
export ASPNETCORE_ENVIRONMENT=$app_env
export ASPNETCORE_URLS=http://+:9010
export ASPNETCORE_HTTP_PORT=9010
dotnet Biz.BrightOnion.ApiGateway.OrderingAggregator.dll &
cd ..

cd Services/Identity.API
export ASPNETCORE_ENVIRONMENT=$app_env
export ASPNETCORE_URLS=http://+:7001
export ASPNETCORE_HTTP_PORT=7001
dotnet Biz.BrightOnion.Identity.API.dll &
cd ../..

cd Services/Identity.BackgroundTasks
export ASPNETCORE_ENVIRONMENT=$app_env
export ASPNETCORE_URLS=http://+:7011
export ASPNETCORE_HTTP_PORT=7011
dotnet Biz.BrightOnion.Identity.BackgroundTasks.dll &
cd ../..

cd Services/Catalog.API
export ASPNETCORE_ENVIRONMENT=$app_env
export ASPNETCORE_URLS=http://+:7002
export ASPNETCORE_HTTP_PORT=7002
dotnet Biz.BrightOnion.Catalog.API.dll &
cd ../..

cd Services/Catalog.BackgroundTasks
export ASPNETCORE_ENVIRONMENT=$app_env
export ASPNETCORE_URLS=http://+:7012
export ASPNETCORE_HTTP_PORT=7012
dotnet Biz.BrightOnion.Catalog.BackgroundTasks.dll &
cd ../..

cd Services/Ordering.API
export ASPNETCORE_ENVIRONMENT=$app_env
export ASPNETCORE_URLS=http://+:7003
export ASPNETCORE_HTTP_PORT=7003
dotnet Biz.BrightOnion.Ordering.API.dll &
cd ../..
