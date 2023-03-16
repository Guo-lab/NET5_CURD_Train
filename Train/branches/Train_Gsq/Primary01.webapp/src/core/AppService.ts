import { App_Dict } from "./AppDict";
import { AppPBPlugInService } from "./AppPBPlugin";
import { BaseAppService } from "projectbase/BaseAppService";
import { PBInjector } from "projectbase/PbInjector";
import { pb } from "projectbase/projectbase.service";
import { pbTranslate } from "projectbase/TranslateService";
import {pbConfig} from "projectbase/ProjectBaseConfig";
import JSGlobal from "projectbase/JSGlobal";
import { ProjectHierarchy } from "@/projectbase/projectbase.type";
import App_ProjectHierarchy from "@/App_ProjectHierarchy";

export class AppService extends BaseAppService {
    modules: string[];
	LoginInfo: any;
		
    constructor(){
        super();
        (JSGlobal as any).pbConfig = pbConfig;
        this.modules = (App_ProjectHierarchy as any as ProjectHierarchy).Modules;
    }
    RegisterServices() {
        PBInjector.register(PBInjector.InjectToken.PBPlugIn, new AppPBPlugInService());
        PBInjector.register(PBInjector.InjectToken.EnumData_Dict, App_Dict);

    }
  Application_Start() {
        super.Application_Start();
        this.LoadVmMeta();
        this.LoadTranslation();
        // uni.configMTLS({
        //     certificates: [{
        //         host: 'www.test.com',
        //         server: ['/static/server.cer']
        //     }]
        // });
    }
    LoadTranslation() {
        let transObjs = [];
        transObjs.push(require(`../lang/zh-cn.json`));
        this.modules.forEach((item) => {
            try {
                transObjs.push(require(`../lang/${item}/zh-cn.json`));
            } catch (err) {
                //console.warn('加载zh-cn.json失败:'+item);
            }
        });
        pbTranslate.LoadTranslation(transObjs);
    }
    LoadVmMeta() {
        pb.LoadVmMeta(require(`../pages/Shared/common.meta.json`));
        this.modules.forEach((item) => {
            try {
                pb.LoadVmMeta(require(`../pages/${item}/${item}.meta.json`));
            } catch (err) {
                //console.warn('加载meta.json失败:'+item);
            }
        });
    }

    OnDevRefresh(stateObj: any) {
        this.LoginInfo = stateObj?.LoginInfo;
    }
}

export const app = new AppService();
