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
	def static void contains(File file,String s) {
		contains(file,s,-1);
	}
	@Keyword
	def static void contains(File file,String expected,int expectedLineCount) {
		List<String> linesToVerify=linesToVerify(file);
		if(expectedLineCount==-1) {
			assertTrue("文件中应包含字符串：:expected="+expected,linesToVerify.any { it.contains(expected) });
			return;
		}
		int actualLineCount=0;
		linesToVerify.forEach({String line->
			if(line.contains(expected)) {
				actualLineCount++;
			}
		});
		assertEquals("文件中应包含字符串个数",expectedLineCount,actualLineCount);
	}
	@Keyword
	static String loadDefault() {
		FindDefault();
		linesOfDefaultFile=defaultFile.readLines("utf-8");
	}
	private static File FindDefault() {
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
	@Keyword
	static String clearDefault() {
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
	def static File exists(String path,String nameSubstr,String lineContentSubstr) {
		File found=exists(path,nameSubstr);
		List<String> lines=found.readLines("utf-8");
		println lineContentSubstr;
		println lines;
		String line=lines.find {it.contains(lineContentSubstr)}
		assertTrue("期待的文件内容不存在",line!=null);
		return found;
	}
	def private static String getAbsolutePath(String path) {
		if(path.contains(":")) {
			return path;
		}else {
			return GlobalVariable.ApplicationRunningPath+"/"+path;
		}
	}
}
