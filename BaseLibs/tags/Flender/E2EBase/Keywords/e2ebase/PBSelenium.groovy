package e2ebase
import internal.GlobalVariable

import org.openqa.selenium.WebElement
import org.openqa.selenium.WebDriver
import org.openqa.selenium.By

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.webui.driver.DriverFactory
import com.kms.katalon.core.webui.exception.WebElementNotFoundException
import com.thoughtworks.selenium.webdriven.WebDriverBackedSelenium
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI

import com.kms.katalon.core.util.KeywordUtil
import java.util.logging.Level
import org.openqa.selenium.logging.*
import java.util.function.*
import static org.junit.Assert.*

class PBSelenium extends WebDriverBackedSelenium{

	def ElementUtil EleUtil;
	PBSelenium(){
		super(DriverFactory.getWebDriver(), "http://localhost")
		EleUtil=new ElementUtil(this)
	}


	//waitEle--------------------------------------------
	@Keyword
	def WebElement waitEle(String locator) {
		return waitEleInternal(locator,null,null,null,null,GlobalVariable.WaitEleTimeout);
	}
	@Keyword
	def WebElement waitEle(String locator,int timeoutMillisec) {
		return waitEleInternal(locator,false,null,null,null,timeoutMillisec)
	}
	@Keyword
	def WebElement waitEle(String locator,String attrValue) {
		return waitEleInternal(locator,true,attrValue,null,null,null)
	}
	@Keyword
	def WebElement waitEle(String locator,String attrValue,int timeoutMillisec) {
		return waitEleInternal(locator,true,attrValue,null,null,timeoutMillisec)
	}
	@Keyword
	def WebElement waitEle(String locator,String attrValue, String attrName) {
		return waitEleInternal(locator,true,attrValue,attrName,null,null)
	}
	@Keyword
	def WebElement waitEle(String locator,String attrValue, String attrName,int timeoutMillisec) {
		return waitEleInternal(locator,true,attrValue,attrName,null,timeoutMillisec)
	}
	@Keyword
	def WebElement waitEle(String locator,Closure func) {
		return waitEleInternal(locator,true,null,null,func,null)
	}
	@Keyword
	def WebElement waitEle(String locator,Closure func,int timeoutMillisec) {
		return waitEleInternal(locator,true,null,null,func,timeoutMillisec)
	}
	private def WebElement waitEleInternal(String locator,Boolean waitValue,String attrValue, String attrName,Closure func,int timeoutMillisec) {
		WebElement ele=null
		if(timeoutMillisec==null) timeoutMillisec=GlobalVariable.WaitEleTimeout
		if(attrValue==null) attrValue=''
		if(attrName==null||attrName.empty) attrName='value'
		while(timeoutMillisec>0){
			try {
				ele=findElement(locator)
			}catch(Exception e) {}
			if (ele!=null) {
				if(waitValue==null || !waitValue) break;
				if(func!=null && func()) break;
				if(attrName!=null && ele.getAttribute(attrName).equalsIgnoreCase(attrValue)) break;
			}
			timeoutMillisec=timeoutMillisec-100;
			Thread.sleep(100)
		}
		return ele
	}



	//-------------register to verify-------------------
	private List<VerifyRegItem> verifyRegItems=new ArrayList<VerifyRegItem>()

