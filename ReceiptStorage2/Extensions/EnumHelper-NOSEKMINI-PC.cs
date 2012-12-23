using System;
using System.Collections.Generic;
using System.Linq;


namespace ReceiptStorage.Extensions
{
    public class Enum<T>
    {
        /// <summary>
        /// Gets the names of the Enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>An array of strings that contains the names of the enumerated values</returns>
        /// <remarks>
        /// From StackExchange: http://stackoverflow.com/questions/3935953/how-do-i-bind-an-enum-to-my-listbox
        /// </remarks>
        public static IEnumerable<string> GetNames()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            return (
              from field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
              where field.IsLiteral
              select field.Name).ToList<string>();
        }

        /// <summary>
        /// Generic Version of Parse.
        /// </summary>
        /// <typeparam name="T">The Enum to parse.</typeparam>
        /// <param name="name">The enumerated value name.</param>
        /// <returns>The Enumerated Value</returns>
        public static T Parse<T>(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");
            }

            return (T)Enum.Parse(typeof(T), name, true);
        }
    }

}
