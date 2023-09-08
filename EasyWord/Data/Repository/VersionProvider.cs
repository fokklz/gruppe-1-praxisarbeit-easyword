using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyWord.Data.Repository
{
    public class VersionProvider
    {

        private static string _resource = "EasyWord.VERSION";

        public static string getVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(_resource);
            if (stream == null)
            {
                throw new InvalidOperationException("Version resource not found");
            }
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

    }
}
