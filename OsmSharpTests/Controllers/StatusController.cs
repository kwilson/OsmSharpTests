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
        public string Get()
        {
            if (Engine.Status == EngineStatus.Running)
            {
                return string.Format("Running since {0}", Engine.LastStatusChange);
            }

            if (Engine.Status == EngineStatus.Starting)
            {
                return string.Format("Starting up since {0}", Engine.LastStatusChange);
            }

            return "Not running.";
        }
    }
}
