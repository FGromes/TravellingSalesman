using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace TravellingSalesmanV2
{
    internal static class JsonHandler
    {
        static JsonHandler() { }

        private static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            MaxDepth = 128,
            WriteIndented = true
        };

        private static string fileName = "Map.json";

        public static async Task SaveToJson(Map map)
        {
            FileStream createStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream, map, options);
            createStream.Dispose();
        }

        public static Map LoadMapFromJson()
        {
            Map result;
            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                if (jsonString is not null && jsonString != "")
                {
                    result = JsonSerializer.Deserialize<Map>(jsonString, options);
                    if (result is not null)
                        return result;
                }
            }
            result = new Map(MapPoint.GetDefaultMapPoints());
            return result;
        }
    }
}
