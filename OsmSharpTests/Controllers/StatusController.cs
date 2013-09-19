using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OsmSharpTests.Controllers
{
    using OsmSharpTests.MapEngine;

    public class StatusController : ApiController
    {
        // GET api/status
        public bool Get()
        {
            return Engine.IsReady;
        }
    }
}
