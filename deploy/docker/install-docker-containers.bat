docker network create biz_onion_net

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=H0t0ni0n!" -p 14330:1433 --net biz_onion_net --name biz_onion_sql -d mcr.microsoft.com/mssql/server:2019-GA-ubuntu-16.04 
docker run -d -p 15672:15672 -p 5672:5672 --net biz_onion_net --name biz_onion_rabbit rabbitmq:3-management

docker container run -p 7001:80 --net biz_onion_net -e ASPNETCORE_ENVIRONMENT="Docker" --name biz_onion_identity_api_1 biz_onion_identity_api
docker container run -p 7011:80 --net biz_onion_net -e ASPNETCORE_ENVIRONMENT="Docker" --name biz_onion_identity_bgt_1 biz_onion_identity_bgt

docker container run -p 7002:80 --net biz_onion_net -e ASPNETCORE_ENVIRONMENT="Docker" --name biz_onion_catalog_api_1 biz_onion_catalog_api
docker container run -p 7012:80 --net biz_onion_net -e ASPNETCORE_ENVIRONMENT="Docker" --name biz_onion_catalog_bgt_1 biz_onion_catalog_bgt
