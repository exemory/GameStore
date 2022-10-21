import {commonEnvironment} from "./environment.common";

const env = {
  production: true,
  apiUrl: 'https://localhost:7116/api/'
};

export const environment = {...commonEnvironment, ...env}
