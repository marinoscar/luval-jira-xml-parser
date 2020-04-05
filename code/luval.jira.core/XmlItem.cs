using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class XmlItem
    {
        //public XmlItem() : this(@"<?xml version=""1.0"" encoding=""utf-8""?><empty></empty>")
        //{

        //}
        public XmlItem(string xml)
        {
            Element = XElement.Parse(xml);
        }

        public XmlItem(Stream xml)
        {
            Element = XElement.Load(xml);
        }

        public XmlItem(XElement element)
        {
            Element = element;
        }
        public XElement Element { get; protected set; }


        protected virtual T GetAttributeOrDefault<T>(XElement element, string attribute, T defaultValue)
        {
            if (element == null) return default(T);
            var att = element.Attributes().FirstOrDefault(i => i.Name == attribute);
            if (att == null) return defaultValue;
            return ChangeType<T>(att.Value);
        }

        protected virtual T GetAttributeOrDefault<T>(string attribute)
        {
            return GetAttributeOrDefault<T>(Element, attribute, default(T));
        }

        protected virtual T GetAttributeOrDefault<T>(XElement element, string attribute)
        {
            return GetAttributeOrDefault<T>(element, attribute, default(T));
        }

        protected virtual T GetAttributeOrDefault<T>(string attribute, T defaultValue)
        {
            return GetAttributeOrDefault<T>(Element, attribute, defaultValue);
        }

        protected virtual XElement GetElement(XElement element, string elementName)
        {
            return element.Elements().FirstOrDefault(i => i.Name == elementName);
        }

        protected virtual XElement GetElement(string elementName)
        {
            return Element.Elements().FirstOrDefault(i => i.Name == elementName);
        }

        protected virtual T GetElementValueOrDefault<T>(XElement element, string elementName, T defaultValue)
        {
            if (element == null) return defaultValue;
            var el = element.Elements().FirstOrDefault(i => i.Name == elementName);
            if (el == null) return defaultValue;
            return ChangeType<T>(el.Value);
        }

        protected virtual T GetElementValueOrDefault<T>(XElement element, T defaultValue)
        {
            if (element == null) return defaultValue;
            return ChangeType<T>(element.Value);
        }

        protected virtual T GetElementValueOrDefault<T>(XElement element)
        {
            if (element == null) return default(T);
            return ChangeType<T>(element.Value);
        }

        protected virtual T GetElementValueOrDefault<T>(XElement element, string elementName)
        {
            return GetElementValueOrDefault<T>(element, elementName, default(T));
        }

        protected virtual T GetElementValueOrDefault<T>(string elementName, T defaultValue)
        {
            return GetElementValueOrDefault<T>(Element, elementName, defaultValue);
        }

        protected virtual T GetElementValueOrDefault<T>(string elementName)
        {
            return GetElementValueOrDefault<T>(Element, elementName, default(T));
        }

        protected virtual T GetElementValueOrDefault<T>()
        {
            return ChangeType<T>(Element.Value);
        }

        protected virtual T ChangeType<T>(object val)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                if (val == null || DBNull.Value.Equals(val)) return default(T);
                if(val is string && string.IsNullOrWhiteSpace(Convert.ToString(val)))
                {
                    return default(T);
                }
                if(Nullable.GetUnderlyingType(typeof(T)) == typeof(DateTimeOffset)){
                    DateTimeOffset dto;
                    var res = DateTimeOffset.TryParse(Convert.ToString(val), out dto);
                    return res ? (T)Convert.ChangeType(dto, Nullable.GetUnderlyingType(typeof(T))) : default(T);
                }
                return (T)Convert.ChangeType(val, Nullable.GetUnderlyingType(typeof(T)));
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(HttpUtility.HtmlDecode(Convert.ToString(val)).Replace("_amp;amp;", "&"), typeof(T));
            }
            else
                return (T)Convert.ChangeType(val, typeof(T));
        }
    }
}
