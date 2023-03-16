using System;
using System.IO;
using System.Threading;

namespace ProjectBase.Utils
{

    public class FileGeneratorUtil
    {

        /////////////private MessageDigest mMessageDigest = null;

        /*****public FileGeneratorUtil() {
            try {
                mMessageDigest = MessageDigest.getInstance("MD5");
            } catch (Exception e) {
                Console.Write(e.Message);
            }
        }***********/

        public static void RunMetaToolForClient(string devtoolOutputRoot, string clientVmMetadataOutput, bool createVmJsonForTesting, bool authCheck)
        {
            var tr = new Thread(() =>
            {
                try
                {
                    if (clientVmMetadataOutput != "" || clientVmMetadataOutput != "no")
                    {
                        // 服务器端转换类型时错误信息和客户端类型验证
                        ////////AngularModelMetadataProvider.setCurrent(new AngularModelMetadataProvider());
                        ///////////AngularModelMetadataProvider.getCurrent().createMetaDatas();
                        ///////AngularClientValidationProvider.setCurrent(new AngularClientValidationProvider());
                        ////////ClientVmMetadataProvider.createAll(clientVmMetadataOutput);
                        ////////ClientVmDefinitionTool.createAllTypeDefinition(clientVmMetadataOutput);
                        ///////////if (createVmJsonForTesting) {
                        /////////////ClientVmDefinitionTool.createVMObjJsonFile(devtoolOutputRoot+"\\test");
                    }
                    ///////////////AuthCheckingTool.createAllConstDefinition(clientVmMetadataOutput);
                    //////////////AuthCheckingTool.createAllLangMessageKeys(devtoolOutputRoot);
                    /////////////AuthCheckingTool.generateAllControllerTest(devtoolOutputRoot+"\\test");
                    //必须在generateAllControllerTest之后
                    /////////////ClientVmDefinitionTool.createVMHtmlFileAndAddVmToTest(devtoolOutputRoot + "\\html",devtoolOutputRoot + "\\test");
                    //}
                    /*******
                    if (authCheck) {
                        AuthCheckingTool.check();
                    }
                    AuthCheckingTool.cleanup();
                    *********/
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            });
            tr.IsBackground = true;
            tr.Start();
        }

        /**
         * obtain the file's MD5
         */
        /*private string getMD5(byte[] src) {
            mMessageDigest.update(src);
            return new BigInteger(1, mMessageDigest.digest()).toString(16);
        }*/

        public bool IsContentSame(string sFileName, byte[] content, bool useMd5=false)
        {
            byte[] filebytes0;
            byte[] filebytes;
            if (File.Exists(sFileName))
            {
                filebytes0 = File.ReadAllBytes(sFileName);
                filebytes = filebytes0;
                //有时多出个文件尾?
                int extra = filebytes0.Length - content.Length;
                if (extra == 2 || extra == 1)
                {
                    Array.Copy(filebytes0, filebytes, filebytes0.Length - extra);
                }
            }
            else
            {
                return false;
            }

            if (filebytes.Length != content.Length)
                return false;

            if (useMd5)
            {
                /********string fileMD5 = getMD5(filebytes);
                string contentMD5 = getMD5(content);
                return fileMD5.equals(contentMD5);***********/
                throw new Exception("暂不支持使用MD5");
            }
            else
            {
                var same = true;
                for (var i = 0; i < filebytes.Length; i++)
                {
                    if (filebytes[i] != content[i])
                    {
                        same = false;
                        break;
                    }
                }
                return same;
            }
        }

    }
}
