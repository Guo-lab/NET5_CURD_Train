package e2ebase

import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject

import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject

import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile

import internal.GlobalVariable as GlobalVariable

import com.kms.katalon.core.annotation.BeforeTestCase
import com.kms.katalon.core.annotation.BeforeTestSuite
import com.kms.katalon.core.annotation.AfterTestCase
import com.kms.katalon.core.annotation.AfterTestSuite
import com.kms.katalon.core.context.TestCaseContext
import com.kms.katalon.core.context.TestSuiteContext

public class BaseTestListener {

	static boolean isInSuite=false;
	static Map<String,Closure> cleanupDBFuncMap=new HashMap<String,Closure>();
	static Map<String,Closure> cleanupFuncMap=new HashMap<String,Closure>();

	static String currentTextCaseId;

	protected boolean useBrowser=true;
	/**
	 * Executes before every test suite starts.
	 * @param testSuiteContext: related information of the executed test suite.
	 */
	@BeforeTestSuite
	def beforeTestSuite(TestSuiteContext testSuiteContext) {
		isInSuite=true;
	}

	/**
	 * Executes after every test suite ends.
	 * @param testSuiteContext: related information of the executed test suite.
	 */
	@AfterTestSuite
	def afterTestSuite(TestSuiteContext testSuiteContext) {
		CrossExeTestData.save();
	}

	/**
	 * Executes before every test case starts.
	 * @param testCaseContext related information of the executed test case.
	 */
	@BeforeTestCase
	def beforeTestCase(TestCaseContext testCaseContext) {
		currentTextCaseId=testCaseContext.testCaseId;
		TestCase defaultLoginTC=findTestCase(GlobalVariable.DefaultLoginTC);
		if(!isInSuite
		&& GlobalVariable.UseLoginCoat
		&& defaultLoginTC!=null
		&& !currentTextCaseId.equalsIgnoreCase(defaultLoginTC.testCaseId)
		&& GlobalVariable.UseLoginCoatExclude.find({ currentTextCaseId.endsWith(it)})==null) {
			WebUI.callTestCase(defaultLoginTC, [:], FailureHandling.STOP_ON_FAILURE)
		}
		GlobalVariable.UseDBNum=1;
	}

	/**
	 * Executes after every test case ends.
	 * @param testCaseContext related information of the executed test case.
	 */
	@AfterTestCase
	def afterTestCase(TestCaseContext testCaseContext) {
		if(!isInSuite) {
			CrossExeTestData.save();
		}
		if(cleanupDBFuncMap[testCaseContext.testCaseId]!=null) {
			BaseRunner.setStepLogDT();
			cleanupDBFuncMap[testCaseContext.testCaseId]();
			BaseRunner.logStep("cleanupDB");
		}
		if(cleanupFuncMap[testCaseContext.testCaseId]!=null) {
			BaseRunner.setStepLogDT();
			cleanupFuncMap[testCaseContext.testCaseId]();
			BaseRunner.logStep("cleanup");
		}
		if(!isInSuite
		&& testCaseContext.getTestCaseStatus().equalsIgnoreCase("Passed")
		&& !testCaseContext.testCaseId.equalsIgnoreCase(GlobalVariable.DefaultLoginTC)
		&& useBrowser) {
			WebUI.closeBrowser()
		}
	}

	def boolean isDebug() {
		return GlobalVariable.RunMode.equals(GlobalConstant.RUN_MODE_DEBUG);
	}
}
