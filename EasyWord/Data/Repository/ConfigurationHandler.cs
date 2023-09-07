using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyWord.Data.Repository
{
    public class ConfigurationHandler
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
        public static void SaveConfig<T>(T config, string filePath)
        {
            if (!File.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            string absPath = Path.Combine(_basePath, filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(absPath))
            {
                serializer.Serialize(writer, config);
            }
        }

        /// <summary>
        /// Load class from serialized XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">relative path</param>
        /// <returns>instance</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static T LoadConfig<T>(string filePath)
        {
            string absPath = Path.Combine(_basePath, filePath);
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
    }
}
