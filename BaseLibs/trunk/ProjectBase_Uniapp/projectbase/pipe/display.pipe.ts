import { PBInjector } from "../PbInjector";
import { DatePipe } from "./date.pipe";
import { DecimalPipe } from "./decimal.pipe";
import { DictPipe } from "./dict.pipe";

export class DisplayPipe{

    static get App_Dict() {
        return PBInjector.get(PBInjector.InjectToken.EnumData_Dict);
    }
    static transform(value: any, dictNameOrFormatOrFracSize_?: string | number, begin?: number, end?: number) {
        let dictNameOrFormatOrFracSize = dictNameOrFormatOrFracSize_;
        if (value === null && dictNameOrFormatOrFracSize && typeof dictNameOrFormatOrFracSize === 'string' && DisplayPipe.App_Dict[dictNameOrFormatOrFracSize]) {
            return DictPipe.transform(value, dictNameOrFormatOrFracSize);
        }
        if (typeof value === 'undefined') { return ''; }
        if ((typeof value === 'number' || typeof value === 'boolean') && dictNameOrFormatOrFracSize) {
            if (typeof dictNameOrFormatOrFracSize === 'string' && DisplayPipe.App_Dict[dictNameOrFormatOrFracSize]) {// second param is dictname
                return DictPipe.transform(value, dictNameOrFormatOrFracSize);
            } else if (typeof dictNameOrFormatOrFracSize === 'number') {
                return DecimalPipe.transform(value as number, dictNameOrFormatOrFracSize);
            }
        }
        if (value === true) { return DisplayPipe.App_Dict.TrueDisplay; }
        if (value === false) { return DisplayPipe.App_Dict.FalseDisplay; }
        if (typeof value === 'string' && begin !== null && begin !== void 0) {
            return this.transformString(value, dictNameOrFormatOrFracSize, begin, end);
        }
        // now I assume it's a date value
        if (typeof dictNameOrFormatOrFracSize === 'undefined') { dictNameOrFormatOrFracSize = 'yyyy-MM-dd'; }
        return DatePipe.transform(value, dictNameOrFormatOrFracSize as string);
    }
    

    private static transformString(value: string, dictNameOrFormatOrFracSize_?: string | number, begin?: number, end?: number) {
        const len = value.length;
        let s = value.substring(begin||0, end);
        if (dictNameOrFormatOrFracSize_ === '...' && end && len > end - (begin||0)) {
            s = s + '...';
        }
        return s;
    }

}