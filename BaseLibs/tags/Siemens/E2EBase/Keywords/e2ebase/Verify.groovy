package e2ebase

import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject

import org.openqa.selenium.WebElement
import org.openqa.selenium.WebDriver
import org.openqa.selenium.By
import org.openqa.selenium.support.ui.Select

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.webui.driver.DriverFactory
import com.kms.katalon.core.webui.exception.WebElementNotFoundException
import com.thoughtworks.selenium.webdriven.WebDriverBackedSelenium

import com.kms.katalon.core.util.KeywordUtil
import static org.junit.Assert.*

import internal.GlobalVariable

public class Verify {
	private static PBSelenium selenium=new PBSelenium()

	@Keyword
	def static void kselectContains(String locator,String itemText) {
		Select select=new Select(selenium.EleUtil.findKSelect(locator))
		for(WebElement opn : select.options) {
			String text=opn.getAttribute('innerHTML')//不能用getText()因为不可见就会返回空串
			if(text.equalsIgnoreCase(itemText)) return;
		}
		assertTrue("下拉列表中不包含指定项：name="+select.element.getAttribute('name')+" 指定项="+itemText,false)
	}
	def WebElement kpagerInfo(String ktableLocator,int totalCount) {
		WebElement ele=selenium.waitEle(ktableLocator+"/descendant::span[contains(@class,'k-pager-info')]")
		int pos1=ele.getText().indexOf('共')
		int pos2=ele.getText().indexOf('条数据')
		int cnt=ele.getText().substring(pos1,pos2).trim().toInteger()
		assertEquals("kendo table 总记录数检验未通过",totalCount,cnt)
	}
}
