import { isDate } from '@vue/shared';
import { Check } from './Check';
import { pbErrorHandler } from './PBErrorHandler';
import { pb } from './projectbase.service';
import { NameValueObj, RcResult, VmForm,NgxArchError } from './projectbase.type';
import { PbTranslate } from './TranslateService';
import { VmMeta } from './VmMeta';

export type ParamMap = NameValueObj;
export class UtilHelper {
    static AddLogAction = 'Shared/Common/AddClientLog';
    static TimeZoneDiff = 28800000; // ms， 8小时。
    /** TodoWhen方法参数timeoutMillisec的缺省值 */
    static ToDoWhenTimeout = 10000; // ms
    GetParamObjFromParamMap(paramMap: ParamMap): NameValueObj {
        const p: any = {};
        for (const key of paramMap.keys) {
            p[key] = paramMap.get(key);
        }
        return p;
    }
    GetParamFromLocationUrl(url: string, paramName: string): string | null {
        const urlparts = url.split(';');
        for (const pair of urlparts) {
            const nv = pair.split('=');
            if (nv[0] === paramName && nv.length > 1) {
                return nv[1];
            }
        }
        return null;
    }
    GetParamFromUrl(url: string, paramName: string): string | null {
        const urlparts = url.split('?');
        if (urlparts.length < 2) return null;
        const pairs = urlparts[1].split('&');
        for (const pair of pairs) {
            const nv = pair.split('=');
            if (nv[0] === paramName && nv.length > 1) {
                return nv[1];
            }
        }
        return null;
    }
    /** 将对象类型参数转换为qs格式。此方法与this.objToQs不同之处在于参数名不是级联名称 */
    ParamsToQS(params: NameValueObj | ParamMap, names?: string[]): string {
        let paramObj = params;
        if (params.keys) {
            paramObj = this.GetParamObjFromParamMap(params as ParamMap);
        }
        let keys = names || Object.keys(paramObj);
        let qs = '';
        for (const key of keys) {
            qs = qs + '&' + key + '=' + paramObj[key];
        }
        return qs ? qs.substr(1) : qs;
    }
    /**
     * 合并querystring名值对格式的字符串，用第二个里的参数覆盖第一个
     */
    MergeQS(qs1: string, qs2: string) {
        if (typeof (qs1) === 'undefined') {
            if (typeof (qs2) === 'undefined') {
                return '';
            }
            return qs2;
        } else {
            if (typeof (qs2) === 'undefined') {
                return qs1;
            }
        }

        if (qs1 === '' || qs2 === '') { return qs1 + qs2; }

        const pairs1 = qs1.split('&');
        const pairs2 = qs2.split('&');
        let newqs = '';
        let qs1new = '';
        let qs2new = '';
        for (let i = 0; i < pairs2.length; i++) {
            const a = pairs2[i].split('=');
            let found = false;
            for (let j = 0; j < pairs1.length; j++) {
                if (pairs1[j].split('=')[0].toLowerCase() === a[0].toLowerCase()) {
                    pairs1[j] = pairs2[i];
                    found = true;
                    break;
                }
            }
            if (!found && pairs2[i] !== '') { qs2new = qs2new + pairs2[i] + '&'; }
        }
        for (let i = 0; i < pairs1.length; i++) {
            qs1new = qs1new + pairs1[i] + '&';
        }
        if (qs1new.endsWith('&')) {
            newqs = qs1new;
        }
        if (qs2new.endsWith('&')) {
            newqs = newqs + qs2new;
        }
        if (newqs.endsWith('&')) {
            newqs = newqs.substr(0, newqs.length - 1);
        }
        return newqs;
    }

