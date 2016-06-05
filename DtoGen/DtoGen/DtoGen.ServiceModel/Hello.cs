using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;

namespace DtoGen.ServiceModel
{
    [Route("/hello")]
    [Route("/hello/{Name}")]
    public class Hello : IReturn<HelloResponse>
    {
        public string Name { get; set; }
    }

    public class HelloResponse
    {
        public string Result { get; set; }
    }

    [Route("/json/{Language}")]
    public class JsonToDto : IReturn<JsonToDtoResponse>
    {
        public string Json { get; set; }
        public string Language { get; set; }
        public string RootClassName { get; set; }
    }

    public class JsonToDtoResponse
    {
        public string Dtos { get; set; }
    }

}