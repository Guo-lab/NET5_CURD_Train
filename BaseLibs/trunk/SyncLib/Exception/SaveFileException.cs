using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLib.Exception
{
    public class SaveFileException :System.Exception
    {

        public SaveFileException(string fileName):base("Save file failed:" + fileName)
        {

        }
    }
}
