using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace iTunesSyncer.Extensions
{
    public static class XMLExtension
    {
        public static XElement? GetElement(this XDocument document, XName name)
        {
            var ret = document.Element(name);

            if (ret is null)
            {
                throw new NotFoundElementException();
            }

            return ret;
        }

        public static XElement? GetElement(this XElement element, XName name)
        {
            var ret = element.Element(name);

            if (ret is null)
            {
                throw new NotFoundElementException();
            }

            return ret;
        }

        public static IEnumerable<XElement> GetElements(this XElement element)
        {
            var ret = element.Elements();

            if (ret is null)
            {
                throw new NotFoundElementException();
            }

            return ret;
        }

        public static IEnumerable<XElement> GetElements(this XElement element, XName name)
        {
            var ret = element.Elements(name);

            if (ret is null)
            {
                throw new NotFoundElementException();
            }

            return ret;
        }
    }

    public class NotFoundElementException : Exception { }
}
