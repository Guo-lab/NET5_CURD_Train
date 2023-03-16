package e2ebase
import internal.GlobalVariable

import org.openqa.selenium.WebElement
import org.openqa.selenium.WebDriver
import org.openqa.selenium.By

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.webui.driver.DriverFactory
import com.kms.katalon.core.webui.exception.WebElementNotFoundException
import com.thoughtworks.selenium.webdriven.WebDriverBackedSelenium

import com.kms.katalon.core.util.KeywordUtil
import java.util.logging.Level
import org.openqa.selenium.logging.*

class ElementUtil{
	PBSelenium selenium;
	ElementUtil(PBSelenium selenium){
		this.selenium=selenium;
	}
	def WebElement findKSelect(String locator) {
		return findKSelect(findKSelectWrap(locator))
	}
	def WebElement findKSelect(WebElement kselectWrap) {
		return kselectWrap.findElement(By.xpath("./descendant::select[@kendo-drop-down-list]"))
	}
	def WebElement findKSelectWrap(String locator) {
		return findKSelectWrap(selenium.findElement(locator))
	}
	def WebElement findKSelectWrap(WebElement ele) {
		return ele.findElement(By.xpath("./ancestor::span[contains(@class,'k-dropdown')]"))
	}
	def WebElement findButtonInTableToClick(String locator,String buttonText) {
		WebElement btn=selenium.waitEle(locator)
		if(btn!=null && btn.enabled) return btn

		int pos=locator.indexOf('/tbody')
		String tbodyLocator=locator.substring(0,pos)

		String btnLocator=tbodyLocator+"/tbody/descendant::button[not(@disabled)"
		if(buttonText==null) {
			btnLocator=btnLocator+"]"
		}else {
			btnLocator=btnLocator+" and text()='"+buttonText+"']"
		}
		return selenium.findElement(btnLocator)
	}
	def String findKGridCellText(String headerLocator) {
		WebElement header=selenium.waitEle(headerLocator)
		header=header.findElement(By.xpath("./ancestor-or-self::th"));
		int colIndex=header.findElements(By.xpath("./preceding-sibling::*")).size()+1;
		WebElement tbody=header.findElement(By.xpath("./ancestor::div[@class='k-grid-header']/following-sibling::div/descendant::tbody"));
		List<WebElement> rows=tbody.findElements(By.xpath('./descendant::tr'));
		for(WebElement row : rows) {
			WebElement cell=row.findElement(By.xpath("./td["+colIndex+"]"))
			if(cell.getText().isEmpty()) continue;
			return cell.getText()
		}
	}
}