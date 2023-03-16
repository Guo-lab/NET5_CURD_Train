import { NameValueObj } from './projectbase.type';

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
    Alert(msgKey: string | string[]) {
        let text = msgKey;
        if (Array.isArray(msgKey)) {
            text = msgKey.join(';');
        }

        uni.showModal({
            title: '提示',
            content: text as string,
            showCancel: false
        });
    }

    Confirm(msgKey: string, msgParams?: NameValueObj | any, extra?: {
        size: any;
        animationsEnabled: boolean;
        confirmTitleKey: string;
        labelKey_No: string;
        labelKey_Yes: string;
    }): Promise<boolean> {
        let text = msgKey;
        return new Promise((resolve, reject) => {
            uni.showModal({
                title: '提示',
                content: text,
                showCancel: false,
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

    Toast(msgKey: string, msgParams?: NameValueObj | any, _duration?: number, position?: 'middle' | 'top' | 'bottom', cssClass?: string) {
        uni.showToast({
            title: msgKey,
            duration: 5000
        });
    }
    ToastSuccess(msgKey: string, msgParams?: NameValueObj | any, duration?: number, position?: 'middle' | 'top' | 'bottom') {
        this.Toast(msgKey, msgParams, duration, position, 'pb-success');
    }
    ToastInfo(msgKey: string, msgParams?: NameValueObj | any, duration?: number, position?: 'middle' | 'top' | 'bottom') {
        this.Toast(msgKey, msgParams, duration, position, 'pb-info');
    }
    ToastWarn(msgKey: string, msgParams?: NameValueObj | any, duration?: number, position?: 'middle' | 'top' | 'bottom') {
        this.Toast(msgKey, msgParams, duration, position, 'pb-warn');
    }

}

export const pbui = new ProjectBaseUIService();