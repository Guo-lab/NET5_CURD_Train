using System;
using ProjectBase.Utils;

namespace TestingBase.TestDouble.ProjectBase
{
    public class UtilFake : Util
    {
        public UtilFake()
        {

        }
        public override string LogRoot
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public override string FuncTree
        {
            get
            {
                return "";
            }
        }

        public override void AddLog(string psSource, Exception e)
        {
            var s = e.StackTrace;
        }
        public override void AddLog(string msg)
        {

        }
    }
}

