using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace JsonParseSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("App started.");

            var json = "{\"payload\":\"{\\\"clickType\\\":2,\\\"clickTypeName\\\":\\\"DOUBLE\\\",\\\"batteryLevel\\\":1,\\\"binaryParserEnabled\\\":true}\"}";
            var objFromJson = JObject.Parse(json);
            var payload = objFromJson
                    .SelectToken("payload").SelectToken("clickTypeName");



            Console.WriteLine(payload);

            Console.ReadLine();

        }
    }
}
