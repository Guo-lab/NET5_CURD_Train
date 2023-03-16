package e2ebase

public class VerifyRegItem {
	String locator//原元素的locator,实际检验时不用做locator,所以只要是能提示原元素的意义的字符串就行
	Object value
	Object verifyValue
	String verifyLocator
	Closure verifyValueGetter//{String locator,PBSelenium selenium}->Object
	Closure verifyFunc //{->String message返回空值表示检验通过}
	String operationType
	boolean verify
}
