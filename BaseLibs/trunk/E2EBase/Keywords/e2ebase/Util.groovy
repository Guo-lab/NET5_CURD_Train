package e2ebase
import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.checkpoint.Checkpoint
import com.kms.katalon.core.checkpoint.CheckpointFactory
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords
import com.kms.katalon.core.model.FailureHandling
import com.kms.katalon.core.testcase.TestCase
import com.kms.katalon.core.testcase.TestCaseFactory
import com.kms.katalon.core.testdata.TestData
import com.kms.katalon.core.testdata.TestDataFactory
import com.kms.katalon.core.testobject.ObjectRepository
import com.kms.katalon.core.testobject.TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords

import internal.GlobalVariable

import org.openqa.selenium.WebElement
import org.openqa.selenium.WebDriver
import org.openqa.selenium.By

import com.kms.katalon.core.mobile.keyword.internal.MobileDriverFactory
import com.kms.katalon.core.webui.driver.DriverFactory

import com.kms.katalon.core.testobject.RequestObject
import com.kms.katalon.core.testobject.ResponseObject
import com.kms.katalon.core.testobject.ConditionType
import com.kms.katalon.core.testobject.TestObjectProperty

import com.kms.katalon.core.mobile.helper.MobileElementCommonHelper
import com.kms.katalon.core.util.KeywordUtil

import com.kms.katalon.core.webui.exception.WebElementNotFoundException
import org.openqa.selenium.logging.*
import java.util.logging.Level
import java.sql.*
import com.katalon.plugin.keyword.connection.DBType
import com.katalon.plugin.keyword.connection.DatabaseKeywords
import com.katalon.plugin.keyword.connection.ResultSetKeywords

import com.thoughtworks.selenium.webdriven.WebDriverBackedSelenium

class Util {
	@Keyword
	def static void wait(int millisec) {
		Thread.sleep((millisec*GlobalVariable.WaitRate).longValue());
	}
	@Keyword
	def static String verifyNoJsError() {
		return verifyNoJsError(GlobalVariable.WaitJsErrorTimeout,"");
	}
	@Keyword
	def static String verifyNoJsError(String msgPrefix) {
		return verifyNoJsError(GlobalVariable.WaitJsErrorTimeout,msgPrefix);
	}
	@Keyword
	def static String verifyNoJsError(int timeoutMillisec,String msgPrefix) {
		WebDriver webDriver = DriverFactory.getWebDriver()
		String err=null;
		while(timeoutMillisec>0){
			err=getJsError(webDriver);
			if(err!=null) break;
			timeoutMillisec=timeoutMillisec-500;
			Thread.sleep(500)
		}
		if(err!=null) {
			err=msgPrefix+" JS运行错："+err;
			KeywordUtil.markFailed(err);
		}
		return err;
	}

	def static String getJsError(WebDriver webDriver) {
		LogEntries logEntries=webDriver.manage().logs().get(LogType.BROWSER)
		String err=null;
		for(LogEntry entry : logEntries) {
			if(entry.getLevel()==Level.SEVERE && !JsErrorIgnore.keywords.any { k-> entry.message.contains(k)}) {
				err=entry.message;
				break;
			}
		}
		return err;
	}

