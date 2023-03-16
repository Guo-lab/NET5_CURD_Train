import { NameValueObj } from "./projectbase.type";
import { util } from "./UtilHelper";

export class PbTranslate {
    static Prefix_Message = 'M.';
    static Prefix_ValMsg = 'V.';
    static Prefix_Exception = 'Error.';
}

export class TranslateService {
    Translation:any=require('./validators.zh-cn.json');
    LoadTranslation(translations:any[]){
        translations.forEach((item) => {
            this.Translation=util.MergeDeep(this.Translation,item);
        });
    }
    Error(key:string){
        return this.Translation[key]||this.Translation.Error[key] || this.Translation[PbTranslate.Prefix_Exception + key] || PbTranslate.Prefix_Exception + key;
    }
    Val(valType: string, valParam: NameValueObj|null, field?: string|null) {
        let msg!: string;
        let key!: string;
        if (field) {
            key = valType + '_' + field;
            msg = this.Translation.V[key];
        }
        if (!msg) {
            msg = this.Translation.V[valType];
        }
         if (!msg) {
            msg = PbTranslate.Prefix_ValMsg+key;
        }  
        if (field) {
            msg=msg.replace(new RegExp('${field}', 'gm'),field);
        }
        if(valParam){
            msg=this.TranslateParam(msg,valParam);
        }
        return msg;
    }
    Translate(key: string, paramObj?: NameValueObj) {
        let msg = this.Translation[key] || key;
        if (paramObj) {
            msg=this.TranslateParam(msg,paramObj);
        }
        return msg;
    }
    Dict(dictname:string,key: string) {
        return this.Translation.pbDict[dictname][key] || 'pbDict.' + dictname+'.'+ key;
    }
    DeprefixError(key: string) {
        return key.startsWith(PbTranslate.Prefix_Exception) ? key.substring(PbTranslate.Prefix_Exception.length):key;
    }

    private TranslateParam(tranlated:string,paramObj: NameValueObj) {
        for (const pname of Object.keys(paramObj)) {
            tranlated = tranlated.replace(new RegExp('${' + pname + '}','gm'), paramObj[pname]);
        }
        return tranlated;
    }
}

export const pbTranslate = new TranslateService();