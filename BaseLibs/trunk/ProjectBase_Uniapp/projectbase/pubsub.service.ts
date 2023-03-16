import { AnyDataSubject, ComponentCallSubject, VmDataSubject } from './projectbase.type';
import { RoutedComponentBase } from './RoutedComponentBase';

export class PubSubService {

    private static EVENT_NAME_VmSource = 'Subject<VmDataSubject>';
    private static EVENT_NAME_anyDataSource = 'Subject<AnyDataSubject>';
    private static EVENT_NAME_ComCall = 'Subject<ComponentCallSubject>';

    // <-----------------Component intercommunication
    provideVm(data: VmDataSubject | any, vmTypeName?: string,
        srcComponentType?: string, srcId?: string, targetId?: string) {
        if (!vmTypeName) {
            uni.$emit(PubSubService.EVENT_NAME_VmSource,data);
        } else {
            uni.$emit(PubSubService.EVENT_NAME_VmSource, {
                vm: data, vmTypeName, srcComponentType, srcId, targetId
            });
        }
    }

    subscribeVm(targetComponentInstance: RoutedComponentBase, func?: (data: VmDataSubject) => void) {
        let _func = func;
        if (!_func) {
            _func = (data: VmDataSubject) => {
                // if (targetComponentInstance.vmTypeName === data.vmTypeName) {
                targetComponentInstance.BindVm(data.vm);
                //  }
            };
        }
        uni.$on(PubSubService.EVENT_NAME_VmSource, data => {
            if (targetComponentInstance.vmTypeName === data.vmTypeName) {
                _func!(data);
            }
        });
    }

    provideAnyData(data: AnyDataSubject | any, subjectName?: string, srcComponentType?: string|null, srcId?: string|null,
        targetComponentType?: string|null, targetId?: string|null) {
        if (!subjectName) {
            uni.$emit(PubSubService.EVENT_NAME_anyDataSource,data as AnyDataSubject);
        } else {
            uni.$emit(PubSubService.EVENT_NAME_anyDataSource, {
                subjectName, anyData: data, srcComponentType, srcId,
                targetComponentType, targetId
            });
        }
    }

    subscribeAnyData(subjectName: string, func?: (data: AnyDataSubject) => void, filterFunc?: (data: AnyDataSubject)=>boolean) {
        uni.$on(PubSubService.EVENT_NAME_anyDataSource, data => {
            if (subjectName === data.subjectName) {
                if (filterFunc && !filterFunc(data)) return;
                if(func){
                   func(data); 
                }
            }
        });
    }

    componentCall<T>(callFunc: (targetComponentInstance: T) => any, args?: any, srcComponentType?: string, srcId?: string,
        targetComponentType?: string, targetId?: string) {
        uni.$emit(PubSubService.EVENT_NAME_ComCall, {
            callFunc, args, srcComponentType, srcId,
            targetComponentType, targetId
        });
    }

    onComponentCall(targetComponentInstance: RoutedComponentBase, targetComponentType?: string, filterFunc?: (data: ComponentCallSubject) => boolean) {
        uni.$on(PubSubService.EVENT_NAME_ComCall, data => {
            if (data.targetComponentType === targetComponentType) {
                if (filterFunc && !filterFunc(data)) return;
                data.callFunc(targetComponentInstance);
            }
        });
    }
    // --------------------------------->Component intercommunication

}
export const pubsub = new PubSubService();

