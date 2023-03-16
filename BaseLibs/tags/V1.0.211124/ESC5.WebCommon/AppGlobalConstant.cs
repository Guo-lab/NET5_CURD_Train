namespace ESC5.WebCommon
{
    /**
     * 本类用于定义全局常量，特别是与客户端一致使用的常量
     * @author rainy
     * @see --internal
     */
    public static class AppGlobalConstant
    {

        //与客户端一致使用的常量
        public static readonly string NAME_FORMTOKEN = "X-XSRF-TOKEN-FORM";
        public static readonly string NAME_LOGINTOKEN = "X-XSRF-TOKEN-LOGIN";
        public static readonly string NAME_CLIENTTYPE = "X-CLIENT-TYPE";

    }
}
