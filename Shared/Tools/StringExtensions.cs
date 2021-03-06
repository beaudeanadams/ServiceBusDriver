using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace ServiceBusDriver.Shared.Tools
{
    public static class StringExtensions
    {

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text); // Just because I dont like to add the extra 'string.'
        }

        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text); // Just because I dont like to add the extra 'string.'
        }

        public static bool IsNotNullOrWhitespace(this string text)
        {
            return !string.IsNullOrWhiteSpace(text); // Just because I dont like to add the extra 'string.'
        }
        
        public static string ToSpaceSeperated(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                stringBuilder.Append(s[i]);

                var nextChar = i + 1;
                if (nextChar < s.Length && char.IsUpper(s[nextChar]) && !char.IsUpper(s[i]))
                {
                    stringBuilder.Append(' ');
                }
            }

            return stringBuilder.ToString();
        }

        public static int GetSizeInBytes(this string text)
        {
            return Encoding.ASCII.GetByteCount(text);
        }

        public static int GetSizeInKiloBytes(this string text)
        {
            return Encoding.ASCII.GetByteCount(text)/1000;
        }

        public static string GetFirstNCharacters(this string text, int length)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            else
            {
                if (text.Trim(' ').Length < length)
                {
                    return text;
                }
                else
                {
                    return text.Substring(0, length);
                }
            }
        }
        
        public static string FormalXml(this string text)
        { try
            {
                var doc = XDocument.Parse(text);
                return doc.ToString();
            }
            catch (Exception)
            {
                // Handle and throw if fatal exception here; don't just ignore them
                return text;
            }
        }

        public static bool IsJson(this string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return false;
            }

            try
            {
                var obj = JToken.Parse(item);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsXml(this string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return false;
                }

                using (var stringReader = new StringReader(text))
                {
                    using var xmlReader = XmlReader.Create(stringReader);
                    while (xmlReader.Read())
                    {
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Append the given query keys and values to the uri. Taken from QueryHelpers
        /// </summary>
        /// <param name="uri">The base uri.</param>
        /// <param name="queryString">A collection of name value query pairs to append.</param>
        /// <returns>The combined result.</returns>
        public static string AddQueryStringWithoutNullCheck(
            string uri,
            IEnumerable<KeyValuePair<string, string>> queryString)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            var anchorIndex = uri.IndexOf('#');
            var uriToBeAppended = uri;
            var anchorText = "";
            // If there is an anchor, then the query string must be inserted before its first occurence.
            if (anchorIndex != -1)
            {
                anchorText = uri[anchorIndex..];
                uriToBeAppended = uri[..anchorIndex];
            }

            var queryIndex = uriToBeAppended.IndexOf('?');
            var hasQuery = queryIndex != -1;

            var sb = new StringBuilder();
            sb.Append(uriToBeAppended);
            foreach (var parameter in queryString)
            {
                if (parameter.Value != null)
                {
                    sb.Append(hasQuery ? '&' : '?');
                    sb.Append(UrlEncoder.Default.Encode(parameter.Key));
                    sb.Append('=');
                    sb.Append(UrlEncoder.Default.Encode(parameter.Value));
                    hasQuery = true;
                }
            }

            sb.Append(anchorText);
            return sb.ToString();
        }
    }
}
