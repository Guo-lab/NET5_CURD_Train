namespace ProjectBase.Web.Mvc {
    /**
     * 本类用于定义全局常量，包括与客户端一致使用的常量
     * @author rainy
     * @see --internal
     */
    public static class GlobalConstant
    {

        //与客户端一致使用的常量
        public static readonly string ViewModel_ClassSuffix = "VM";
        public static readonly string ViewModel_ClassSuffix2 = "ViewModel";
        public static readonly string ViewModel_InputSuffix = "Input";
        public static readonly string ViewModel_InputSuffix1 = "$Input";
        public static readonly string ViewModel_InputSuffix2 = "_Input";
        public static readonly string DictJsName = "Dict";
        public static readonly string UnSubmitNameMarker = "_";
        public static readonly string VmMeta_LeafElePrefix = "LeafEle#";
        public static readonly string ValidationType_ClassValidate = "classValidate";
        public static readonly string ValidationType_RemoteValPrefix = "remote";
        public static readonly string ValidationType_VmInnerValPrefix = "vminner_";
        public static readonly string RemoveValidation_ParamPlaceHolder = "##";
        public static readonly string ServerOnlyValidation_MessageKeyPrefix = "ServerOnly_";
        public static readonly string Event_ServerOnlyValidation_Fail = "ServerOnlyValidation_Fail";
        public static readonly string Key_For_ForViewModelOnly = "_ForViewModelOnly";
        public static readonly string Key_For_ForceViewName = "_ForceViewName";
        public static readonly string Key_For_ForSmallScreen = "_ForSmallScreen";
        public static readonly string Key_For_AjaxNav = "ajax-nav";
        public static readonly string Value_For_AjaxNavRoot = "root";
        public static readonly string Key_For_ForInstruct = "pbInstruct";

        //应用上下文初始参数
        public static readonly string ContextParam_Debug = "debug";
        public static readonly string ContextParam_LogRoot = "logRoot";
        public static readonly string ContextParam_RunAuthCheckingTool = "runAuthCheckingTool";
        public static readonly string ContextParam_ClientDateSerializeFormat = "clientDateSerializeFormat";
        public static readonly string ContextParam_TimeZone = "timeZone";

        //其它
        public static readonly string Encoding_Default = "UTF-8";
        public static readonly string Controller_ClassSuffix = "Ctrl";

        public static readonly string Request_Attr_ValGroups = "RainyArch_GroupedValidate_ValGroups";
        public const string ValGroup_Default = "_D";
        public const string ValGroup_Always = "_A";
    }
}
