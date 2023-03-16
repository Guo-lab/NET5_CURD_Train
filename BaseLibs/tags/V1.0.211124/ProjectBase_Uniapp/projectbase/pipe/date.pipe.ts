export class DatePipe{
    static DefaultFormat = 'YYYY-MM-DD';
    static transform(value: Date, format?: string) {
        format = format ||DatePipe.DefaultFormat;
        var o = {
            "M+": value.getMonth() + 1,                 //月份
            "d+": value.getDate(),                    //日
            "h+": value.getHours(),                   //小时
            "m+": value.getMinutes(),                 //分
            "s+": value.getSeconds(),                 //秒
            "q+": Math.floor((value.getMonth() + 3) / 3), //季度
            "S": value.getMilliseconds()             //毫秒
        } as any;
        if (/(y+)/.test(format))
            format = format.replace(RegExp.$1, (value.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k of Object.keys(o))
            if (new RegExp("(" + k + ")").test(format))
                format = format.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return format;
    }

}
