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
                    var obj = JsonArrayObjects.Parse(val);
                    var childTmp = obj.FromJsonArray(key);
                    childTmp.Name = key.ToPascalCase();
                    childTmp.TypeName = key.ToPascalCase();
                    childTmp.IsArray = true;
                    childTmp.IsObject = true;

                    result.Children.Add(childTmp.TypeName, childTmp);
                    result.Childs.Add(childTmp);
                    return;
                }
                JsonField childFieldTmp = new JsonField();

                if (val.IsType<int>())
                {
                    childFieldTmp.IsPrimitive = true;
                    childFieldTmp.TypeName = "int";
                    childFieldTmp.Name = key;

                    result.Children.Add(key, childFieldTmp);
                    result.Childs.Add(childFieldTmp);
                    return;
                }

                if (val.IsType<float>())
                {
                    childFieldTmp.IsPrimitive = true;
                    childFieldTmp.TypeName = "float";
                    childFieldTmp.Name = key;

                    result.Children.Add(key, childFieldTmp);
                    result.Childs.Add(childFieldTmp);
                    return;
                }

                if (val.IsType<bool>())
                {
                    childFieldTmp.IsPrimitive = true;
                    childFieldTmp.TypeName = "bool";
                    childFieldTmp.Name = key;

                    result.Children.Add(key, childFieldTmp);
                    result.Childs.Add(childFieldTmp);
                    return;
                }

                if (val.IsType<DateTime>())
                {
                    childFieldTmp.IsPrimitive = true;
                    childFieldTmp.TypeName = "DateTime";
                    childFieldTmp.Name = key;

                    result.Children.Add(key, childFieldTmp);
                    result.Childs.Add(childFieldTmp);
                    return;
                }

                if (val.IsType<string>())
                {
                    childFieldTmp.IsPrimitive = true;
                    childFieldTmp.TypeName = "string";
                    childFieldTmp.Name = key;

                    result.Children.Add(key, childFieldTmp);
                    result.Childs.Add(childFieldTmp);
                }
            });
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
                        sb.AppendLine("    public List<{0}> {1} ".Fmt(field.Value.TypeName, field.Value.Name.ToTitleCase()) + "{ get;set; }");
                        continue;
                    }
                    sb.AppendLine("    public {0} {1} ".Fmt(field.Value.TypeName, field.Value.Name.ToTitleCase()) + "{ get;set; }");
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
