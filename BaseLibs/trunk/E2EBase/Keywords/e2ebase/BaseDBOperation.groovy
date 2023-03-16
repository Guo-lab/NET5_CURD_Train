package e2ebase

public class BaseDBOperation {

	def trySetupDB(){
		trySetupDB(false);
	}
	def trySetupDB(boolean tryReset){
		if(tryReset==null) {
			tryReset=false;
		}
		if(alreadySetup()) {
			if(!tryReset) return;
			resetDB();
			return
		}
		setupDB();
	}
	def setupDB(){
	};

	def cleanupDB() {
	};

	def boolean alreadySetup() {
		return false;
	};

	def resetDB(){
		cleanupDB();
		setupDB();
	}
}

