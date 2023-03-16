import { ValidationRule } from './projectbase.type';

export class VmMeta {
    static TextInputMaxLength = 2000;
    rules: { [key: string]: ValidationRule };
    translate: string;
    required: boolean;
    modelType: string;
    formIgnore: boolean;
    submitIgnore: boolean;
    properties: { [propname: string]: VmMeta };

    constructor(translate?: string, modelType?: string, required?: boolean) {
        this.translate = translate;
        this.modelType = modelType;
        this.required = required;
    }
    isEnum() {
        return this.modelType?.endsWith('Enum');
    }
    isDORef() {
        return this.modelType?.startsWith('DORef:');
    }
    isBool() {
        return this.modelType === 'boolean' || this.modelType === 'Boolean';
    }
    isString() {
        return this.modelType === 'String';
    }
    isDate() {
        return this.modelType === 'DateTime';
    }
    isNumber() {
        return this.modelType === 'Int32' || this.modelType === 'Int16' || this.modelType === 'Byte'
            || this.modelType === 'Long' || this.modelType === 'short' || this.modelType === 'Short'
            || this.modelType === 'Decimal';
    }
    maxLength() {
        return (this.rules && this.rules.maxlength) ? this.rules.maxlength.param : VmMeta.TextInputMaxLength;
    }

}

export interface VmMetaMiniFied {
    /** rules */
    v: { [key: string]: ValidationRuleMiniFied };
    /** translate */
    k: string;
    /** required */
    r?: boolean;
    /** modelType */
    m: string;
    /** formIgnore */
    f?: boolean;
    /** submitIgnore */
    s?: boolean;
    /** properties */
    c?: { [propname: string]: VmMetaMiniFied };
}
export interface ValidationRuleMiniFied {
    /** param */
    p?: any;
    /** type */
    t: string;
    /** groups */
    g?: string[];
    /** errmsg */
    e?: string;
}
