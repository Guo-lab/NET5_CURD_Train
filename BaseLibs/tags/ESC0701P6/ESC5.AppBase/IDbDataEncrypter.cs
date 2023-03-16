namespace ESC5.AppBase
{
    /// <summary>
    /// @author rainy
    /// </summary>
    public interface IDbDataEncrypter
    {

        /**
		 * 加密
		 * @param src 明文
		 * @return 密文
		 */
        string ToEncrypted(string src);
        /**
		 * 解密
		 * @param src 密文
		 * @return 明文
		 */
        string ToDecrypted(string src);
        /**
		 *  根据kek密钥解密后设置原始密钥
		 * @param kekPartInput1 kek密要输入部分1
		 * @param kekPartInput2 kek密要输入部分2
		 */
        void DecryptDbAesKey(string kekPartInput1, string kekPartInput2);
    }
}