$rootDir = [Environment]::GetFolderPath("MyDocuments")

mkdir $rootDir/Biz.BrightOnion
mkdir $rootDir/Biz.BrightOnion/Web
mkdir $rootDir/Biz.BrightOnion/ApiGateway
mkdir $rootDir/Biz.BrightOnion/ApiGateway.OrderingAggregator
mkdir $rootDir/Biz.BrightOnion/Services
mkdir $rootDir/Biz.BrightOnion/Services/Identity.API
mkdir $rootDir/Biz.BrightOnion/Services/Identity.BackgroundTasks
mkdir $rootDir/Biz.BrightOnion/Services/Catalog.API
mkdir $rootDir/Biz.BrightOnion/Services/Catalog.BackgroundTasks
mkdir $rootDir/Biz.BrightOnion/Services/Ordering.API
mkdir $rootDir/Biz.BrightOnion/Services/Ordering.SignalrHub
mkdir $rootDir/Biz.BrightOnion/Services/Analytics.API

cp -r ../../../Web/Biz.BrightOnion.Web/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/Web

cp -r ../../../ApiGateways/Biz.BrightOnion.ApiGateway/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/ApiGateway

cp -r ../../../ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/ApiGateway.OrderingAggregator

cp -r ../../../Services/Analytics/Biz.BrightOnion.Analytics.API/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/Services/Analytics.API

cp -r ../../../Services/Catalog/Biz.BrightOnion.Catalog.API/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/Services/Catalog.API

cp -r ../../../Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/Services/Catalog.BackgroundTasks

cp -r ../../../Services/Identity/Biz.BrightOnion.Identity.API/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/Services/Identity.API

cp -r ../../../Services/Identity/Biz.BrightOnion.Identity.BackgroundTasks/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/Services/Identity.BackgroundTasks

cp -r ../../../Services/Ordering/Biz.BrightOnion.Ordering.API/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/Services/Ordering.API

cp -r ../../../Services/Ordering/Biz.BrightOnion.Ordering.SignalrHub/bin/Debug/netcoreapp2.2/publish/* $rootDir/Biz.BrightOnion/Services/Ordering.SignalrHub

cp ./*.ps1 $rootDir/Biz.BrightOnion