    /**
     * 
     * @param checkFormIgnore :检查数据是否未在form中生成，即被form忽略的数据，将该数据包含到处理结果中
     * @param checkSubmitIgnore :检查数据是否不应提交，对不应提交的数据，不将其包含在处理结果中
     */
    MetaFilter(enclosingObj: any, nameofPropToFilter: string|null, metaForIgnore: VmMeta, checkFormIgnore = false, checkSubmitIgnore = true) {
        if (!metaForIgnore) { return; }
        if (metaForIgnore && nameofPropToFilter && ((checkFormIgnore && metaForIgnore.formIgnore === false)
            || (checkSubmitIgnore && metaForIgnore.submitIgnore))) {
            delete enclosingObj[nameofPropToFilter];
            return;
        }
        let currentObj = nameofPropToFilter ? enclosingObj[nameofPropToFilter] : enclosingObj;
        if (this.IsObject(currentObj) && metaForIgnore.properties) {
            for (const prop of Object.keys(currentObj)) {
                if (checkFormIgnore) {
                    if (metaForIgnore.formIgnore !== false) {
                        this.MetaFilter(currentObj, prop, metaForIgnore.properties[prop], metaForIgnore.formIgnore ? false : true,
                            checkSubmitIgnore);
                    }
                } else if (checkSubmitIgnore) {
                    this.MetaFilter(currentObj, prop, metaForIgnore.properties[prop], false, true);
                }
            }
        } else if (Array.isArray(currentObj)) {
            if (nameofPropToFilter) {
                enclosingObj[nameofPropToFilter] = currentObj.filter((obj) => !this.IsNullOrUndefined(obj));
            } else {
                enclosingObj = currentObj.filter((obj) => !this.IsNullOrUndefined(obj));
            }
        }
        return;
    }
    Obj2QsDict(obj: any, metaForIgnore: VmMeta = null as any, checkFormIgnore = false, checkSubmitIgnore = true
        , map: Map<string, string> = null as any, enclosingPropName = '') {
        if (!map) { map = new Map<string, string>(); }
        if (metaForIgnore
            && ((checkFormIgnore && metaForIgnore.formIgnore === false)
                || (checkSubmitIgnore && metaForIgnore.submitIgnore))) {
            return map;
        }
        if (this.IsNullOrUndefined(obj)) {
            map.set(enclosingPropName, '');
        } else if (Array.isArray(obj)) {
            if (obj.length === 0) {
                map.set(enclosingPropName, '');
            } else {
                for (let i = 0, j = 0; i < obj.length; i++) {
                    if (!this.IsNullOrUndefined(obj[i])) {
                        this.Obj2QsDict(obj[i], null as any, checkFormIgnore, checkSubmitIgnore, map, enclosingPropName + '[' + j + ']');
                        j++;
                    }
                }
                // map.set(enclosingPropName,'');
            }
        } else if (typeof obj !== 'object') {// simple value
            map.set(enclosingPropName, obj);
        } else if (obj instanceof Date) {// Date
            const adjusted: Date = new Date(obj.getTime() + UtilHelper.TimeZoneDiff);
            map.set(enclosingPropName, adjusted.toISOString());
        } else { // object
            for (const prop of Object.keys(obj)) {
                const pname = (enclosingPropName === '' ? '' : (enclosingPropName + '.')) + prop;
                if (metaForIgnore && checkFormIgnore) {
                    if (metaForIgnore.formIgnore !== false) {
                        this.Obj2QsDict(obj[prop], (metaForIgnore.properties as any)[prop], metaForIgnore.formIgnore ? false : true,
                            checkSubmitIgnore, map, pname);
                    }
                } else {
                    this.Obj2QsDict(obj[prop], null as any, false, false, map, pname);
                }
            }
        }
        return map;
    }

    /**
     * @param obj<br>
     * @param metaForIgnore<br>
     * @checkFormIgnore :检查数据是否未在form中生成，即被form忽略的数据，将该数据包含到处理结果中<br>
     * @param checkSubmitIgnore :检查数据是否不应提交，对不应提交的数据，不将其包含在处理结果中<br>
     */
    Obj2Qs(obj: any, metaForIgnore?: VmMeta, checkFormIgnore = false, checkSubmitIgnore = true) {
        if (!obj) { return ''; }
        const map = this.Obj2QsDict(obj, metaForIgnore, checkFormIgnore, checkSubmitIgnore);
        let qs = '';
        map.forEach((value, key) => {
            qs = qs + '&' + encodeURIComponent(key) + '=' + encodeURIComponent(value);
        });
        return qs.substring(1);
    }

    CreateRcResult(command: string, _data?: any, ok = true): RcResult {
        let data = _data;
        if (command==='derived' && typeof(data)==='undefined') {
            data = { ViewModel: null };
        }
        return {
            ok,
            command,
            data,
            isRcResult:ok
        };
    }
    Capitalize(str: string) {
        if (!str) return str;
        return str.substr(0, 1).toUpperCase() + (str.length > 1 ? str.substr(1) : '');
    }

    MsgKey(key: string) {
        return PbTranslate.Prefix_Message + key;
    }

