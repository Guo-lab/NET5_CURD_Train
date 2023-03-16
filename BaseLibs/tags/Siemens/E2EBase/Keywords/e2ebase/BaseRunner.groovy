package e2ebase

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.webui.exception.*

import internal.GlobalVariable
import static org.junit.Assert.*

class BaseRunner {

	//	@Deprecated
	//	@Keyword
	//	def static void start(Closure setupDBFunc,String enterUrl) {
	//		Closure enterFunc= {new PBSelenium().open(enterUrl)};
	//		start(setupDBFunc,null,enterFunc,null,false);
	//	}
	//	@Deprecated
	//	@Keyword
	//	def static void start(Closure setupDBFunc,Closure enterFunc) {
	//		start(setupDBFunc,null,enterFunc,null,false);
	//	}
	//	@Deprecated
	//	@Keyword
	//	def static void start(Closure setupDBFunc,String enterUrl,Closure cleanupDBFunc) {
	//		start(setupDBFunc,null,enterUrl,cleanupDBFunc,false);
	//	}
	//	@Deprecated
	//	@Keyword
	//	def static void start(Closure setupDBFunc,Closure setupFunc,String enterUrl) {
	//		start(setupDBFunc,setupFunc,enterUrl,null);
	//	}
	//	@Deprecated
	//	@Keyword
	//	def static void start(Closure setupDBFunc,Closure setupFunc,String enterUrl,Closure cleanupDBFunc) {
	//		Closure enterFunc= {new PBSelenium().open(enterUrl)};
	//		start(setupDBFunc,setupFunc,enterFunc,cleanupDBFunc,false);
	//	}
	//	@Deprecated
	//	@Keyword
	//	def static void start(Closure setupDBFunc,Closure setupFunc,Closure enterFunc) {
	//		start(setupDBFunc,setupFunc,enterFunc,null,false);
	//	}
	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,Closure enterFunc,Closure cleanupDBFunc) {
		start(setupDBFunc,setupFunc,enterFunc,cleanupDBFunc,false);
	}
	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,WebApiTestCase apiEnter,Closure cleanupDBFunc) {
		start(setupDBFunc,setupFunc,apiEnter,cleanupDBFunc,false);
	}
	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,WebApiTestCase apiEnter,Closure cleanupDBFunc,boolean force) {
		Closure enterFunc= {new PBSelenium().enter(apiEnter)};
		start(setupDBFunc,setupFunc,enterFunc,cleanupDBFunc,force);
	}
	@Keyword
	def static void start(Closure setupDBFunc,Closure setupFunc,Closure enterFunc,Closure cleanupDBFunc,boolean force) {
		if(setupDBFunc==null||setupFunc||enterFunc||cleanupDBFunc) {
			//throw new Exception("参数不可为空");
		}
		if(cleanupDBFunc!=null) {
			cleanupDB(cleanupDBFunc,force);
		}
		if(setupDBFunc!=null) {
			setupDB(setupDBFunc,force);
		}
		if(setupFunc!=null) {
			setup(setupFunc,force);
		}
		if(enterFunc!=null) {
			enter(enterFunc,force);
			try {
				Util.verifyNoJsError("进入页面时");
			}catch(BrowserNotOpenedException e) {
			}
		}
	}
	@Keyword
	def static void end(Closure cleanupDBFunc) {
		if(cleanupDBFunc!=null) {
			cleanupDB(cleanupDBFunc);
		}
	}


	def static boolean shouldDoEnter() {
		return GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_DEBUG) || GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_SINGLE);
	}
	def static boolean shouldDoSetupDB() {
		return GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_DEBUG) || GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_SINGLE);
	}
	def static boolean shouldDoCleanupDB() {
		return GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_DEBUG) || GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_SINGLE);
	}
	def static boolean shouldDoSetup() {
		return GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_DEBUG) || GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_SINGLE);
	}
	def static boolean enter() {
		return GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_DEBUG) || GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_SINGLE);
	}
	def static boolean setupDB(Closure setupDBFunc,boolean force) {
		if(!force && !shouldDoSetupDB()) return false;
		setupDBFunc();
		return true;
	}
	def static boolean setupDB(Closure setupDBFunc) {
		return setupDB(setupDBFunc,false);
	}
	def static boolean cleanupDB(Closure cleanupDBFunc,boolean force) {
		if(!force && !shouldDoCleanupDB()) return false;
		cleanupDBFunc();
		return true;
	}
	def static boolean cleanupDB(Closure cleanupDBFunc) {
		return cleanupDB(cleanupDBFunc,false);
	}
	def static boolean enter(Closure enterFunc,boolean force) {
		if(!force && !shouldDoEnter()) return false;
		enterFunc();
		return true;
	}
	def static boolean enter(Closure enterFunc) {
		return enter(enterFunc,false);
	}
	def static boolean setup(Closure setupFunc,boolean force) {
		if(!force && !shouldDoSetup()) return false;
		setupFunc();
		return true;
	}
	def static boolean setup(Closure setupFunc) {
		return setup(setupFunc,false);
	}
	@Keyword
	def static boolean stopOnDebug() {
		String err;
		try {
			err=Util.verifyNoJsError(1000,"");
		}catch(BrowserNotOpenedException e) {
		}
		String msg="OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK- 当前运行模式为开发过程中辅助调试，后续动作需手动执行，如verify等。OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OK-OKOK-OK-OK-OK-OK-OK";
		if(err!=null) {
			msg=err+"\r\n\r\n"+msg;
		}
		if(GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_DEBUG)) {
			assertTrue(msg,false);
		}
	}
}