	private VerifyRegItem registerItemToVerify(String locator, Object value,Object verifyValue,String verifyLocator,Closure verifyActualGetter) {
		VerifyRegItem item;
		Optional<VerifyRegItem> existing=verifyRegItems.stream().filter({o->o.getLocator().equalsIgnoreCase(locator)}).findFirst()
		if(existing.present) {
			item=existing.value
		}else {
			item=new VerifyRegItem()
		}
		item.setOperationType("type")
		item.setLocator(locator)
		item.setValue(value)
		item.setVerifyValue(verifyValue)
		item.setVerifyLocator(verifyLocator)
		item.setVerifyValueGetter(verifyActualGetter)
		item.setVerify(true)
		verifyRegItems.add(item);
		return item
	}
	private void registerItemNotVerify(String locator, Object value) {
		VerifyRegItem item=new VerifyRegItem()
		item.setLocator(locator)
		item.setValue(value)
		item.setVerify(false)
		verifyRegItems.add(item);
	}
	private void registerItemToVerifyChecked(String locator, Object value,Object verifyValue,String verifyLocator,Closure verifyActualGetter) {
		VerifyRegItem item=registerItemToVerify(locator,value,verifyValue,verifyLocator,verifyActualGetter);
		item.setOperationType("click2check")
	}
	def void verifyAll() {
		String msg1=""
		String msg0=""
		int cnt1=0
		int cnt0=0
		for(VerifyRegItem item : verifyRegItems) {
			if(item.getVerify()) {
				String verifyLocator=item.getVerifyLocator()
				if(verifyLocator==null) verifyLocator=item.getLocator()
				if(item.getVerifyFunc()!=null) {
					String message=item.getVerifyFunc()()
					assertTrue('检验未通过：'+item.getOperationType()+" ("+message+")"+verifyLocator,message==null)
				}else {
					String verifyValue=item.getVerifyValue()
					if(verifyValue==null) verifyValue=item.getValue()
					Object actualValue=null;
					if(item.getVerifyValueGetter()==null) {
						actualValue=getValue(verifyLocator)
					}else {
						actualValue=item.getVerifyValueGetter()(verifyLocator,this)
					}
					assertEquals('检验元素值未通过：'+verifyLocator,verifyValue,actualValue)
				}
				cnt1++
				msg1=msg1+verifyLocator+"\r\n"
			}else {
				cnt0++
				msg0=msg0+item.getLocator()+"\r\n"
			}
		}
		msg1="检验输入值通过：共 "+cnt1+" 项"+"\r\n"+msg1+"\r\n"
		msg0="未检验项：共 "+cnt0+" 项"+"\r\n"+msg0
		verifyRegItems.clear()
		KeywordUtil.markPassed(msg1+msg0)
	}

