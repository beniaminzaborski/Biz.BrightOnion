// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,

  restBaseUrl: "http://localhost:8666/",
  accountServiceUrl: "https://localhost:7100/identity-api/account",
  roomServiceUrl: "https://localhost:7100/catalog-api/room"
};