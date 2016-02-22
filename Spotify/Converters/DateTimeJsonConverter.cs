using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

namespace Spotify.Converters
{
    class DateTimeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                writer.WriteValue((DateTime)value);
            }
        }

        // Other JsonConverterMethods
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                // current token is already at base64 string
                // unable to call ReadAsBytes so do it the old fashion way
                string encodedData = reader.Value.ToString();
                DateTime dateTime;
                if (DateTime.TryParse(reader.Value.ToString(), out dateTime))
                    return dateTime;

            }
            else
            {
                // throw JsonSerializationException.Create(reader, "Unexpected token parsing date time. Expected String got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            return DateTime.MinValue; 
        }
    }
}
