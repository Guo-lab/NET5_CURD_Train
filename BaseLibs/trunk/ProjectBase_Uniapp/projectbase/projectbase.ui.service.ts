import { NameValueObj } from './projectbase.type';
import { pbTranslate } from './TranslateService';

export class ProjectBaseUIService {

    constructor() {

    }

    static SmallScreenMaxWidth = 500;//////////////environment.smallScreenMaxWidth;
    static SmallScreenMaxHeight = 830;
    static DefaultToastDuration = 3000;

    get IsNarrowScreen() {
        return true;
        //return this.platform.width() < ProjectBaseUIService.smallScreenMaxWidth;
    }
    get IsSmallScreen() {
        return true;
        //////// return this.platform.isPortrait() && (this.platform.width() < ProjectBaseUIService.smallScreenMaxWidth || this.platform.height() < ProjectBaseUIService.smallScreenMaxHeight)
        ///////////    || this.platform.isLandscape && (this.platform.width() < ProjectBaseUIService.smallScreenMaxHeight || this.platform.height() < ProjectBaseUIService.smallScreenMaxWidth);
    }
    Alert(msgKey: string | string[]): Promise<void> {
        let text = '';
        if (typeof msgKey === 'string') {
            text = pbTranslate.Translate(msgKey);
        }else if (Array.isArray(msgKey)) {
            text = msgKey.join(';');
        }
        return new Promise((resolve, reject) => {
            uni.showModal({
                title: '提示',
                content: text as string,
                showCancel: false,
                success: function (res) {
                    resolve();
                }
            });
        });
    }

    Confirm(msgKey: string, msgParams?: NameValueObj | any, extra?: {
        size: any;
        animationsEnabled: boolean;
        confirmTitleKey: string;
        labelKey_No: string;
        labelKey_Yes: string;
    }): Promise<boolean> {
        let text = pbTranslate.Translate(msgKey,msgParams);
        return new Promise((resolve, reject) => {
            uni.showModal({
                title: '提示',
                content: text,
                showCancel: true,
                success: function (res) {
                    if (res.confirm) {
                        resolve(true);
                    } else if (res.cancel) {
                        resolve(false);
                    }
                }
            });
        });
    }

    Toast(msgKey: string, msgParams?: NameValueObj | any, _duration?: number, _position?: 'top' | 'center' | 'bottom', _icon?: 'success' | 'loading' | 'error' | 'none') {
        let text = pbTranslate.Translate(msgKey,msgParams);
        uni.showToast({
            title: text,
            duration: _duration || ProjectBaseUIService.DefaultToastDuration,
            position: _position || 'center',
            icon: _icon ||'none'
        });
    }
    ToastSuccess(msgKey: string, msgParams?: NameValueObj | any, duration?: number, position?: 'top' | 'center' | 'bottom') {
        this.Toast(msgKey, msgParams, duration, position, 'success');
    }
    ToastInfo(msgKey: string, msgParams?: NameValueObj | any, duration?: number, position?: 'top' | 'center' | 'bottom') {
        this.Toast(msgKey, msgParams, duration, position, 'none');
    }
    ToastWarn(msgKey: string, msgParams?: NameValueObj | any, duration?: number, position?: 'top' | 'center' | 'bottom') {
        this.Toast(msgKey, msgParams, duration, position, 'error');
    }

}

export const pbui = new ProjectBaseUIService();