using AntLib.Model.Layer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Tools
{
    public static class Serializer
    {
        public static string Serialize(object obj)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };

            Newtonsoft.Json.Serialization.DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            dcr.SerializeCompilerGeneratedMembers = true;
            jss.ContractResolver = dcr;

            return JsonConvert.SerializeObject(obj, jss);
        }

        public static object Deserialize(string json)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };

            Newtonsoft.Json.Serialization.DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            dcr.SerializeCompilerGeneratedMembers = true;
            jss.ContractResolver = dcr;

            return JsonConvert.DeserializeObject(json, jss);
        }
    }
}
