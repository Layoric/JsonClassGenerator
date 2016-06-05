using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using DtoGen.ServiceModel;
using JsonClassGenerator;

namespace DtoGen.ServiceInterface
{
    public class MyServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Hello, {0}!".Fmt(request.Name) };
        }

        public JsonToDtoResponse Post(JsonToDto request)
        {
            return new JsonToDtoResponse
            {
                Dtos = JsonCSharpGenerator.FromJsonObject(request.Json)
            };
        }
    }
}