$app_env="PresentationHome"

cd Web
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:5000"
$Env:ASPNETCORE_HTTP_PORT=5000
start dotnet Biz.BrightOnion.Web.dll
cd ..

cd ApiGateway
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:9000"
$Env:ASPNETCORE_HTTP_PORT=9000
start dotnet Biz.BrightOnion.ApiGateway.dll
cd ..

cd ApiGateway.OrderingAggregator
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:9010"
$Env:ASPNETCORE_HTTP_PORT=9010
start dotnet Biz.BrightOnion.ApiGateway.OrderingAggregator.dll
cd ..

cd Services/Identity.API
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:7001"
$Env:ASPNETCORE_HTTP_PORT=7001
start dotnet Biz.BrightOnion.Identity.API.dll
cd ../..

cd Services/Identity.BackgroundTasks
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:7011"
$Env:ASPNETCORE_HTTP_PORT=7011
start dotnet Biz.BrightOnion.Identity.BackgroundTasks.dll
cd ../..

cd Services/Catalog.API
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:7002"
$Env:ASPNETCORE_HTTP_PORT=7002
start dotnet Biz.BrightOnion.Catalog.API.dll
cd ../..

cd Services/Catalog.BackgroundTasks
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:7012"
$Env:ASPNETCORE_HTTP_PORT=7012
start dotnet Biz.BrightOnion.Catalog.BackgroundTasks.dll
cd ../..

cd Services/Ordering.API
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:7003"
$Env:ASPNETCORE_HTTP_PORT=7003
start dotnet Biz.BrightOnion.Ordering.API.dll
cd ../..

cd Services/Ordering.SignalrHub
$Env:ASPNETCORE_ENVIRONMENT=$app_env
$Env:ASPNETCORE_URLS="http://+:7004"
$Env:ASPNETCORE_HTTP_PORT=7004
start dotnet Biz.BrightOnion.Ordering.SignalrHub.dll
cd ../..
