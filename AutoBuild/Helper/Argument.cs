using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuild.Helper
{
    public static class Argument
    {
        public static Dictionary<string,string> Args { get; set; }
        public static void Initialize(string[] args)
        {
            Args = new Dictionary<string, string>();
            if (args.Length == 0)
                return;
            if (args.Length % 2 != 0)
                return;
            
            for(int i=0;i<args.Length/2;i++)
            {
                Args.Add(args[i], args[++i]);
            }
        }
    }
}
