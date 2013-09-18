using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharpTests.Core
{
    public class Route
    {
        public GeographicCoordinate Start { get; set; }
        public GeographicCoordinate End { get; set; }

        public IList<GeographicCoordinate> Points { get; set; }
        public double Length { get; set; }
        public double TravelTimeSeconds { get; set; }
    }
}
