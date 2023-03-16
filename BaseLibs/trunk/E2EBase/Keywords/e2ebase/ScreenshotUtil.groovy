package e2ebase

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.checkpoint.Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling
import com.kms.katalon.core.testcase.TestCase
import com.kms.katalon.core.testdata.TestData
import com.kms.katalon.core.testobject.TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import static com.kms.katalon.core.testcase.TestCaseFactory.*
import com.kms.katalon.core.configuration.RunConfiguration

import java.nio.file.Files

import internal.GlobalVariable

public class ScreenshotUtil {

	def static void takePageScreenshot() {
		takePageScreenshot(null,null);
	}
	def static void takePageScreenshot(String suffixName) {
		takePageScreenshot(suffixName,null);
	}
	def static void takePageScreenshot(String suffixName,String ext) {
		if(!GlobalVariable.EnableScreenshot) return;

		if(suffixName==null) {
			suffixName="";
		}
		if(ext==null) {
			ext=".jpg";
		}
		int order=0;
		String relativePath = getTestCaseRelativeId(BaseTestListener.getCurrentTextCaseId());
		String filename=RunConfiguration.getProjectDir()+"/Screenshot/"+relativePath+suffixName+"."+order.toString()+ext;

		WebUI.takeFullPageScreenshot(filename);
	}
}