	//--------------包装过时接口并增强------------
	def void type(String locator,String value) {
		type(locator, value,null)
	}
	def void type(String locator,String value,String verifyValue) {
		type(locator,value,verifyValue,null)
	}
	def void type(String locator,String value,String verifyValue,String verifyLocator) {
		type(locator,value,verifyValue,verifyLocator,null)
	}
	def void type(String locator,String value,String verifyValue,String verifyLocator,Closure verifyActualGetter) {
		wait2type(locator, value)
		registerItemToVerify(locator, value,verifyValue,verifyLocator,verifyActualGetter)
	}
	def void type(String locator,String value,boolean registerVerify) {
		wait2type(locator, value)
		if(registerVerify) {
			registerItemToVerify(locator, value,null,null,null)
		}else {
			registerItemNotVerify(locator, value)
		}
	}
	def void click(String locator) {
		click(locator, null,null, null,null)
	}
	def void click(String locator,Object value,Object verifyValue,String verifyLocator,Closure verifyActualGetter) {
		WebElement ele=waitEle(locator)
		regClickInput(ele,locator,value,verifyValue,verifyLocator,verifyActualGetter);
		ele.click();
	}
	def void click(String locator,boolean canbeAnyButtonInTable) {
		click(locator,canbeAnyButtonInTable,null,true)
	}
	def void click(String locator,boolean canbeAnyButtonInTable,String buttonText) {
		click(locator,canbeAnyButtonInTable,buttonText,true)
	}
	def void click(String locator,boolean canbeAnyButtonInTable,String buttonText,boolean verifyRemoveRow) {
		WebElement btn
		if(canbeAnyButtonInTable) {
			btn=EleUtil.findButtonInTableToClick(locator,buttonText)
		}else {
			btn=waitEle(locator)
		}
		if(verifyRemoveRow) {
			WebElement tr=btn.findElement(By.xpath('./ancestor::tr'))
			String orgText=tr.getText();
			VerifyRegItem item=registerItemToVerify('表格中的按钮：['+btn.getText()+']', null,null,null,null)
			item.setOperationType('buttonInTableToRemoveRow')
			item.setVerifyFunc({
				->
				WebElement actualBtn =EleUtil.findButtonInTableToClick(locator,buttonText)
				if(actualBtn==null) return null
				WebElement actualTr=actualBtn.findElement(By.xpath('./ancestor::tr'))
				if(actualTr==null || !orgText.equals(actualTr.getText())) return null
				return '未能删除行'
			})
		}
		btn.click()
	}
	//自动识别是否点击了输入控件(包括kendo等包装出来的输入控件),相应注册检验
	private def void regClickInput(WebElement clickedEle,String locator,Object value,Object verifyValue,String verifyLocator,Closure verifyActualGetter) {
		System.console().println(clickedEle.getTagName());
		String tagName=clickedEle.getTagName()
		if(tagName.equalsIgnoreCase('input')) {
			String type=clickedEle.getAttribute('type');
			if(type.equalsIgnoreCase('radio')||type.equalsIgnoreCase('checkbox')) {
				if(value==null) value='true'
				if(verifyActualGetter==null) {
					if(verifyLocator==null) {
						verifyActualGetter= {l,sl->clickedEle.getAttribute('checked')}
					}else {
						verifyActualGetter= {l,sl->this.waitEle(verifyLocator).getAttribute('checked')}
					}
				}else {
					verifyValue=value;
				}
				registerItemToVerifyChecked(locator, value,verifyValue,verifyLocator,verifyActualGetter)
			}
		}else if(tagName.equalsIgnoreCase('span')) {
			WebElement kselectWrap=null
			WebElement kselectShowSpan=null
			try {
				kselectWrap=EleUtil.findKSelectWrap(clickedEle)
				kselectShowSpan=kselectWrap.findElement(By.xpath("./span/span"))
			}catch(Exception e) {
				return
			}
			String rootLocator="kSelect Name="+EleUtil.findKSelect(kselectWrap).getAttribute('name')
			verifyActualGetter= {l,sl -> kselectShowSpan.getText()}
			VerifyRegItem item=registerItemToVerify(rootLocator, value,verifyValue,null,verifyActualGetter)
			item.setOperationType('kselect')
		}else if(tagName.equalsIgnoreCase('li') && clickedEle.getAttribute('class').contains('k-item') && clickedEle.getAttribute('role').equalsIgnoreCase('option')) {
			VerifyRegItem lastItem=verifyRegItems.get(verifyRegItems.size()-1)
			if(lastItem.getOperationType().equalsIgnoreCase('kselect') && lastItem.getValue()==null) {
				lastItem.setValue(clickedEle.getText())
			}
		}
	}
	def void doubleClick(String locator) {
		wait2doubleClick(locator)
	}
	def String getValue(String locator) {
		return wait2getValue(locator)
	}
	def String getText(String locator) {
		return wait2getText(locator)
	}
	def void open(String url) {
		//super.open(locator);好像不等待
		WebUI.navigateToUrl(url);
	}
	def void sendKeys(String locator,CharSequence... arg0) {
		waitEle(locator).sendKeys(arg0)
	}
	//-------------------------------------

	private def WebElement findElement(String locator) {
		int pos=locator.indexOf("=");
		String type=locator.substring(0, pos)
		String expr=locator.substring(pos+1)
		By by=null;
		switch (type) {
			case "link":
				by=By.linkText(expr)
				break
			case "name":
				by=By.name(expr)
				break
			case "xpath":
				by=By.xpath(expr)
				break
			case "id":
				by=By.id(expr)
				break
		}
		WebElement element = this.wrappedDriver.findElement(by)
		return element
	}


	/*	@Keyword
	 def void verifyNoJsError() {
	 LogEntries logEntries=this.wrappedDriver.manage().logs().get(LogType.BROWSER)
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
	 */

	//--------------wait2------------------------------

	private def WebElement wait2click(String locator) {
		WebElement ele=waitEle(locator);
		ele.click()
		return ele
	}

	private def WebElement wait2type(String locator,String value) {
		WebElement ele=waitEle(locator);
		super.type(locator, value)
		return ele
	}

	private def WebElement wait2doubleClick(String locator) {
		WebElement ele=waitEle(locator);
		super.doubleClick(locator);
		return ele
	}

	private def String wait2getValue(String locator) {
		waitEle(locator);
		return super.getValue(locator);
	}
	private def String wait2getText(String locator) {
		waitEle(locator);
		return super.getText(locator);
	}
}