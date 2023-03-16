using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLib.Exception
{
    public class FileMissingException :System.Exception
    {

        public FileMissingException(string fileName):base("File not found:" + fileName)
        {

        }
    }
}
