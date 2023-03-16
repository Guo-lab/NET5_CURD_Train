import { NameValueObj } from "./projectbase.type";

export class PBInjector {
    static InjectToken={
        PBPlugIn:'PBPlugIn',
        EnumData_Dict: 'EnumData_Dict',
    }
    private static  store: NameValueObj={};
    static inject(tokenName: string, obj: any) {
        this.store[tokenName] = obj;
    }
    static get(tokenName: string) {
        return this.store[tokenName];
    }
}