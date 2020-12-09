using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using ServiceStack.Web;
using SqlServer.ServiceModel;

namespace SqlServer.ServiceInterface
{
    public class MyServices : Service
    {
        public object Any(Hello request)
        {
            return new HTTPResult("Hello, {0}!".Fmt(request.Name)) {
                ContentType = MimeTypes.PlainText,
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
