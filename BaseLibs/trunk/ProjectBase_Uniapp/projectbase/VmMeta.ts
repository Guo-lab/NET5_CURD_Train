import { pb } from './projectbase.service';
import { ValidationRule } from './projectbase.type';

export class VmMeta {
    static TextInputMaxLength = 2000;
    rules: { [key: string]: ValidationRule }={};
    translate: string|null=null;
    required: boolean=false;
    modelType: string='';
    formIgnore: boolean=false;
    submitIgnore: boolean=false;
    properties: { [propname: string]: VmMeta }={};

    constructor(translate?: string|null, modelType?: string, required?: boolean) {
        if(translate!==undefined){
            this.translate = translate;            
        }
        if(modelType!==undefined){
            this.modelType = modelType;            
        }
        if(required!==undefined){
            this.required = required;            
        }

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

export class VmMetaBuilder {
    CreateMetaForType(vmInputTypeName: string, propNames: string) {
        let meta = pb.GetVmMeta(vmInputTypeName);
        if (meta) return meta;

        const containerTypeMeta = new VmMeta();
        pb.AddVmMeta(vmInputTypeName, containerTypeMeta);
        for (const prop of propNames.split(',')) {
            meta = new VmMeta();
            containerTypeMeta.properties[prop] = meta;
        }
        return meta;
    }
    AddRules(containerTypeMeta: VmMeta, propName: string, rules: ValidationRules) {
        const meta = containerTypeMeta.properties[propName];
        meta.rules = rules;
    }
}
export type ValidationRules = {
    required?: ValidationRule;
    max?: ValidationRule;
    min?: ValidationRule;
    maxLength?: ValidationRule;
    minLength?: ValidationRule;
    pattern?: ValidationRule;
    vmInner?: ValidationRule;
}
export const pbMetaBuilder = new VmMetaBuilder();