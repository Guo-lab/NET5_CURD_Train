package e2ebase

import org.openqa.selenium.WebElement

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows

import groovy.json.*;
import static org.junit.Assert.*
import internal.GlobalVariable

public class ClientScriptUtil {

	private static JsonSlurper jsonSlurper=new JsonSlurper();

	PBSelenium selenium;
	ClientScriptUtil(PBSelenium selenium){
		this.selenium=selenium;
	}

	def doInController(String script) {
		return doInControllerInternal(script,false,null);
	}
	def doInController(String script,Object[] params) {
		return doInControllerInternal(script,false,params);
	}
	def doInControllerApply(String script,Object[] params) {
		return doInControllerInternal(script,true,params);
	}
	def private doInControllerInternal(String script,boolean triggerInput,Object[] params) {
		if(params!=null) {
			script=replaceParam(script,params);
		}
		script="PB_Global_WebApiTestingInterceptor.TestingDoInController(function(c,scope){"+script+"});";
		WebUI.executeJavaScript(script,null);
	}
	def Map<String,Object> getInController(String scriptExpr) {
		String script="return PB_Global_WebApiTestingInterceptor.TestingGetInController(function(c,scope){ return "+scriptExpr+"});";
		String r= WebUI.executeJavaScript(script,null);
		Map<String,Object> rtn=jsonSlurper.parseText(r);
		return rtn;
	}
	def doCallAction(WebApiTestCase webapi) {
		doCallAction(webapi,"null",false);
	}
	def doCallAction(WebApiTestCase webapi,boolean byPb) {
		doCallAction(webapi,"null",byPb);
	}
	def doCallAction(WebApiTestCase webapi,String ajaxOpnJson) {
		doCallAction(webapi,ajaxOpnJson,false);
	}
	def doCallAction(WebApiTestCase webapi,String ajaxOpnJson,boolean byPb) {
		String sByPb=byPb?"true":"false";
		String bodyJson=JsonOutput.toJson(webapi.parameter);

		if(bodyJson==null) {
			bodyJson="null";
		}
		if(ajaxOpnJson==null || !ajaxOpnJson.startsWith("{")) ajaxOpnJson="null";

		String script="PB_Global_WebApiTestingInterceptor.TestingCallAction('"+webapi.url+"',"+bodyJson+","+ajaxOpnJson+","+sByPb+");";
		println script;
		WebUI.executeJavaScript(script,null);
	}

	def RcResult getRcResult(String aca) {
		boolean exists=wait({
			boolean ok=WebUI.executeJavaScript("return PB_Global_WebApiTestingInterceptor.HasResponse('"+aca+"');",null);
			return ok;
		});
		assertTrue("未等到webapi的响应"+aca,exists);
		String script="var r=PB_Global_WebApiTestingInterceptor.GetRcResultJson('"+aca+"');return r;";
		String r= WebUI.executeJavaScript(script,null);
		//println r;
		Map<String,Object> res=jsonSlurper.parseText(r);
		//println res;
		RcResult result=new RcResult(res.isRcResult,res.command,res.data);
		//println result;
		return result;
	}
	def verifyRcResult(RcResult actual, RcResult expected) {
		Map<String,Object> actualMap=[ok:actual.ok,command:actual.command,data:actual.data];
		Map<String,Object> expectedMap=[ok:expected.ok,command:expected.command,data:expected.data];
		println actualMap;
		AssertUtil.assertMapDataEquals(actualMap,expectedMap,"RcResult");
	}

	def boolean wait(Closure condition) {
		wait(condition,null);
	}
	def boolean wait(Closure condition,Integer timeoutMillisec) {
		if(timeoutMillisec==null) timeoutMillisec=GlobalVariable.WaitEleTimeout;
		while(timeoutMillisec>0){
			if (condition()) return true;
			timeoutMillisec=timeoutMillisec-100;
			Thread.sleep(100);
		}
		return false;
	}
	def static private String replaceParam(String js,Object[] params) {
		for(int i=0;i<params.length;i++) {
			if(params[i].class==String.class) {
				js=js.replaceFirst("\\?", "'"+params[i]+"'");
			}else {
				js=js.replaceFirst("\\?", params[i].toString());
			}
		}
		return js;
	}
}
