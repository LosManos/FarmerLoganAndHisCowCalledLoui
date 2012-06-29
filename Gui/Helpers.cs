using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FarmerLoganAndHisCowCalledLoui.Gui
{
    /// <summary>
    /// http://www.c-sharpcorner.com/UploadFile/chauhan_sonu57/SerializingObjects07202006065806AM/SerializingObjects.aspx
    /// </summary>
    class Helpers
    {
        ////This will returns the set of included namespaces for the serializer.
        //public static XmlSerializerNamespaces GetNamespaces()
        //{

        //    XmlSerializerNamespaces ns;
        //    ns = new XmlSerializerNamespaces();
        //    ns.Add("xs", "http://www.w3.org/2001/XMLSchema");
        //    ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        //    return ns;

        //}

        ////Returns the target namespace for the serializer.
        //public static string TargetNamespace
        //{

        //    get
        //    {

        //        return "http://www.w3.org/2001/XMLSchema";
        //    }

        //}

        //Creates an object from an XML string.
        public static object FromXml(string Xml, System.Type ObjType)
        {

            XmlSerializer ser;
            ser = new XmlSerializer(ObjType);
            StringReader stringReader;
            stringReader = new StringReader(Xml);
            XmlTextReader xmlReader;
            xmlReader = new XmlTextReader(stringReader);
            object obj;
            obj = ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return obj;

        }

        //Serializes the <i>Obj</i> to an XML string.
        public static string ToXml(object Obj, System.Type ObjType)
        {

            //XmlSerializer ser;
            //ser = new XmlSerializer(ObjType, TargetNamespace);
            //MemoryStream memStream;
            //memStream = new MemoryStream();
            //XmlTextWriter xmlWriter;
            //xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
            //xmlWriter.Namespaces = true;
            //ser.Serialize(xmlWriter, Obj, GetNamespaces());
            //xmlWriter.Close();
            //memStream.Close();
            //string xml;
            //xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            //xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            //xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            //return xml;

            XmlSerializer ser;
            ser = new XmlSerializer(ObjType);
            MemoryStream memStream;
            memStream = new MemoryStream();
            XmlTextWriter xmlWriter;
            xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
            xmlWriter.Namespaces = true;
            ser.Serialize(xmlWriter, Obj);
            xmlWriter.Close();
            memStream.Close();
            string xml;
            xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            return xml;

        }
    }
}
