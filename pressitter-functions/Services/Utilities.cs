using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Pressitter.Dtos;

namespace Pressitter.Services
{
    public static class Utilities
    {
        public static string RemoveInvalidChars(string text) 
        {
            string result = text;

            foreach (string str in new string[] {"\"", "'"})
            {
                result = result.Replace(str, "");
            }

            return result;            
        }
    }
}