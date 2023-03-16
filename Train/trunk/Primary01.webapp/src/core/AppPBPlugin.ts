import { PBPlugInService } from "@/projectbase/pbplugin.service";
import { ProjectBaseService } from "@/projectbase/projectbase.service";
import { RcResult } from "@/projectbase/projectbase.type";

export class AppPBPlugInService extends PBPlugInService {

    ExecuteErrorResult(rcJsonResult: RcResult, pb: ProjectBaseService, hostComponentInstance?: any) {
        if (rcJsonResult.isRedirect && rcJsonResult.data=='/Home/ShowLogin') {
            rcJsonResult.data ='/Home/Main/ShowLogin'
        }
        super.ExecuteErrorResult(rcJsonResult, pb, hostComponentInstance);
    }
}
