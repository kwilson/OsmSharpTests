using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OsmSharpTests.Models
{
    using OsmSharpTests.Core;

    public class RouteModel
    {
        public RouteModel(Route route)
        {
            this.Time = route.TravelTimeSeconds;
            this.Length = route.Length;
            this.Points = route.Points.Select(x => new[] { x.Latitude, x.Longitude }).ToList();
        }

        public string Id { get; set; }
        public IEnumerable<double[]> Points { get; set; }

        public double Time { get; set; }
        public double Length { get; set; }
    }
}