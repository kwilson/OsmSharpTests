using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OsmSharpTests.Models
{
    using OsmSharpTests.Core;

    public class RouteQueryModel
    {
        public string Id { get; set; }
        public IEnumerable<GeographicCoordinate> Points { get; set; }
    }
}