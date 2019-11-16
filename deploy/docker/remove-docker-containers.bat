docker container stop biz_onion_catalog_bgt_1
docker container rm biz_onion_catalog_bgt_1

docker container stop biz_onion_catalog_api_1
docker container rm biz_onion_catalog_api_1

docker container stop biz_onion_identity_bgt_1
docker container rm biz_onion_identity_bgt_1

docker container stop biz_onion_identity_api_1
docker container rm biz_onion_identity_api_1

docker container stop biz_onion_order_api_1
docker container rm biz_onion_order_api_1

docker container stop biz_onion_order_hub_1
docker container rm biz_onion_order_hub_1

docker container stop biz_onion_web_1
docker container rm biz_onion_web_1

rem docker container stop biz_onion_rabbit
rem docker container rm biz_onion_rabbit

rem docker container stop biz_onion_sql
rem docker container rm biz_onion_sql

rem docker network rm biz_onion_net
