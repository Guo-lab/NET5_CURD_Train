import { PbValidationErrors, ValidatorFn } from './projectbase.type';

function pbIntValidator(ruleParam:any): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (value === null || value === 'null' || value === '') {
                return true;
            }
            return (typeof (value) === 'number' && Number.isInteger(value))
                || (typeof (value) === 'string' && Number.parseInt(value, 10).toString() === value);
        }
    };
}
function pbMaxValidator(ruleParam:any): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (value === null || value === 'null' || value === '') {
                return true;
            }
            return (typeof (value) === 'number' && value<=ruleParam.max)
                || (typeof (value) === 'string' && -(-value)<=ruleParam.max);
        }
    };
}
function pbMinValidator(ruleParam:any): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (value === null || value === 'null' || value === '') {
                return true;
            }
            return (typeof (value) === 'number' && value>=ruleParam.min)
                || (typeof (value) === 'string' && -(-value)>=ruleParam.min);
        }
    };
}

function pbDateValidator(fake: string): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            return true;
        }
    };
}

function pbEqualtoValidator(ruleParam:any): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            return value === data[ruleParam.equalToControlPath];
        }
    };
}
function pbCompareToValidator(ruleParam:any): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (ruleParam.compare === 0) {
                return value === data[ruleParam.equalToControlPath];
            } else if (ruleParam.compare === 1) {
                return value > data[ruleParam.equalToControlPath];
            } else {
                return value < data[ruleParam.equalToControlPath];
            }
        }
    };
}
function pbRangeToValidator(ruleParam:any): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (!value && value!==0) return true;
            var min = ruleParam.min;
            var max = ruleParam.max;
            if (min != null && value < min)  return false;
            if (max != null) return value <= max;
            return true;
        }
    };
}
function pbStringLengthValidator(ruleParam:any): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (!value) return true;
            var min = ruleParam.min;
            var max = ruleParam.max;
            return value.length >= min && value.length <= max;;
        }
    };
}
function pbDORefNotNullIdValidator(): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            return value !== null && value !== 'null' && value !== 0 && value !== '0';
        }
    };
}
// function pbArraySizeMaxValidator(max: number): ValidatorFn {
//     return (control: AbstractControl): PbValidationErrors => {
//         return (control.value === null || Array.isArray(control.value) && control.value.length <= max) ?
//             null : { 'arraySizeMax': { shouldBe: max, actual: control.value.length } };
//     };
// }
// function pbArraySizeMinValidator(min: number): ValidatorFn {
//     return (control: AbstractControl): PbValidationErrors => {
//         return (control.value === null || Array.isArray(control.value) && control.value.length >= min) ?
//             null : { 'arraySizeMin': { shouldBe: min, actual: control.value.length } };
//     };
// }
function pbClassValidateValidator(classValidatefunc: ClassValidator): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            const err = classValidatefunc(value, data);
            if (err) {
                callback(err);
                return false;
            } else {
                return true;
            }
        }
    };
}
function pbRequiredValidator(): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            return !(value === null || (typeof value === 'string' && (value as string).trim().length === 0));
        }
    };
}
function pbStringTrimValidator(): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (typeof value !== 'string') return true;
            const v = value as string;
            if (v === null) return true;
            if (v.startsWith(' ') || v.endsWith(' ')) {
                value = v.trim();
            }
            return true;
        }
    };
}
function pbMinLengthValidator(minlength: number): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (typeof value !== 'string') return true;
            return value.length <= minlength;
        }
    };
}

/** uniapp内置验证规则 */
export const Validators = {
    required: () => {
        return { required: true };
    },
    max: (p: any) => {
        return { maximum: p.max };
    },
    min: (p: any) => {
        return { minimum: p.min };
    },
    email: () => {
        return { format: 'email', errorMessage: '电邮地址格式错' };
    },
    maxLength: (p: any) => {
        return { maxLength: p.max };
    },
    minLength: (p: any) => {
        return { minLength: p.min };
    },
    pattern: (p: any) => {
        return { pattern: p.pattern };
    }
}

/**
 * 验证器仓库，包括内置验证器，并可注册应用程序自定义的验证器
 */
export class ValidatorRepo {
    private map = {
        required: Validators.required,//不需要pbRequiredValidator,uniapp本身检查空串等
        min: pbMinValidator,
        max: pbMaxValidator,
        'maxlength': Validators.maxLength,
        'minlength': Validators.minLength,
        pattern: Validators.pattern,
        int: pbIntValidator,
        date: pbDateValidator,
        trim: 'ignore',//pbStringTrimValidator,uniapp输入框不需要trim，是自动trim的。
        email: Validators.email,
        equalTo: pbEqualtoValidator,
        'pb-CompareTo': pbCompareToValidator,
        'pb-Range': pbRangeToValidator,
        'pb-String-Length':pbStringLengthValidator,
        dorefNotNullId: pbDORefNotNullIdValidator,
        //          dorefNotNull: pbDORefNotNullValidator
        /////// arraySizeMax: pbArraySizeMaxValidator,
        //////// arraySizeMin: pbArraySizeMinValidator,
        classValidate: pbClassValidateValidator
    } as any;

    getValidator(validationType: string) {
        return this.map[validationType];
    }
    registerValidator(validationType: string, validator: (...p: any) => ValidatorFn) {
        this.map[validationType] = validator;
    }
    registerAll(validatorMap: { [validationType: string]: (...p: any) => ValidatorFn }) {
        Object.assign(this.map, validatorMap);
    }
}
export const pbValidatorRepo = new ValidatorRepo();
export type ClassValidator = (value: any, rawValue?: any) => string;

