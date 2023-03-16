package e2ebase

import com.kms.katalon.core.annotation.Keyword

import static org.junit.Assert.*

import java.nio.file.Files
import java.nio.file.Paths

import internal.GlobalVariable

public class VerifyFile {

	private static File defaultFile;
	private static List<String> linesOfDefaultFile;
	//	private static String beginDT;
	//	private static String endDT;

	static String DateTimeFormat="yyyy/MM/dd HH:mm:ss";
	//	static Closure findDateTimeStringFunc = {String line->
	//		return line.substring(0, 18);
	//	};

	@Keyword
	def static void contains(String s) {
		contains(s,-1);
	}
	@Keyword
	def static void contains(String expected,int expectedLineCount) {
		contains(null,expected,expectedLineCount);
	}
	@Keyword
	def static void contains(String filePath,String s) {
		contains(filePath,s,-1);
	}
	@Keyword
	def static void contains(String filePath,String expected,int expectedLineCount) {
		containsInternal(true,filePath,expected,expectedLineCount);
	}
	@Keyword
	def static void notContains(String s) {
		notContains(s,-1);
	}
	@Keyword
	def static void notContains(String expected,int expectedLineCount) {
		notContains(null,expected,expectedLineCount);
	}
	@Keyword
	def static void notContains(String filePath,String s) {
		notContains(filePath,s,-1);
	}
	@Keyword
	def static void notContains(String filePath,String expected,int expectedLineCount) {
		containsInternal(false,filePath,expected,expectedLineCount);
	}
	private def static void containsInternal(boolean verifyTrue,String filePath,String expected,int expectedLineCount) {
		File file=null;
		if(filePath!=null) {
			file=findFile(filePath);
		}
		List<String> linesToVerify=linesToVerify(file);
		if(expectedLineCount==-1) {
			String msg="文件中应包含字符串：:expected="+expected;
			boolean condition=linesToVerify.any { it.contains(expected) };
			if(verifyTrue){
				assertTrue(msg,condition);
			}else {
				assertFalse(msg,condition);
			}
			return;
		}
		int actualLineCount=0;
		linesToVerify.forEach({String line->
			if(line.contains(expected)) {
				actualLineCount++;
			}
		});
		if(verifyTrue){
			assertEquals("文件中应包含字符串个数",expectedLineCount,actualLineCount);
		}else {
			assertNotEquals("文件中应包含字符串个数",expectedLineCount,actualLineCount);
		}
	}
	@Keyword
	static String loadDefault() {
		findDefault();
		linesOfDefaultFile=defaultFile.readLines("utf-8");
	}
	private static File findDefault() {
		File fileOrDir=new File(GlobalVariable.VerifyFileDir);
		if(fileOrDir.isDirectory()) {
			File[] files=fileOrDir.listFiles();
			defaultFile=files.max({File a,File b->
				if(a.lastModified()>=b.lastModified()) return 1;
				return -1;
			});
		}else {
			defaultFile=fileOrDir;
		}
	}
	private static File findFile(String dirOrFilePath) {
		File dirOrFile=new File(getAbsolutePath(dirOrFilePath));
		if(dirOrFile.isDirectory()) {
			File[] files=dirOrFile.listFiles();
			return files.max({File a,File b->
				if(a.lastModified()>=b.lastModified()) return 1;
				return -1;
			});
		}else {
			return dirOrFile;
		}
	}
	@Keyword
	static String clearDefault() {
		findDefault();
		if(defaultFile!=null) {
			defaultFile.write("","utf-8");
		}
	}
	@Keyword
	static String clearFile(File file) {
		if(file!=null) {
			file.write("","utf-8");
		}
	}
	@Keyword
	static String clearDir(String dirPath) {
		String absoluteDirPath=getAbsolutePath(dirPath);
		File dir=new File(absoluteDirPath);
		if(dir.exists() && dir.isDirectory()) {
			File[] files=dir.listFiles();
			files.each { File f->f.delete() }
		} else {
			assertTrue("未找到指定目录"+dirPath,false);
		}
	}
	//	@Keyword
	//	static String markBeginTime() {
	//		beginDT=Calendar.getInstance().format(DateTimeFormat);
	//		endDT=null;
	//	}
	//	@Keyword
	//	static String markEndTime() {
	//		endDT=Calendar.getInstance().format(DateTimeFormat);
	//	}
	private static List<String> linesToVerify(File file) {
		List<String> fileLines=linesOfDefaultFile;
		if(file!=null) {
			fileLines=file.readLines("utf-8");
		}
		return fileLines;
	}

	@Keyword
	def static File exists(String path) {
		String absolutePath=getAbsolutePath(path);
		assertTrue("期待的文件不存在",Files.exists(absolutePath));
		return new File(absolutePath);
	}
	@Keyword
	def static File exists(String dirPath,String nameSubstr) {
		String absoluteDirPath=getAbsolutePath(dirPath);
		File dir=new File(absoluteDirPath);
		assertTrue("期待的目录不存在",dir.exists() && dir.isDirectory());
		File[] files=dir.listFiles();
		File found=files.find {it.name.contains(nameSubstr)};
		assertTrue("期待的文件不存在",found!=null);
		return found;
	}
	@Keyword
	def static File exists(String dirPath,String nameSubstr,String lineContentSubstr) {
		File found=exists(dirPath,nameSubstr);
		List<String> lines=found.readLines("utf-8");
		String line=lines.find {it.contains(lineContentSubstr)}
		assertTrue("期待的文件内容不存在",line!=null);
		return found;
	}
	@Keyword
	def static void notExists(String path) {
		String absolutePath=getAbsolutePath(path);
		assertFalse("文件已存在",Files.exists(absolutePath));
	}
	@Keyword
	def static void notExists(String dirPath,String nameSubstr) {
		String absoluteDirPath=getAbsolutePath(dirPath);
		File dir=new File(absoluteDirPath);
		if(dir.exists() && dir.isDirectory()) {
			File[] files=dir.listFiles();
			File found=files.find {it.name.contains(nameSubstr)};
			assertTrue("文件已存在",found==null);
		}
	}
	@Keyword
	def static void notExists(String dirPath,String nameSubstr,String lineContentSubstr) {
		String absoluteDirPath=getAbsolutePath(dirPath);
		File dir=new File(absoluteDirPath);
		if(dir.exists() && dir.isDirectory()) {
			File[] files=dir.listFiles();
			File found=files.find {it.name.contains(nameSubstr)};
			if(!found) return;
			List<String> lines=found.readLines("utf-8");
			String line=lines.find {it.contains(lineContentSubstr)}
			assertTrue("文件内容已存在",line==null);
		}
	}
	def private static String getAbsolutePath(String path) {
		if(path.contains(":")) {
			return path;
		}else {
			return GlobalVariable.ApplicationRunningPath+"/"+path;
		}
	}
}
