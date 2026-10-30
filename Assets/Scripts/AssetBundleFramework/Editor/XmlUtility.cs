using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AssetBundleFramework.Editor
{
    public static class XmlUtility
    {
        public static T Read<T>(string fileName) where T : class
        {
            FileStream stream = null;
            if (!File.Exists(fileName))
            {
                return default(T);
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T)); // 创建指定目标类型
                stream = File.OpenRead(fileName); // 以只读方式打开 
                XmlReader reader = XmlReader.Create(stream); // 读取数据
                T instance = (T)serializer.Deserialize(reader); // 序列化
                stream.Close(); // 关闭流
                return instance;
            }
            catch (Exception e)
            {
                if(stream != null) stream.Close();
                return default(T);
            }
        }
    }
}