using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class AutoMapperException : Exception
    {
        public AutoMapperException(string message) : base(message)
        {
            this.StatusCode = StatusCode;
        }
        public int StatusCode
        {
            get; set;
        }
    }
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {
            this.StatusCode = StatusCode;
        }
        public int StatusCode
        {
            get;set;
        }
    }
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
            this.StatusCode = StatusCode;
        }
        public int StatusCode
        {
            get; set;
        }
    }

}
