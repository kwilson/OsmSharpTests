using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharpTests.MapEngine
{
    using System.Diagnostics;
    using System.IO;

    using OsmSharp.Osm;
    using OsmSharp.Osm.Data.PBF.Raw.Processor;
    using OsmSharp.Osm.Data.Processor.Filter.Sort;
    using OsmSharp.Osm.Data.XML.Processor;
    using OsmSharp.Routing;
    using OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed;
    using OsmSharp.Routing.Graph.Memory;
    using OsmSharp.Routing.Graph.Router.Dykstra;
    using OsmSharp.Routing.Osm.Data.Processing;
    using OsmSharp.Routing.Osm.Interpreter;
    using OsmSharp.Routing.Route;
    using OsmSharp.Tools.Math.Geo;

    using OsmSharpTests.Core;

    public class Engine
    {
        private static volatile Engine instance;

        private static readonly object syncRoot = new Object();

        private static IRouter<RouterPoint> router;

        private Engine()
        {
            // keeps a memory-efficient version of the osm-tags.
            var tagsIndex = new OsmTagsIndex();

            // creates a routing interpreter. (used to translate osm-tags into a routable network)
            var interpreter = new OsmRoutingInterpreter();

            // create a routing datasource, keeps all processed osm routing data.
            var osmData = new MemoryRouterDataSource<SimpleWeighedEdge>(tagsIndex);

            // load data into this routing datasource.
            Stream osmPbfData = new FileInfo(@"D:\projects\OsmSharpTests\OsmSharpTests\App_Data\scotland-latest.osm.pbf").OpenRead(); // for example moscow!
            using (osmPbfData)
            {
                var targetData = new
                   SimpleWeighedDataGraphProcessingTarget(
                                osmData, interpreter, osmData.TagsIndex, VehicleEnum.Car);

                // replace this with PBFdataProcessSource when having downloaded a PBF file.
                var dataProcessorSource = new
                  PBFDataProcessorSource(osmPbfData);

                // pre-process the data.
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var sorter = new DataProcessorFilterSort();
                sorter.RegisterSource(dataProcessorSource);
                targetData.RegisterSource(sorter);
                targetData.Pull();

                stopwatch.Stop();
            }

            // create the router object: there all routing functions are available.
            router = new Router<SimpleWeighedEdge>(
                osmData, interpreter, new DykstraRoutingLive(osmData.TagsIndex));
        }

        public GeographicCoordinate ResolvePoint(GeographicCoordinate coordinate)
        {
            var position = ToGeoCoordinate(coordinate);
            var resolved = router.Resolve(VehicleEnum.Car, position).Location;

            return new GeographicCoordinate { Latitude = resolved.Latitude, Longitude = resolved.Longitude };
        }

        public Route GetRoute(IEnumerable<GeographicCoordinate> points)
        {
            var geographicCoordinates = points as IList<GeographicCoordinate> ?? points.ToList();
            if (geographicCoordinates.Count() < 2)
            {
                return null;
            }

            // resolve both points; find the closest routable road.
            var resolvedPoints = geographicCoordinates.Select(x => router.Resolve(VehicleEnum.Car, ToGeoCoordinate(x))).ToList();

            // Set up container
            var coordinates = new List<GeoCoordinate>();
            double totalDistance = 0;
            double totalTime = 0;

            for (var i = 1; i < resolvedPoints.Count; i++)
            {
                var fromPoint = resolvedPoints[i - 1];
                var toPoint = resolvedPoints[i];

                // calculate route.
                var osmRoute = router.Calculate(VehicleEnum.Car, fromPoint, toPoint);
                if (osmRoute == null)
                {
                    return null;
                }

                coordinates.AddRange(osmRoute.GetPoints().Skip(1));
                totalDistance += osmRoute.TotalDistance;
                totalTime += osmRoute.TotalTime;
            }

            return new Route
                       {
                           Start = ToGeographicCoordinate(coordinates.First()),
                           End = ToGeographicCoordinate(coordinates.Last()),
                           Points = coordinates.Select(ToGeographicCoordinate).ToList(),
                           Length = totalDistance,
                           TravelTimeSeconds = totalTime
                       };
        }

        private static GeoCoordinate ToGeoCoordinate(GeographicCoordinate geographicCoordinate)
        {
            var position = new GeoCoordinate(geographicCoordinate.Latitude, geographicCoordinate.Longitude);
            return position;
        }

        private static GeographicCoordinate ToGeographicCoordinate(GeoCoordinate geoCoordinate)
        {
            return new GeographicCoordinate { Latitude = geoCoordinate.Latitude, Longitude = geoCoordinate.Longitude };
        }

        public static Engine Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Engine();
                        }
                    }
                }

                return instance;
            }
        }
    }
}
