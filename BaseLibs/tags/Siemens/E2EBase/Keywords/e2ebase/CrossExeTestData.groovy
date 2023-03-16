package e2ebase

import java.nio.charset.Charset

import org.apache.commons.lang3.CharSetUtils

import groovy.json.*;
import internal.GlobalVariable as GlobalVariable

/**
 * 跨TC执行的全局变量。由于internal.GlobalVariable的作用域是TestSuite，当不在TestSuite中执行时无法达到在多次TC执行之间共享变量值（这种成为跨TC执行的全局变量）。因此需要使用此类。
 * 首先为这种变量定义Plain形式的类（只有示例变量），然后添加一个global()方法,例如：<br>
 *  public class LoginInfo {<br>
 *   * 	static LoginInfo global() {<br>
 * 		return CrossExeTestData.getGlobal(LoginInfo.class);<br>
 * 	}<br>
 * 
 * 	   Integer UserId;<br>
 *     String UserName;<br>
 * 
 * }<br>
 * 注意：需配置GlobalVariable.TestDataDir，值为绝对路径
 * @author Rainy
 *
 */
public class CrossExeTestData {

	private static Map<String,Object> rawMap=new HashMap<String,Object>();
	static Map<String,Object> map=new HashMap<String,Object>();

	private static JsonSlurper jsonSlurper=new JsonSlurper();

	static Object getGlobal(Class clazz) {
		String key=clazz.simpleName;
		if(rawMap.size()==0) {
			File file=new File(filename());
			if(file.exists()) {
				rawMap=jsonSlurper.parse(file, "utf-8");//此处rawMap中的value实际是String类型，如果直接返回就会传值不传址，即rawMap.get(key)每次都返回不同的对象。所以后面要转成存对象指针再存一个map
				for(String k : rawMap.keySet()) {
					map.put(k,null);
				}
			}
		}
		Object obj;
		if(!map.containsKey(key) || map.get(key)==null) {
			if(!rawMap.containsKey(key)){
				obj=clazz.newInstance();
			}else {
				obj=rawMap.get(key).asType(clazz);//此处把无指针的String对象转成有指针的对象。这样map.get(key)每次就会返回同一对象
			}
			map.put(key,obj);
		}

		return map.get(key);
	}
	static void save() {
		if(rawMap.size()==0) return;
		for(String k : map.keySet()) {
			if(map.get(k)==null) {
				map.put(k,rawMap.get(k));
			};
		}
		String s=JsonOutput.toJson(map);
		File file=new File(filename());
		if(!file.exists()) {
			file.createNewFile();
		}
		file.write(s,"utf-8");
	}
	static void clear() {
		map=new HashMap<String,Object>();
	}
	static String filename() {
		return GlobalVariable.TestDataDir+"\\.cache\\CrossExeTestData.json";
	}
}
