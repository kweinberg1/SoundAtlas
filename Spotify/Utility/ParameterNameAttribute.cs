using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Utility
{
    public static class Utility
    {
        //Compiles a list of flag names.
        public static string GetParameterNameAttribute<T>(this T en, String separator) where T : struct, IConvertible
        {
            Enum e = (Enum)(object)en;

            IEnumerable<Enum> enumValues = Enum.GetValues(typeof(T)).Cast<Enum>().Where(value => e.HasFlag(value));

            List<String> enumValueList = new List<String>();
            foreach(Enum enumValue in enumValues)
            {
                String enumName = typeof(T).GetField(enumValue.ToString()).GetCustomAttributes(typeof(ParameterNameAttribute), false).Cast<ParameterNameAttribute>().FirstOrDefault<ParameterNameAttribute>().Name;
                enumValueList.Add(enumName);
            }

            return String.Join(separator, enumValueList.ToArray());
        }
    }

    public sealed class ParameterNameAttribute : Attribute
    {
        public String Name;

        public ParameterNameAttribute(String name)
        {
            Name = name;
        }
    }
}
