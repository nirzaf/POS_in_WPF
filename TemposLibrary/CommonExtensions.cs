using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Remoting.Messaging;

namespace TemposLibrary
{
    public static class CommonExtensions
    {
        #region Licensed Access Only
        static CommonExtensions()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(CommonExtensions).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use TemposLibrary.dll");
            }
#endif
        }
        #endregion

        public static byte[] XmlSerializeObject(this object serializableObject)
        {
            if (serializableObject != null)
            {

                XmlSerializer formatter = new XmlSerializer(serializableObject.GetType());
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, serializableObject);
                byte[] bytes = stream.GetBuffer();
                stream.Close();
                return bytes;
            }
            return null;
        }

        public static object XmlDeserializeObject(this byte[] serialBytes, Type type)
        {
            if ((serialBytes == null) || (serialBytes.Length == 0))
                return null;
            try
            {
                MemoryStream stream = new MemoryStream(serialBytes);
                stream.Seek(0, SeekOrigin.Begin);
                XmlSerializer formatter = new XmlSerializer(type);
                object result = formatter.Deserialize(stream);
                stream.Close();
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] SerializeObject(this object serializableObject)
        {
            if (serializableObject != null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, serializableObject);
                byte[] bytes = stream.GetBuffer();
                stream.Close();
                return bytes;
            }
            return null;
        }

        public static byte[] SerializeObject(this ISerializable serializableObject)
        {
            if (serializableObject != null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, serializableObject);
                byte[] bytes = stream.GetBuffer();
                stream.Close();
                return bytes;
            }
            return null;
        }

        public static object DeserializeObject(this byte[] serialBytes)
        {
            if ((serialBytes == null) || (serialBytes.Length == 0))
                return null;
            try
            {
                MemoryStream stream = new MemoryStream(serialBytes);
                stream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter formatter = new BinaryFormatter();
                object result = formatter.UnsafeDeserialize(stream, FormatterHeaderHandler); //formatter.Deserialize(stream);
                stream.Close();
                return result;
            }
            catch
            {
                return null;
            }
        }

        private static Object FormatterHeaderHandler(Header[] headers)
        {
            return null;
        }
    }
}
