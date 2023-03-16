using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Utils
{
    public static class DictionaryExtension
    {
        public static IDictionary<string, T> AddRange<T>(this IDictionary<string, T> org, IDictionary<string, T> tobeAdded)
        {
            foreach (var k in tobeAdded.Keys)
            {
                org.Add(k, tobeAdded[k]);
            }
            return org;
        }
    }
}

