mkdir ~/Biz.BrightOnion
mkdir ~/Biz.BrightOnion/Web
mkdir ~/Biz.BrightOnion/ApiGateway
mkdir ~/Biz.BrightOnion/ApiGateway.OrderingAggregator
mkdir ~/Biz.BrightOnion/Services
mkdir ~/Biz.BrightOnion/Services/Identity.API
mkdir ~/Biz.BrightOnion/Services/Identity.BackgroundTasks
mkdir ~/Biz.BrightOnion/Services/Catalog.API
mkdir ~/Biz.BrightOnion/Services/Catalog.BackgroundTasks
mkdir ~/Biz.BrightOnion/Services/Ordering.API
mkdir ~/Biz.BrightOnion/Services/Ordering.SignalrHub
mkdir ~/Biz.BrightOnion/Services/Analytics.API

cp -r ../../Web/Biz.BrightOnion.Web/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/Web

cp -r ../../ApiGateways/Biz.BrightOnion.ApiGateway/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/ApiGateway

cp -r ../../ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/ApiGateway.OrderingAggregator

cp -r ../../Services/Analytics/Biz.BrightOnion.Analytics.API/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/Services/Analytics.API

cp -r ../../Services/Catalog/Biz.BrightOnion.Catalog.API/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/Services/Catalog.API

cp -r ../../Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/Services/Catalog.BackgroundTasks

cp -r ../../Services/Identity/Biz.BrightOnion.Identity.API/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/Services/Identity.API

cp -r ../../Services/Identity/Biz.BrightOnion.Identity.BackgroundTasks/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/Services/Identity.BackgroundTasks

cp -r ../../Services/Ordering/Biz.BrightOnion.Ordering.API/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/Services/Ordering.API

cp -r ../../Services/Ordering/Biz.BrightOnion.Ordering.SignalrHub/bin/Debug/netcoreapp2.2/publish/* ~/Biz.BrightOnion/Services/Ordering.SignalrHub

cp ./*.sh ~/Biz.BrightOnion
