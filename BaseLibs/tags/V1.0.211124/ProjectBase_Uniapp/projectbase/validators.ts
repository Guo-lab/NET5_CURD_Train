import { PbValidationErrors, ValidatorFn } from './projectbase.type';

function pbIntValidator(typeName: string): { validateFunction: ValidatorFn } {
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

function pbDateValidator(fake: string): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            return true;
        }
    };
}

function pbEqualtoValidator(equalToControlPath: string): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            return value === data[equalToControlPath];
        }
    };
}
function pbCompareToValidator(equalToControlPath: string,compare:number): { validateFunction: ValidatorFn } {
    return {
        validateFunction: (rule, value, data, callback): boolean => {
            if (compare === 0) {
                return value === data[equalToControlPath];
            } else if (compare === 1) {
                return value > data[equalToControlPath];
            } else {
                return value < data[equalToControlPath];
            }
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
            } else {
                return true;
            }
        }
    };
}
function pbRequireValidaor(): { validateFunction: ValidatorFn } {
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
        required: Validators.required,
        min: Validators.min,
        max: Validators.max,
        'maxlength': Validators.maxLength,
        'minlength': Validators.minLength,
        pattern: Validators.pattern,
        int: pbIntValidator,
        date: pbDateValidator,
        trim: pbStringTrimValidator,
        email: Validators.email,
        equalTo: pbEqualtoValidator,
        compareTo: pbCompareToValidator,
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

