using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mm.Models
{
    public class ErrorModel
    {
        public string Message { get; set; }

        public ErrorModel(string errorMessage)
        {
            Message = errorMessage;
        }
    }
}
