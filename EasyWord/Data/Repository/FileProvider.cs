using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyWord.Data.Repository
{
    public class FileProvider
    {
        /// <summary>
        /// AppData
        /// </summary>
        private static string _appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// AppData + AppName
        /// </summary>
        private static string _basePath = Path.Combine(_appPath, "EasyWord");

        /// <summary>
        /// Serialize Class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config">instance</param>
        /// <param name="filePath">relative path</param>
        public static void SaveConfig<T>(T config, string filePath, bool package)
        {
            string absPath = filePath;
            if (package)
            {
                if (!Directory.Exists(_basePath))
                {
                    Directory.CreateDirectory(_basePath);
                }
                absPath = Path.Combine(_basePath, filePath);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(absPath))
            {
                serializer.Serialize(writer, config);
            }
        }

        public static void SaveConfig<T>(T config, string filePath)
        {
            SaveConfig(config, filePath, false);
        }

        /// <summary>
        /// Load class from serialized XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">Absolute Path</param>
        /// <param name="package">Makes filePath relative to the AppData Folder for the Application</param>
        /// <returns>instance</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static T LoadConfig<T>(string filePath, bool package)
        {
            string absPath = filePath;
            if (package)
            {
                absPath = Path.Combine(_basePath, filePath);
            }

            if (File.Exists(absPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                using (TextReader reader = new StreamReader(absPath))
                {
                    return (T)deserializer.Deserialize(reader);
                }
            }
            else
            {
                throw new FileNotFoundException("Config file not found");
            }
        }

        /// <summary>
        /// Overfloww defaulting to External loading
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadConfig<T>(string path) {
            return LoadConfig<T>(path, false);
        }
    }
}
