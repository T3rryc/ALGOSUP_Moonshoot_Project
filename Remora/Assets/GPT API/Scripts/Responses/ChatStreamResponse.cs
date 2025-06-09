using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TzarGPT
{
    public class ChatStreamResponse
    {
        public Data data { get; set; }

        public class Choice
        {
            public int index { get; set; }
            public Delta delta { get; set; }
            public object logprobs { get; set; }
            public object finish_reason { get; set; }
        }

        public class Data
        {
            public string id { get; set; }
            public string @object { get; set; }
            public int created { get; set; }
            public string model { get; set; }
            public object system_fingerprint { get; set; }
            public List<Choice> choices { get; set; }
        }

        public class Delta
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }

}