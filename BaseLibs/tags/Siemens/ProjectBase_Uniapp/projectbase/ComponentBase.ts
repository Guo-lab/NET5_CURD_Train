//import { Vue } from "vue-property-decorator";
import Vue from 'vue'
import { pb } from './projectbase.service';
import { pbui } from './projectbase.ui.service';
import { urlHelper } from './UrlHelper';
import { PbTranslate } from './TranslateService';
import Component from "vue-class-component";
import { pbcWorker } from './ComponentWorker';

/**
 * use this base for non-routed components,which when used as a child, will use the same route-related data of it's parent
 * this class provides common utility functions to use in templates.
 */
@Component
export class ComponentBase extends Vue {
    static EVENT_VIEW_READY = 'pbViewReady';

    pb = pb;
    pbui = pbui;
    Url = urlHelper;

    Emit(eventName: string, event?: any, srcId?: string) {
        pbcWorker.Emit(this,eventName, event);
    }

    On(eventName: string, func: (event: any) => void, srcId?: string) {
        pbcWorker.On(this,eventName, func);
    }

    Values(obj: Object) {
        return Object.values(obj);
    }

    Keys(obj: Object) {
        return Object.keys(obj);
    }

    /** 使lambda表达式执行时在运行错误时返回null而不是抛异常。比如不必考虑点号引用链上的考虑空值问题 */
    SafeGet(getFunc: () => any) {
        try {
            return getFunc();
        } catch (e) {
            return null;
        }
    }

    protected MsgKey(key: string) {
        return PbTranslate.Prefix_Message + key;
    }

    protected ValKey(key: string) {
        return PbTranslate.Prefix_ValMsg + key;
    }

    protected SuperReady_ComponentBase() {
        uni.$emit(ComponentBase.EVENT_VIEW_READY, this);
    }

}
