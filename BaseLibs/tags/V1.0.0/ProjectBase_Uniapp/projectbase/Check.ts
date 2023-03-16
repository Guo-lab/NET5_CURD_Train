import { NgxArchError } from './projectbase.type';

/** designbycontract.同服务器端Check类。 */
export class Check {
    static Require(assertion: boolean, errorMsg?: string, err?: Error) {
        if (assertion) return;
        if (err) throw err;
        throw new NgxArchError('检查断言发现错误：' + errorMsg);
    }

}


