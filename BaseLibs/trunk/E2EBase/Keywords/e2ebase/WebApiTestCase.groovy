package e2ebase;

public class WebApiTestCase {
	/**
	 * api对应的aca,此处格式为/area/controller/action
	 */
	private String aca;
	/**
	 * url with qs
	 */
	private String url;
	/**
	 * request body
	 */
	private Object parameter;
	private Map<String,Object> param;
	/**
	 * 
	 */
	private RcResult result;
	private String method;//http method
	private boolean testable=true;//是否用于执行测试
	public boolean getTestable() { return testable; }
	private Boolean needLogin;//请求前需要先登录
	private Boolean fixed;//是否可被工具修改
	private Boolean fakeServer;//是否用作fakeserver

	public WebApiTestCase(String url,String aca,Object param,RcResult result) {
		constuct(url,aca,param,result);
	}
	/**
	 * 
	 * @param aca api对应的aca,此处格式为/area/controller/action
	 * @param param api请求参数
	 * @param result api响应结果，null则不检验结果
	 */
	public WebApiTestCase(String aca,Object param,RcResult result) {
		constuct(null,aca,param,result);
	}
	public WebApiTestCase(String aca,Object param,Map<String,Object> resultMap) {
		this(null,aca,param,resultMap);
	}
	public WebApiTestCase(String url,String aca,Object param,Map<String,Object> resultMap) {
		RcResult result=resultMap==null?(RcResult)null:new RcResult(resultMap);
		constuct(url,aca,param,result);
	
	}
	private void constuct(String url,String aca,Object param,RcResult result) {
		this.aca=aca;
		this.url=url==null?aca:url;
		this.parameter=param;
		try {
			this.param=(Map<String,Object>)param;
		}catch(Exception e) {}
		this.result=result;
	}
}
