using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OsmSharpTests.Controllers
{
    using OsmSharpTests.Core;

    public class PointController : ApiController
    {
        // POST api/point
        public double[] Post(GeographicCoordinate position)
        {
            var engine = MapEngine.Engine.Instance;
            var point = engine.ResolvePoint(position);

            return new[] { point.Latitude, position.Longitude };
        }
    }
}
