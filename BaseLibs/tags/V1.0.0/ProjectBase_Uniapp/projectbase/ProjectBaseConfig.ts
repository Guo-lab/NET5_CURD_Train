/** use this to config services in projectbase module */
export class ProjectBaseConfig {
    UrlContextPrefix = 'tobeset';
    UrlMappingPrefix = 'tobeset';
    TokenChannel:'Header'|'Request'|'Cookie'='Header';
    ClientType='APP';
    HttpRetry?: number;
}

export const pbConfig = new ProjectBaseConfig();
