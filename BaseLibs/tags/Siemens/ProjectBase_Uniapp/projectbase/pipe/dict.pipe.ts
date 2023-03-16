import { PBInjector } from "../PbInjector";
import { pbTranslate } from "../TranslateService";

export class DictPipe{

    static get App_Dict(){
        return PBInjector.get(PBInjector.InjectToken.EnumData_Dict);
    }

    static transform(value: any, dictName: string) {
        const obj = DictPipe.App_Dict[dictName];
        const prefix = 'pbDict.' + dictName + '.';
        for (const label of Object.keys(obj)) {
            if (obj[label] === value) {
                return label;//pbTranslate.Dict(dictName,label);
            }
        }
        return value === null ? '' : (prefix + value);
    }

}
