// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  accountServiceUrl: "http://localhost:9000/identity-api/account",
  roomServiceUrl: "http://localhost:9000/catalog-api/room",
  orderServiceUrl: "http://localhost:9000/ordering-api/v1/orders",
  orderSignalrHubUrl: "http://localhost:9000/notificationhub"
};
