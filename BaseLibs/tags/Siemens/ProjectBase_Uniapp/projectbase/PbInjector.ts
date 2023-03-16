import { NameValueObj } from "./projectbase.type";

/**
 * 简单模拟Angular的手动IOC。
 */
export class PBInjector {
    /** 可注入到框架中的依赖项名称 */
    static InjectToken={
        PBPlugIn:'PBPlugIn',
        EnumData_Dict: 'EnumData_Dict',
        AppService:'AppService'
    }
    private static  store: NameValueObj={};
    /** 注册依赖项 */
    static register(tokenName: string, obj: any) {
        this.store[tokenName] = obj;
    }
    /** 获取依赖项 */
    static get(tokenName: string) {
        return this.store[tokenName];
    }
}