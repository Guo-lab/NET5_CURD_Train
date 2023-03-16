package e2ebase

import com.kms.katalon.core.annotation.Keyword
import static org.junit.Assert.*
import internal.GlobalVariable

public class AssertUtil {

	private static final String errmsg = "实际与预期不符: ";
	private static final String dateStringPrefix ="Date(";

	@Keyword
	public static boolean assertMapDataEquals(Object actual, Object expected,String errmsgPrefix) {
		return assertMapDataEquals(actual,expected,errmsgPrefix,true);
	}
	/**
	 * 
	 * @param actual 简单类型或Map类型
	 * @param expected 简单类型或Map类型
	 * @param errmsgPrefix
	 * @return
	 */
	@Keyword
	public static boolean assertMapDataEquals(Object actual, Object expected,String errmsgPrefix,boolean stopOnFail) {
		String err=errmsgPrefix+errmsg;
		if (actual==null && expected!=null) {
			return fail(err+"expected="+expected.toString()+",actual=null",stopOnFail);
		}
		if (actual!=null && expected==null) {
			return fail(err+"expected=null,actual="+actual.toString(),stopOnFail);
		}
		if ((actual==null && expected==null) || actual.equals(expected) || expected.equals("@@any")) {
			return true;
		}
		if (actual instanceof String && expected instanceof String) {
			String actualS=(String)actual;
			String expectedS=(String)expected;
			if(actualS.startsWith(dateStringPrefix) || expectedS.startsWith(dateStringPrefix)) {
				//日期比较
				actualS=actualS.substring(dateStringPrefix.length());
				expectedS=expectedS.substring(dateStringPrefix.length());
				if(!actualS.startsWith(expectedS)) {
					return fail(err+"expected="+expected.toString()+",actual="+actual.toString(),stopOnFail);
				}
			}
			return true;
		}

		if(actual.getClass()==ArrayList.class && expected.getClass()!=ArrayList.class
		||actual.getClass()!=ArrayList.class && expected.getClass()==ArrayList.class) {
			return fail(err+"预期与实际有一方是数组而另一方不是",true);
		}
		if(actual.getClass()==ArrayList.class && expected.getClass()==ArrayList.class) {
			ArrayList actualList=(ArrayList)actual;
			ArrayList expectedList=(ArrayList)expected;
			if(actualList.size()<expectedList.size()) {
				return fail(err+"数组大小"+"expected="+expectedList.size()+",actual="+actualList.size(),true);
			}
			for(int i=0;i<expectedList.size();i++) {
				Object notFound=expectedList[i];
				for(int j=0;i<actualList.size();j++) {
					if (assertMapDataEquals(actualList[j], expectedList[i],errmsgPrefix+"["+i.toString()+"]",false)) {
						notFound=null;
						break;
					}
				}
				if(notFound!=null) {
					return fail(err+"预期的数组元素未找到"+"expected="+notFound.toString()+",actual="+actualList.toString(),true);
				}
			}
			return true;
		}

		Map<String, Object> actualMap = null;
		Map<String, Object> expectedMap = null;
		try {
			actualMap = (Map<String, Object>) actual;
			expectedMap = (Map<String, Object>) expected;
		} catch (Exception e) {
			return fail(err+"expected="+expected.toString()+",actual="+actual.toString(),stopOnFail);
		}
		for (Map.Entry<String, Object> entry : expectedMap.entrySet()) {
			String key = entry.getKey();
			if (!actualMap.containsKey(key)) {
				return fail(err+"预期属性不在实际结果中: "+entry.getKey(),stopOnFail);
			}
			Object actualValue = actualMap.get(entry.getKey());
			Object expectedValue = entry.getValue();
			if (!assertMapDataEquals(actualValue, expectedValue,errmsgPrefix+"."+key)) {
				return fail(err+"property="+key
						+",expected="+expectedValue+",actual="+actualValue,stopOnFail);
			}
		}
		return true;
	}

	private static boolean fail(String msg,boolean stop) {
		if(stop) {
			fail(msg);
		}
		return false;
	}
}
