using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ESC5.Email.Message
{
    public interface IEmailBodyGenerator
    {
        public string GenerateBody(string template, Dictionary<string, string> values);
        public string GenerateBody(string template, Dictionary<string, string> values, Dictionary<string, IList<Dictionary<string, string>>> loopValues);
    }
}
