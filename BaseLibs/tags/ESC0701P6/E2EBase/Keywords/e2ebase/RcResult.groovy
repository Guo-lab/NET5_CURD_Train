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
}
