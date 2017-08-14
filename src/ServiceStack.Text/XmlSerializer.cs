#if !LITE
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace ServiceStack.Text
{
    /// <summary>
    /// Serializes and deserializes objects into and from XML strings. ServiceStack.Text.XmlSerializer enables you to control 
    /// how objects are encoded into XML.
    /// </summary>
    public class XmlSerializer
    {
        /// <summary>
        ///  Gets an default System.Xml.XmlWriterSettings instance providing a set of features to support on the System.Xml.XmlWriter object 
        /// created by the Overload:System.Xml.XmlWriter.Create method.
        /// </summary>
        public static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { Encoding = PclExport.Instance.GetUseEncoding(false) };

        /// <summary>
        ///  Gets an System.Xml.XmlReaderSettings instance providing a set of features to support on the System.Xml.XmlReader object 
        /// created by the Overload:System.Xml.XmlReader.Create method.
        /// </summary>
        public static readonly XmlReaderSettings ReaderSettings = new XmlReaderSettings { MaxCharactersInDocument = 1 << 20 };

        /// <summary>
        /// Deserializes the XML string with an System.IO.TextReader into an instance of object.
        /// </summary>
        /// <param name="reader">The System.IO.TextReader used to read the XML string.</param>
        /// <param name="type">The supplied data contract type of the object that are deserialized.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlReader, if the value is null use the default 
        /// <see cref="XmlSerializer.ReaderSettings"/>.</param>
        /// <returns>The deserialized object.</returns>
        public static object Deserialize(TextReader reader, Type type, XmlReaderSettings settings = null)
        {
            using (var xmlReader = XmlReader.Create(reader, settings ?? ReaderSettings))
            {
                var serializer = new DataContractSerializer(type);
                return serializer.ReadObject(xmlReader);
            }
        }

        /// <summary>
        /// Deserializes the XML string with an System.IO.TextReader into an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The supplied data contract type of the instance that are deserialized.</typeparam>
        /// <param name="reader">The System.IO.TextReader used to read the XML string.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlReader, if the value is null use the default 
        /// <see cref="XmlSerializer.ReaderSettings"/>.</param>
        /// <returns>The deserialized instance of the specified type.</returns>
        public static T Deserialize<T>(TextReader reader, XmlReaderSettings settings = null)
        {
            var type = typeof(T);
            return (T)Deserialize(reader, type, settings);
        }

        /// <summary>
        /// Deserializes the XML string into an instance of object.
        /// </summary>
        /// <param name="xmlString">The XML string to deserialize.</param>
        /// <param name="type">The supplied data contract type of the object that are deserialized.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlReader, if the value is null use the default 
        /// <see cref="XmlSerializer.ReaderSettings"/>.</param>
        /// <returns>The deserialized object.</returns>
        public static object Deserialize(string xmlString, Type type, XmlReaderSettings settings = null)
        {
            return Deserialize(new StringReader(xmlString), type, settings);
        }

        /// <summary>
        /// Deserializes the XML string into an instance of the specified type. 
        /// </summary>
        /// <typeparam name="T">The supplied data contract type of the instance that are deserialized.</typeparam>
        /// <param name="xmlString">The XML string to deserialize.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlReader, if the value is null use the default 
        /// <see cref="XmlSerializer.ReaderSettings"/>.</param>
        /// <returns>The deserialized instance of the specified type.</returns>
        public static T Deserialize<T>(string xmlString, XmlReaderSettings settings = null)
        {
            var type = typeof(T);
            return (T)Deserialize(xmlString, type, settings);
        }

        /// <summary>
        /// Deserializes the XML stream with an System.IO.Stream into an instance of object.
        /// </summary>
        /// <param name="stream">The System.IO.Stream used to read the XML stream.</param>
        /// <param name="type">The supplied data contract type of the object that are deserialized.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlReader, if the value is null use the default 
        /// <see cref="XmlSerializer.ReaderSettings"/>.</param>
        /// <returns>The deserialized object.</returns>
        public static object Deserialize(Stream stream, Type type, XmlReaderSettings settings = null)
        {
            using (var xmlReader = XmlReader.Create(stream, settings ?? ReaderSettings))
            {
                var serializer = new DataContractSerializer(type);
                return serializer.ReadObject(xmlReader);
            }
        }

        /// <summary>
        /// Deserializes the XML stream with an System.IO.Stream into an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The supplied data contract type of the instance that are deserialized.</typeparam>
        /// <param name="stream">The System.IO.Stream used to read the XML stream.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlReader, if the value is null use the default 
        /// <see cref="XmlSerializer.ReaderSettings"/>.</param>
        /// <returns>The deserialized instance of the specified type.</returns>
        public static T Deserialize<T>(Stream stream, XmlReaderSettings settings = null)
        {
            var type = typeof(T);
            return (T) Deserialize(stream, type, settings);
        }

        /// <summary>
        /// Serializes the specified System.Object and writes the XML string to a instance of System.IO.TextWriter.
        /// </summary>
        /// <param name="obj">The supplied data contract object that contains the data to write to the stream.</param>
        /// <param name="writer">The System.IO.Writer used to write the XML string.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlWriter, if the value is null use the default 
        /// <see cref="XmlSerializer.WriterSettings"/>.</param>
        public static void Serialize(object obj, TextWriter writer, XmlWriterSettings settings = null)
        {
            using (var xw = XmlWriter.Create(writer, settings ?? WriterSettings))
            {
                var serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(xw, obj);
            }
        }

        /// <summary>
        /// Serializes the specified System.Object and writes the XML string to a instance of System.IO.Stream.
        /// </summary>
        /// <param name="obj">The supplied data contract object that contains the data to write to the stream.</param>
        /// <param name="stream">The System.IO.Stream used to write the XML string.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlWriter, if the value is null use the default 
        /// <see cref="XmlSerializer.WriterSettings"/>.</param>
        public static void Serialize(object obj, Stream stream, XmlWriterSettings settings = null)
        {
            using (var xmlWriter = XmlWriter.Create(stream, settings ?? WriterSettings))
            {
                var serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(xmlWriter, obj);
            }
        }

        /// <summary>
        /// Serializes the specified System.Object into the XML string.
        /// </summary>
        /// <param name="obj">The supplied data contract object that are serialized.</param>
        /// <param name="settings">The settings for the new System.Xml.XmlWriter, if the value is null use the default 
        /// <see cref="XmlSerializer.WriterSettings"/>.</param>
        /// <returns>The XML string of the serialized object.</returns>
        public static string Serialize(object obj, XmlWriterSettings settings = null)
        {
            using (var ms = MemoryStreamFactory.GetStream())
            {
                Serialize(obj, ms, settings);            
                ms.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(ms);
                return reader.ReadToEnd();               
            }
        }
    }
}
#endif