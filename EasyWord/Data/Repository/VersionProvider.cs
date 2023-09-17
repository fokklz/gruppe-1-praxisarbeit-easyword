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

        private static Stream? _getStreamFromResource()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                return assembly.GetManifestResourceStream(_resource);
            }

            return null;
        }

        public static (string Version, DateTime LastModified) GetVersion()
        {
            using Stream? stream = _getStreamFromResource();
            if (stream == null)
            {
                throw new InvalidOperationException("Version resource not found");
            }

            using StreamReader reader = new StreamReader(stream);
            string? version = reader.ReadLine();
            string? lastModifiedStr = reader.ReadLine() ?? "2023-09-16 14:45:00";

            if (DateTime.TryParse(lastModifiedStr, out DateTime lastModified))
            {
                return (version, lastModified);
            }
            else
            {
                throw new InvalidOperationException("Could not parse last modified date");
            }
        }


    }
}