    ValKey(key: string) {
        return PbTranslate.Prefix_ValMsg + key;
    }
    MergeDeep(target: any, source: any): any {
        let output = { ...target };
        if (this.IsObject(target) && this.IsObject(source)) {
            Object.keys(source).forEach((key: any) => {
                if (this.IsObject(source[key])) {
                    if (!(key in target)) {
                        Object.assign(output, { [key]: source[key] });
                    } else {
                        output[key] = this.MergeDeep(target[key], source[key]);
                    }
                } else {
                    Object.assign(output, { [key]: source[key] });
                }
            });
        }
        return output;
    }
    /** 将源数组中在目标数组中没有的元素加入到目标数组中。根据元素对象的id属性判断是否相同 */
    ConcatDistinctById(target: any[], source: any[]): any {
        const newones = source.filter((sitem) => !target.find((titem) => titem.id === sitem.id));
        target.concat(newones);
        return target;
    }
    /** 使用补丁对象，更新目标对象的相应属性值。补丁对象或目标对象无彼此对应的属性不处理。 */
    PatchValue(target: any, patch: any): any {
        if (patch === null) {
            // tslint:disable-next-line: no-parameter-reassignment
            target = null;
            return;
        }
        Check.Require(this.IsObject(target) && this.IsObject(patch));
        Object.keys(patch).forEach((key: any) => {
            if (key in target) {
                if (this.IsObject(target[key])) {
                    this.PatchValue(target[key], patch[key]);
                } else {
                    target[key] = patch[key];
                }
            }
        });
    }
    /** 使用一个或多个补丁对象，替换目标数组中id值相同的对象。如果补丁对象id在目标数组中无对应则根据addNew决定是否添加。 */
    PatchList(listofItemWithId: any[], patchItemWithIds: any | any[], addNew = true) {
        if (Array.isArray(patchItemWithIds)) {
            for (const patchItem of patchItemWithIds) {
                const i = listofItemWithId.findIndex((v) => v.id === patchItem.id);
                if (i >= 0) {
                    listofItemWithId.splice(i, 1, patchItemWithIds);
                } else if (addNew) {
                    listofItemWithId.unshift(patchItemWithIds);
                }
            }
        } else {
            const i = listofItemWithId.findIndex((v) => v.id === patchItemWithIds.id);
            if (i >= 0) {
                listofItemWithId.splice(i, 1, patchItemWithIds);
            } else if (addNew) {
                listofItemWithId.unshift(patchItemWithIds);
            }
        }

    }
    /**
     * 使用一个补丁对象，更新目标数组中id值相同的对象的相应属性值。
     * 参数props指定仅使用补丁对象的指定属性，如果目标对象无对应属性则添加。如果不指定props，则使用补丁对象的全部属性，但无对应的属性不处理。
     * 返回目标数组中对应的对象。
     * @param addNew 如果id无对应，是否添加。
     */
    PatchListItem(listofItemWithId: any[], patchItemWithId: any, props?: string[] | string, addNew = false): any {
        const i = listofItemWithId.findIndex((v) => v.id === patchItemWithId.id);
        if (i < 0) {
            if (addNew) {
                listofItemWithId.unshift(patchItemWithId);
            }
            return;
        }

        if (!props) {
            this.PatchValue(listofItemWithId[i], patchItemWithId);
        } else if (Array.isArray(props)) {
            for (const prop of props) {
                if (prop in patchItemWithId) {
                    listofItemWithId[i][prop] = patchItemWithId[prop];
                }
            }
        } else {
            if (props in patchItemWithId) {
                listofItemWithId[i][props] = patchItemWithId[props];
            }
        }
        return listofItemWithId[i];
    }
    /** 列表中的更新选中行，更新逻辑由operateFunc给出。即对每个选中行执行一遍operateFunc方法 */
    UpdateSelectedItem(resultList: any[], selectedValues: any[], operateFunc: (item: any) => void, trackBy = 'id') {
        for (const item of resultList) {
            if (selectedValues.includes(item[trackBy])) {
                operateFunc(item);
            }
        }
    }
    // /**
    //  * 同步数组数据，缺省将sourceList中对应targetList的行整行覆盖到targetList。如果指定syncFunc参数，则对sourceList中对应行执行一遍syncFunc以便修改targetList数据。
    //  */目前未发现需要的地方，一般使用utilHelper.patchList方法即可
    // syncListData(targetList: any[], sourceList: any[], syncFunc?: (index: any, srcItem: any) => void, trackBy = 'id') {
    //     for (let index of Object.keys(targetList)) {
    //         let titem = targetList[index];
    //         for (const sitem of sourceList) {
    //             if (sitem[trackBy] === titem[trackBy]) {
    //                 if (syncFunc) {
    //                     syncFunc(index, sitem);
    //                 } else {
    //                     targetList[index] = sitem;
    //                 }
    //                 break;
    //             }
    //         }
    //     }
    // }
    ConvertToFormData(form: VmForm) {
        const formData = new FormData();
        for (const k of Object.keys(form.value)) {
            formData.append(k, form.value[k]);
        }
        return formData;
    }
    /**
     * only use this to copy literal object,mainly the vm. not for circular reference
     */
    CopyDeep(source: any): any {
        let output: any = Array.isArray(source) ? [] : (this.IsObject(source) && !(source instanceof Date) ? {} : null);
        if (output !== null) {
            Object.keys(source).forEach((key: any) => {
                output[key] = this.CopyDeep(source[key]);
            });
            return output;
        } else if (source instanceof Date) {
            return new Date(source.valueOf());
        } else {
            return source;
        }
    }
    // change array identity and every item's identity while values remain the same. not for leaf type data.
    //    ReassignArray(anyArray: any[]) {
    //        const newArray = [];
    //        anyArray.forEach((value,index) =>newArray[index]=Object.assign({},value));
    //        anyArray=newArray;
    //    }
    DelArrayItems(anyArray: any[], excludeIds: any[], trackBy = 'id') {
        const newArray: any[] = [];
        let i = 0;
        anyArray.forEach((value) => {
            if (!excludeIds.includes(value[trackBy])) {
                newArray[i++] = value;
            }
        });
        // tslint:disable-next-line: no-parameter-reassignment
        anyArray = newArray;
    }
    IsObject(value: any): boolean {
        return (value && typeof value === 'object' && !Array.isArray(value));
    }
    IsNullOrUndefined(value: any): boolean {
        return (value === null || value === void 0);
    }
    IsLikeNull(value: any) {
        return (value === null || value === void 0 || value.id === null || value === '');
    }
    IsBlank(value: string): boolean {
        return (value === null || value.trim().length === 0);
    }
    IsEmpty(value: string): boolean {
        return (value === null || value.length === 0);
    }
    /**
     * 根据文件扩展名判断是否是允许的类型
     * @param extsAccepted 允许的扩展名，逗号分隔，如.jpg,.gif
     */
    FileAccept(filename: string, extsAccepted: string) {
        const ext = filename.substring(filename.lastIndexOf('.'));
        return extsAccepted.includes(ext);
    }
    /** 复制数据并删除其中数组中的空值 */
    CopyAsNoNullArrayMember(orgData: any) {
        let submitdata = {};
        submitdata = this.MergeDeep(submitdata, orgData);
        this.RemoveNullFromArray(submitdata);
        return submitdata;
    }
    /** 删除给定对象中数组中的空值和undefined */
    RemoveNullFromArray(orgData: any, prop?: string) {
        let currentObj = prop ? orgData[prop] : orgData;
        if (this.IsObject(currentObj)) {
            for (const p of Object.keys(currentObj)) {
                this.RemoveNullFromArray(currentObj, p);
            }
        } else if (Array.isArray(currentObj)) {
            const noNullArray = currentObj.filter((obj) => !this.IsNullOrUndefined(obj));
            if (prop) {
                orgData[prop] = noNullArray;
            } else {
                orgData = noNullArray;
            }
        }
    }
    ConvertDateInObj(obj:any) {
		if (typeof (obj) == 'string' && obj.startsWith('/Date(')) {
			var dt = parseInt(obj.substring(6,obj.length-2));
			obj = dt > 0 ? new Date(dt) : null;
		} else if (Array.isArray(obj)) {
			for (var j = 0; j < obj.length; j++) {
				obj[j]=this.ConvertDateInObj(obj[j]);
			}
		} else if (typeof (obj) == 'object' && !isDate(obj)) {
			for (var prop in obj) {
				var v = obj[prop];
				obj[prop]=this.ConvertDateInObj(v);
			}
		}
		return obj;
    }
    AddLog(msg:string) {
        pb.CallAction(UtilHelper.AddLogAction, { msg })
            .catch((e) => {
                console.warn('记录客户端日志到服务器失败：'+e);
            });
    }
    /**
     * 满足条件时执行动作
     */
    ToDoWhen(condition: () => boolean, todo: () => void,timeoutCallback?:()=>void, timeoutMillisec?: number) {
        if (condition()) {
            todo();
            return;
        }
        if (timeoutMillisec!==undefined && (timeoutMillisec < 0 || timeoutMillisec === 0)) {//不能用timeoutMillisec<=0,因为null<=为true,而null<0 || null==0为false
            if (timeoutCallback) {
                timeoutCallback();
            } else {
                pbErrorHandler.Throw('Wait.Timeout');
            }
            return;
        }
        if (!timeoutMillisec) {
            timeoutMillisec = UtilHelper.ToDoWhenTimeout;
        }
        timeoutMillisec -= 100;
        setTimeout(() => {
            this.ToDoWhen(condition, todo, timeoutCallback,timeoutMillisec);
        }, 100);
    }
    GetFromResponseHeader(header: any, key: string) {
        //pc浏览器将header中的key变成全小写，真机上不变，所以此处要尝试两种情况
        return header[key] || header[key.toLowerCase()];
    }
    
    ToBoolean(value:any):boolean|undefined|null {
        if(value==='false') return false;
        if(value==='true') return true;
        if(value===undefined || value===null) return value;
        throw new NgxArchError('类型转换错：不支持的值：'+value);
    }
}

export const util = new UtilHelper();