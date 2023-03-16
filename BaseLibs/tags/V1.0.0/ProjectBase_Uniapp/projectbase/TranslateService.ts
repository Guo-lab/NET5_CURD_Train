import { util } from "./UtilHelper";

export class PbTranslate {
    static Prefix_Message = 'M.';
    static Prefix_ValMsg = 'V.';
    static Prefix_Exception = 'Error.';
}

export class TranslateService {
    Translation:any={};
    LoadTranslation(translations:any[]){
        translations.forEach((item) => {
            this.Translation=util.MergeDeep(this.Translation,item);
        });
    }
    Error(key:string){
        return this.Translation.Error[key]||'Error.'+key;
    }
    Val(valType:string,valParam:any,field?:string){
        const key = field ? valType + '_' + field:valType;
        var msg:string= this.Translation.V[key]||PbTranslate.Prefix_ValMsg+key;
        if(valParam){
            for (const pname of Object.keys(valParam)) {
                msg=msg.replaceAll('${' + pname+'}', valParam[pname]);
            }
        }
        return msg;
    }
    Dict(dictname:string,key: string) {
        return this.Translation.pbDict[dictname][key] || 'pbDict.' + dictname+'.'+ key;
    }
}

export const pbTranslate = new TranslateService();