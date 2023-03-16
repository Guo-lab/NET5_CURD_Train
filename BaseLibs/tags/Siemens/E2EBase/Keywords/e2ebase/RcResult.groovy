package e2ebase;

public class RcResult {
	private boolean ok;
	private boolean isRcResult;

	private String command;
	private Object data;

	public boolean getOk() {
		return ok;
	}

	public RcResult(boolean ok,String command,Object data) {
		this.ok=ok;
		this.command=command;
		this.data=data;
	}
	public RcResult(Map<String,Object> rcResult) {
		if(rcResult.containsKey("isRcResult")) {
			this.ok=rcResult.isRcResult;
		}else if(rcResult.containsKey("ok")) {
			this.ok=rcResult.ok;
		}else if(rcResult.containsKey("Ok")) {
			this.ok=rcResult.Ok;
		}
		this.command=rcResult.command;
		this.data=rcResult.data;
	}

	public static String Command_Noop = "Noop";
	public static String Command_Message = "Message";
	public static String Command_Redirect = "Redirect";
	public static String Command_AppPage = "AppPage";
	public static String Command_ServerVM = "ServerVM";
	public static String Command_ServerData = "ServerData";
	public static String Command_BizException = "BizException";
	public static String Command_Exception = "Exception";

	public static Map<String,Object> Noop(){
		return [isRcResult:true,command:Command_Noop];
	}
	public static Map<String,Object> Message(String msg) {
		return [isRcResult:true,command:Command_Message,data:msg];
	}
	public static Map<String,Object> VM(Map<String,Object> vmData) {
		return [isRcResult:true,command:Command_ServerVM,data:[ViewModel:vmData]];
	}
	public static Map<String,Object> Data(Object data) {
		return [isRcResult:true,command:Command_ServerData,data:data];
	}
	public static Map<String,Object> BizException(String bizExceptionString) {
		return [isRcResult:false,command:Command_BizException,data:bizExceptionString];
	}
	
	public static Map<String,Object> Data() {
		return [isRcResult:true,command:Command_ServerData,data:"@@any"];
	}
	public static Map<String,Object> VM() {
		return [isRcResult:true,command:Command_ServerVM,data:"@@any"];
	}
	public static Map<String,Object> Message() {
		return [isRcResult:true,command:Command_Message,data:"@@any"];
	}
	public static Map<String,Object> BizException() {
		return [isRcResult:false,command:Command_BizException,data:"@@any"];
	}
}