	@Keyword
	def static void runSql(String sql) {
		runSqlInternal(sql)
	}
	@Keyword
	def static void runSql(String sql,Object[] params) {
		runSqlInternal(replaceParam(sql,params));
	}
	@Keyword
	def static void dbDelete(String table,String where) {
		String sql="delete from "+table+" where "+where;
		runSqlInternal(sql)
	}
	@Keyword
	def static void dbDelete(String table,long Id) {
		String sql="delete from "+table+" where id="+Id;
		runSqlInternal(sql)
	}
	@Keyword
	def static void dbDelete(String table,String where,Object[] params) {
		dbDelete(table,replaceParam(where,params));
	}
	@Keyword
	def static long dbInsert(String sql,Object[] params) {
		return dbInsert(replaceParam(sql,params));
	}
	def static private String replaceParam(String sql,Object[] params) {
		for(int i=0;i<params.length;i++) {
			if(params[i].class==String.class) {
				sql=sql.replaceFirst("\\?", "'"+params[i]+"'");
			}else {
				sql=sql.replaceFirst("\\?", params[i].toString());
			}
		}
		return sql;
	}
	@Keyword
	def static long dbInsert(String sql) {
		if(!sql.startsWith("insert")) {
			sql="insert into "+sql;
		}

		DatabaseKeywords kws=new DatabaseKeywords();
		Connection connection = createConnection(kws)
		long generatedId=0;
		try {
			PreparedStatement stmt=connection.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS);
			stmt.executeUpdate();
			printSql(sql)
			ResultSet rs=stmt.getGeneratedKeys();
			if(rs.next()) {
				generatedId=rs.getLong(1);
			}
			return generatedId;
		}catch(SQLException e) {
			KeywordUtil.markFailed("sql运行错："+sql+'\r\n'+e.message)
			throw e
		}finally {
			kws.closeConnection(connection)
		}
	}
	@Keyword
	def static Object dbUpdate(String sql,String where) {
		if(!sql.startsWith("update")) {
			sql="update "+sql+" where "+where;
		}
		return runSqlInternal(sql);
	}
	@Keyword
	def static Object dbUpdate(String sql,String where,Object[] params) {
		if(!sql.startsWith("update")) {
			sql="update "+sql+" where "+where;
		}
		return runSqlInternal(replaceParam(sql,params));
	}
	@Keyword
	def static int dbCount(String table) {
		return dbCount(table,"1=1");
	}
	@Keyword
	def static int dbCount(String table,String where) {
		return Integer.valueOf(runScalar("select count(*) from "+table+" where "+where));
	}
	@Keyword
	def static int dbCount(String table,String where,Object[] params) {
		return dbCount(table,replaceParam(where,params));
	}
	@Keyword
	def static boolean dbExists(String table) {
		return runScalar("select top 1 id from "+table)!=null;
	}
	@Keyword
	def static boolean dbExists(String table,String where) {
		return runScalar("select top 1 id from "+table+" where "+where)!=null;
	}
	@Keyword
	def static boolean dbExists(String table,String where,Object[] params) {
		return dbExists(table,replaceParam(where,params));
	}
	@Keyword
	def static int getId(String table,String where) {
		return Integer.valueOf(runScalar("select top 1 id from "+table+" where "+where));
	}
	@Keyword
	def static int getId(String table,String where,Object[] params) {
		return getId(table,replaceParam(where,params));
	}
	def static runSqlInternal(String sql) {
		DatabaseKeywords kws=new DatabaseKeywords();
		Connection connection = createConnection(kws)
		Object rtn=null;
		try {
			rtn=kws.execute(connection, sql)//executeUpdate无法捕获异常，只好用execute
			printSql(sql)
		}catch(SQLException e) {
			KeywordUtil.markFailed("sql运行错："+sql+'\r\n'+e.message)
			throw e
		}finally {
			kws.closeConnection(connection)
		}
		return rtn;
	}
	def static Object getScalar(String sql) {
		return runScalar(sql);
	}
	def static Object runScalar(String sql) {
		DatabaseKeywords kws=new DatabaseKeywords();
		Connection connection = createConnection(kws);
		Object scalar=null;
		ResultSet rs=null;
		try {
			rs=kws.executeQuery(connection, sql)
			printSql(sql)
			ResultSetKeywords rsk=new ResultSetKeywords()
			if(!rsk.isEmptyResult(rs)) {
				scalar=rsk.getSingleCellValue(rs, 1, 1)
			}
		}catch(Exception e) {
			//keyword异常无法在此捕获
		}finally {
			kws.closeConnection(connection)
		}
		if(rs!=null)return scalar;
		KeywordUtil.markFailed("sql运行错："+sql)
	}
	def static Connection createConnection(DatabaseKeywords kws) {
		String pass=null;
		if(GlobalVariable.UseDBNum==1) {
			pass=kws.base64Encode(GlobalVariable.DBPass)
			return kws.createConnection(DBType.sqlserver, GlobalVariable.DBServer, GlobalVariable.DBPort,GlobalVariable.DBSchema, GlobalVariable.DBUser,pass )
		}else if(GlobalVariable.UseDBNum==2) {
			pass=kws.base64Encode(GlobalVariable.DBPass2)
			return kws.createConnection(DBType.sqlserver, GlobalVariable.DBServer2, GlobalVariable.DBPort2,GlobalVariable.DBSchema2, GlobalVariable.DBUser2,pass )
		}
	}
	private def static void printSql(String sql) {
		if(GlobalVariable.ShowSql) {
			println sql;
		}
	}
}