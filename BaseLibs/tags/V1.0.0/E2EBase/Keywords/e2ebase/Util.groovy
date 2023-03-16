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
	def static verifyNoJsError() {
		WebDriver webDriver = DriverFactory.getWebDriver()
		LogEntries logEntries=webDriver.manage().logs().get(LogType.BROWSER)
		String err=null;
		for(LogEntry entry : logEntries) {
			if(entry.getLevel()==Level.SEVERE && !entry.message.contains("favicon.ico")) {
				err=entry.message;
				break;
			}
		}
		if(err!=null) {
			KeywordUtil.markFailed("JS运行错："+err)
		}
	}
	@Keyword
	def static void runSql(String sql) {
		runSqlInternal(sql)
	}
	@Keyword
	def static void dbDelete(String table) {
		String sql="delete from "+table;
		runSqlInternal(sql)
	}
	@Keyword
	def static void dbDelete(String table,String where) {
		String sql="delete from "+table+" where "+where;
		runSqlInternal(sql)
	}
	@Keyword
	def static int dbCount(String table) {
		return (int)runScalar("select count(*) from "+table);
	}
	@Keyword
	def static int dbCount(String table,String where) {
		return (int)runScalar("select count(*) "+table+" where "+where);
	}
	@Keyword
	def static dbExists(String table) {
		return runScalar("select top 1 id from "+table)!=null;
	}
	@Keyword
	def static dbExists(String table,String where) {
		return runScalar("select top 1 id from "+table+" where "+where)!=null;
	}
	def static runSqlInternal(String sql) {
		DatabaseKeywords kws=new DatabaseKeywords();
		Connection connection = createConnection(kws)
		Object rtn=null;
		try {
			rtn=kws.execute(connection, sql)//executeUpdate无法捕获异常，只好用execute
		}catch(SQLException e) {
			KeywordUtil.markFailed("sql运行错："+sql+'\r\n'+e.message)
			throw e
		}finally {
			kws.closeConnection(connection)
		}
	}
	def static Object runScalar(String sql) {
		DatabaseKeywords kws=new DatabaseKeywords();
		Connection connection = createConnection(kws)
		Object scalar=null
		ResultSet rs=null;
		try {
			rs=kws.executeQuery(connection, sql)
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
}