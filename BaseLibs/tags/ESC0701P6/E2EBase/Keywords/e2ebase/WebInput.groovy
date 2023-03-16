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
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI

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


class WebInput {
	/**
	 * Click and input to kendo numericbox
	 * @param to Katalon test object
	 */
	@Keyword
	def static kNumericBox(TestObject toInput,String valueText) {
		try {
			WebElement input = WebUI.findWebElement(toInput);
			WebElement span=input.findElement(By.xpath(".."));
			span.click()
			//WebUI.waitForElementPresent(to,GlobalVariable.WaitEleTimeout)
			WebUI.setText(toInput, valueText)
			KeywordUtil.markPassed("kendo NumericBox输入成功")
		} catch (WebElementNotFoundException e) {
			KeywordUtil.markFailed("kendo NumericBox未找到："+valueText)
		}
	}
	@Keyword
	def static kDatepicker(TestObject toInput,int day,int month,int year) {
		try {
			WebElement input = WebUI.findWebElement(toInput);
			WebElement btn=input.findElement(By.xpath("./following-sibling::span[1]/span[1]"));
			btn.click()
			String xpath="//*[@href = '#' and @title = '"+year+"年"+month+"月"+day+"日' and (text() = '"+day+"' or . = '"+day+"')]"
			WebElement dayLink=input.findElement(By.xpath(xpath));
			dayLink.click()
			KeywordUtil.markPassed("kendo 日期输入成功")
		} catch (WebElementNotFoundException e) {
			KeywordUtil.markFailed("kendo 日期控件未找到："+day)
		}
	}
	@Keyword
	def static kDatepicker(TestObject toInput,int day) {
		kDatepicker(toInput,day,10,2021);
	}
	@Keyword
	def static kSelect(TestObject toLabel,String selectedText) {
		try {
			WebElement wrapper = WebUI.findWebElement(toLabel).findElement(By.xpath("./following-sibling::span[1]"));
			wrapper.click()
			String xpath="//*[@role = 'option' and @class = 'k-item ng-scope' and (text() = '"+selectedText+"' or . = '"+selectedText+"')]"
			WebElement opt=wrapper.findElement(By.xpath(xpath));
			opt.click()
			KeywordUtil.markPassed("kendo下拉选择成功")
		} catch (WebElementNotFoundException e) {
			KeywordUtil.markFailed("kendo下拉控件未找到："+selectedText)
		}
	}
	@Keyword
	def static selectTableRows(TestObject toCheckbox,int ...index) {
		try {
			WebElement input = WebUI.findWebElement(toCheckbox);
			String ngModel=input.getAttribute("ng-model");
			WebElement tbody=input.findElement(By.xpath("ancestor::tbody[1]"));
			List<WebElement> inputs=tbody.findElements(By.xpath("descendant::input[@type = 'checkbox' and @ng-model = '"+ngModel+"']"));
			for(int i : index) {
				inputs.get(i).click()
			}
			KeywordUtil.markPassed("多行选择成功")
		} catch (WebElementNotFoundException e) {
			KeywordUtil.markFailed("多行选择控件未找到")
		}
	}
}