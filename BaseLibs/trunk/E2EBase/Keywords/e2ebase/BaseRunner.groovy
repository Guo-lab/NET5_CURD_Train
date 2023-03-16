package e2ebase

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.webui.exception.*
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.util.KeywordUtil

import internal.GlobalVariable
import static org.junit.Assert.*

class BaseRunner {
	private static Calendar lastStepLogDT;

	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,Closure enterFunc,Closure cleanupDBFunc) {
		start(setupDBFunc,setupFunc,enterFunc,cleanupDBFunc,[:]);
	}
	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,WebApiTestCase apiEnter,Closure cleanupDBFunc) {
		Closure enterFunc= {new PBSelenium().enter(apiEnter)};
		start(setupDBFunc,setupFunc,enterFunc,cleanupDBFunc,[:]);
	}
	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,Closure enterFunc,Closure cleanupDBFunc,Closure cleanupFunc) {
		start(setupDBFunc,setupFunc,enterFunc,cleanupDBFunc,[cleanupFunc:cleanupFunc]);
	}
	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,WebApiTestCase apiEnter,Closure cleanupDBFunc,Closure cleanupFunc) {
		Closure enterFunc= {new PBSelenium().enter(apiEnter)};
		start(setupDBFunc,setupFunc,enterFunc,cleanupDBFunc,[cleanupFunc:cleanupFunc]);
	}
	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,Closure enterFunc,Closure cleanupDBFunc,Map<String,Object> extra) {
		if(setupDBFunc==null||setupFunc==null||enterFunc==null||cleanupDBFunc==null) {
			throw new Exception("参数不可为空");
		}
		Closure cleanupFunc=null;
		boolean force=false;
		boolean cleanupDBFirst=true;
		boolean cleanupFirst=false;
		if(extra.containsKey("cleanupFunc") && extra.cleanupFunc!=null) {
			cleanupFunc=extra.cleanupFunc;
		}
		if(extra.containsKey("force") && extra.force!=null) {
			force=extra.force;
		}
		if(extra.containsKey("cleanupDBFirst") && extra.cleanupDBFirst!=null) {
			cleanupDBFirst=extra.cleanupDBFirst;
		}
		if(extra.containsKey("cleanupFirst") && extra.cleanupFirst!=null) {
			cleanupFirst=extra.cleanupFirst;
		}

		if(cleanupDBFunc!=null) {
			registerCleanupDB(cleanupDBFunc,force);
			if(cleanupDBFirst && shouldDoCleanupDBBegoreSetupDB()) {
				setStepLogDT();
				cleanupDBFunc();
				logStep("cleanupDB");
			}
		}
		if(cleanupFunc!=null) {
			registerCleanup(cleanupFunc,force);
			if(cleanupFirst) {
				setStepLogDT();
				cleanupFunc();
				logStep("cleanup");
			}
		}
		if(setupDBFunc!=null) {
			doSetupDB(setupDBFunc,force);
		}
		if(setupFunc!=null) {
			doSetup(setupFunc,force);
		}
		if(enterFunc!=null) {
			doEnter(enterFunc,force);
			try {
				Util.verifyNoJsError("进入页面时");
			}catch(BrowserNotOpenedException e) {
			}
			ScreenshotUtil.takePageScreenshot();
		}
	}
	@Keyword
	def static void end() {
		String err;
		try {
			err=Util.verifyNoJsError(1000,"");
		}catch(BrowserNotOpenedException e) {
		}
		if(err!=null) {
			assertTrue("页面JS报错："+err,false);
		}
	}

	def static boolean shouldDoEnter() {
		return BaseTestListener.isInSuite || !GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_COMBO);
	}
	def static boolean shouldDoSetupDB() {
		return BaseTestListener.isInSuite || !GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_COMBO);
	}
	def static boolean shouldRegisterCleanups() {
		return BaseTestListener.isInSuite || GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_SINGLE) || GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_SUITE);
	}
	def static boolean shouldDoCleanupDBBegoreSetupDB() {
		return BaseTestListener.isInSuite || !GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_COMBO);
	}
	def static boolean shouldDoSetup() {
		return BaseTestListener.isInSuite || !GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_COMBO);
	}
	def static boolean doSetupDB(Closure setupDBFunc,boolean force) {
		if(!force && !shouldDoSetupDB()) return false;
		setStepLogDT();
		setupDBFunc();
		logStep("setupDB");
		return true;
	}
	def static boolean doSetupDB(Closure setupDBFunc) {
		return doSetupDB(setupDBFunc,false);
	}
	def static boolean registerCleanupDB(Closure cleanupDBFunc,boolean force) {
		if(!force && !shouldRegisterCleanups()) return false;
		BaseTestListener.cleanupDBFuncMap[BaseTestListener.currentTextCaseId]=cleanupDBFunc;
		return true;
	}
	def static boolean registerCleanupDB(Closure cleanupDBFunc) {
		return registerCleanupDB(cleanupDBFunc,false);
	}
	def static boolean registerCleanup(Closure cleanupFunc,boolean force) {
		if(!force && !shouldRegisterCleanups()) return false;
		BaseTestListener.cleanupFuncMap[BaseTestListener.currentTextCaseId]=cleanupFunc;
		return true;
	}
	def static boolean registerCleanup(Closure cleanupFunc) {
		return registerCleanup(cleanupFunc,false);
	}
	def static boolean doEnter(Closure enterFunc,boolean force) {
		if(!force && !shouldDoEnter()) return false;
		setStepLogDT();
		enterFunc();
		logStep("enter");
		return true;
	}
	def static boolean doEnter(Closure enterFunc) {
		return doEnter(enterFunc,false);
	}
	def static boolean doSetup(Closure setupFunc,boolean force) {
		if(!force && !shouldDoSetup()) return false;
		setStepLogDT();
		setupFunc();
		logStep("setup");
		return true;
	}
	def static boolean doSetup(Closure setupFunc) {
		return doSetup(setupFunc,false);
	}
	@Keyword
	def static boolean stopOnDebug() {
		String err;
		try {
			err=Util.verifyNoJsError(1000,"");
		}catch(BrowserNotOpenedException e) {
		}
		String msg="OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK- 测试通过！！！ 当前运行模式为开发过程中辅助调试，后续动作需手动执行，如verify等。OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK";
		msg=msg+"OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK";
		msg=msg+"OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK";
		msg=msg+"OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK";

		if(err!=null) {
			msg=err+"\r\n\r\n"+msg;
		}
		if(GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_DEBUG)) {
			assertTrue(msg,false);
		}
	}
	def static void setStepLogDT() {
		lastStepLogDT=Calendar.getInstance();
	}
	def static void logStep(String msg) {
		logStep(msg,"");
	}
	def static void logStep(String msg,String longMsg) {
		Calendar dt=Calendar.getInstance();
		BigDecimal diff=(dt.getTimeInMillis()-lastStepLogDT.getTimeInMillis())/1000;
		KeywordUtil.logInfo(dt.format("yyyy/MM/dd HH:mm:ss")+" do=> "+msg+" ("+diff+"s)"+"            "+longMsg);
		lastStepLogDT=dt;
	}
}
