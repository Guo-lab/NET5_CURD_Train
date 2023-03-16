export const ClientServerConst = { // 这里的常量与服务器GlobalConstant/ClientServerConst一致以互相配合使用
    AuthFailure: '_AuthFailure',
    KeyForViewModelOnly: '_ForViewModelOnly',
    KeyForSmallScreen: '_ForSmallScreen',
    KeyForKeepDateAsNumber:'KeepDateAsNumber',
    Command_Noop: 'Noop',
    Command_Message: 'Message',
    Command_Event: 'Event',
    Command_Redirect: 'Redirect',  // redirect to component
    Command_AppPage: 'AppPage', // reenter the onepage app
    Command_ServerVM: 'ServerVM',
    Command_ServerVMPatch: 'ServerVMPatch',
    Command_ServerScrollList: 'ServerScrollList',
    Command_ServerData: 'ServerData',
    Command_ChangedData: 'ChangedData',
    Command_Exception: 'Exception',
    Command_BizException : "BizException",
    Command_AjaxNavVM: 'AjaxNavVM',
    Command_HttpStatus: 'HttpStatus',
    Command_NewWindow: 'NewWindow', // open a new window
    ViewModel_InputSuffix: '$Input',
    ViewModel_InputSuffix2: '_Input',
    VmMeta_LeafElePrefix: 'LeafEle#',
    VmMeta_DefaultRuleGroup: '_D',
    VmMeta_AlwaysRuleGroup: '_A',
    ValidationType_ClassValidate: 'classValidate',
    ValidationType_RemoteValPrefix: 'remote',
    ValidationType_VmInnerValPrefix: 'vminner_',
    RemoveValidation_ParamPlaceHolder: '##',
    Event_ServerOnlyValidation_Fail: 'ServerOnlyValidation_Fail',
    Message_SaveSuccessfully: 'M.Save_Successfully',
    Message_OperateSuccessfully: 'M.OperateSuccessfully',
    NAME_FORMTOKEN: "X-XSRF-TOKEN-FORM",
    NAME_LOGINTOKEN: "X-XSRF-TOKEN-LOGIN",
    NAME_CLIENTTYPE:'X-CLIENT-TYPE',


    CLIENT_PROMISE_MANAGER_CALL_BACK_METHOD : "ClientPromiseManager_CallbackOnServerComplete",
    CLIENT_PROMISE_SERVER_RETURN_STATUS_RESOLVED : "Resolved",
    CLIENT_PROMISE_SERVER_RETURN_STATUS_REJECTED : "Rejected",
    FRONT_CONTROLLER_METHOD_HANDLE_SIGNALR_REQUEST : "HandleSignalRRequest",
    CLIENT_CALLBACK_ON_SERVER_RETURN : "OnServerReturn",
    CLIENT_CALLBACK_ON_SERVER_ERROR : "OnServerError"
};
