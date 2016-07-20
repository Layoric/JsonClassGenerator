using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack;
using ServiceStack.Text;

namespace JsonClassGenerator
{
    public static class JsonCSharpGenerator
    {
        public static string FromJsonObject(string json, string rootClassName = null)
        {
            JsonObject jsonObject = JsonObject.Parse(json);
            return jsonObject.FromJsonObject(rootClassName ?? "RootObject").Generate();
        }

        public static string FromJsonArray(string jsonArray, string rootClassName = null)
        {
            var jsonObject = JsonArrayObjects.Parse(jsonArray);
            return jsonObject.FromJsonArray(rootClassName ?? "RootObject").Generate();
        }
    }

    internal class JsonField
    {
        public string Name { get; set; }
        public string TypeName { get; set; }

        public bool IsObject { get; set; }
        public bool IsArray { get; set; }
        public bool IsPrimitive { get; set; }
        public Dictionary<string, JsonField> Children { get; set; }
        public List<JsonField> Childs { get; set; }
    }

    internal static class Extensions
    {
        public static JsonField FromJsonObject(this JsonObject jsonObject, string name)
        {
            JsonField result = new JsonField
            {
                Children = new Dictionary<string, JsonField>(),
                Childs = new List<JsonField>(),
                Name = name
            };
            jsonObject.ForEach((key, val) =>
            {
                if (val == null)
                {
                    JsonField tempFieldNull = new JsonField();
                    tempFieldNull.IsObject = true;
                    tempFieldNull.TypeName = "object";
                    tempFieldNull.Name = key;
                    result.Children.Add(key, tempFieldNull);
                    result.Childs.Add(tempFieldNull);
                    return;
                }
                if (val.StartsWith("{") && val.EndsWith("}"))
                {
                    var obj = JsonObject.Parse(val);
                    var childTmp = obj.FromJsonObject(key);
                    childTmp.Name = key.ToPascalCase();
                    childTmp.TypeName = key.ToPascalCase();
                    childTmp.IsObject = true;

                    result.Children.Add(key, childTmp);
                    result.Childs.Add(childTmp);
                    return;
                }
                if (val.StartsWith("[") && val.EndsWith("]"))
                {
                    string minJson = val.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                    if (minJson.Replace(" ","").StartsWith("[{"))
                    {
                        var obj = JsonArrayObjects.Parse(val);
                        var childTmp = obj.FromJsonArray(key);
                        childTmp.Name = key.ToPascalCase();
                        childTmp.TypeName = key.ToPascalCase();
                        childTmp.IsArray = true;
                        childTmp.IsObject = true;

                        result.Children.Add(childTmp.TypeName, childTmp);
                        result.Childs.Add(childTmp);
                    }
                    else
                    {
                        var field = JsonSerializer.DeserializeFromString<List<object>>(minJson);
                        var arraVal = field.First() as string;
                        JsonField arrayFieldTemp = GenerateJsonField(key, arraVal);
                        arrayFieldTemp.IsArray = true;
                        arrayFieldTemp.IsObject = false;
                        
                        result.Children.Add(key, arrayFieldTemp);
                        result.Childs.Add(arrayFieldTemp);
                    }
                    
                    return;
                }
                JsonField childFieldTmp = GenerateJsonField(key, val);
                result.Children.Add(key, childFieldTmp);
                result.Childs.Add(childFieldTmp);

            });
            return result;
        }

        private static JsonField GenerateJsonField(string key, string val)
        {
            JsonField result = new JsonField();
            result.IsPrimitive = true;
            result.Name = key;
            if (val.Trim().Equals("true") || val.Trim().Equals("false"))
            {
                result.TypeName = "bool";
                return result;
            }
            if (val.IsType<int>())
            {
                result.TypeName = "int";
                return result;
            }
            if (val.IsType<float>())
            {
                result.TypeName = "float";
                return result;
            }

            if (val.IsType<DateTime>())
            {
                result.TypeName = "DateTime";
                return result;
            }
            if (val.IsType<string>())
            {
                result.TypeName = "string";
                return result;
            }


            return result;
        }

        public static JsonField FromJsonArray(this JsonArrayObjects jsonArray, string name)
        {
            return jsonArray.First().FromJsonObject(name);
        }

        /// TODO Optimize
        public static bool IsType<T>(this string val)
        {
            bool result = false;
            try
            {
                Type type = typeof(T);
                var tVal = TypeSerializer.DeserializeFromString(val, type);
                if (tVal != null)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static string Generate(this JsonField jsonField, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();

            var objs = AllChildFields(jsonField);
            foreach (var obj in objs)
            {
                sb.AppendLine("public class {0}".Fmt(obj.Key));
                sb.AppendLine("{");
                var fields = obj.Value.Children;
                foreach (var field in fields)
                {
                    if (field.Value.IsArray)
                    {
                        sb.AppendLine("    public List<{0}> {1} ".Fmt(field.Value.TypeName, field.Value.Name.ToPascalCase()) + "{ get;set; }");
                        continue;
                    }
                    sb.AppendLine("    public {0} {1} ".Fmt(field.Value.TypeName, field.Value.Name.ToPascalCase()) + "{ get;set; }");
                }
                sb.AppendLine("}");
                sb.AppendLine();
            }


            return sb.ToString();
        }

        public static Dictionary<string, JsonField> AllChildFields(JsonField parent, Dictionary<string, JsonField> result = null)
        {
            result = result ?? new Dictionary<string, JsonField>();
            if (parent == null)
                return result;
            if (parent.Children == null)
                return result;
            parent.Children.ForEach((k, v) =>
            {
                AllChildFields(v, result);
            });
            if (parent.IsObject)
                result.Add(parent.TypeName, parent);
            if (parent.Name == "RootObject")
            {
                parent.TypeName = "RootObject";
                result.Add("RootObject", parent);
            }
            return result;
        }

        public static List<string> CSharpTypes = new List<string>()
        {
            "int",
            "float",
            "bool",
            "string"
        };
    }
}
