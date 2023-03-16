using ProjectBase.Utils;
using SharpArch.Domain;

namespace ESC5.AppBase
{

    /**
	  *   加密存放的数据的原始密钥本身也要加密，密文存在网站源码里，原始密钥明文和KEK密码明文分别分成至少两部分，
	  *   一部分写在加密工具和网站源码里，另一部分由一名或多名管理用户输入。这样实现了异地分散存放，同时原始密钥保持不变，KEK密码可以经常改变，
	  *   每次改变后重新加密原始密钥并将密文和KEK密码部分明文更新到源码中。
	  *   应用启动后，由管理员登录后台输入KEK，系统用KEK解密得到原始密钥。
	 * @author Rainy
	 *
	 */

    public class DbDataEncrypter : IDbDataEncrypter {

		public IUtil Util { get; set; }

		private static string dbAesKeyEncrypted = "xxxxxxxxxxxxxxxxxxxx=";

		private static string dbAesKey="aaaaaaaa";//暂用明文
		private static string kekPrefix = "Free";

		public string ToEncrypted(string src) {
			Check.Require(dbAesKey != null);
			return Util.DesEncrypt(src, dbAesKey);//TODO改算法
		}
		public string ToDecrypted(string src) {
			Check.Require(dbAesKey != null);
			return Util.DesDecrypt(src, dbAesKey);//TODO改算法
		}
		public void DecryptDbAesKey(string kekPartInput1, string kekPartInput2) {
			dbAesKey = Util.DesDecrypt(dbAesKeyEncrypted, kekPrefix + kekPartInput1 + kekPartInput2);
		}

	}
}