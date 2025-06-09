using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TzarGPT
{
    public class ErrorResponse
    {
        public Error error { get; set; }

        public class Error
        {
            public string message { get; set; }
            public string type { get; set; }
            public object param { get; set; }
            public object code { get; set; }
        }
    }
}