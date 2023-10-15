using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;

namespace appLauncher.Core.Serializers
{
    public class ApporFolderConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IApporFolder);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Read the type name from the JSON
            JObject jo = JObject.Load(reader);
            string typeName = jo["$type"].Value<string>();

            // Create an instance of the appropriate class based on the type name
            IApporFolder animal = null;
            switch (typeName)
            {
                case "FinalTiles":
                    animal = new FinalTiles();
                    break;
                case "AppFolder":
                    animal = new AppFolder();
                    break;
                default:
                    throw new JsonSerializationException("Unknown animal type: " + typeName);
            }

            // Populate the properties from the JSON
            serializer.Populate(jo.CreateReader(), animal);

            return animal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Write the type name and the properties to the JSON
            JObject jo = new JObject();
            jo.Add("$type", value.GetType().Name);
            jo.Add("Name", ((IApporFolder)value).Name);
            jo.WriteTo(writer);
        }
        public override bool CanWrite => true;
        public override bool CanRead => true;
    }

}
