using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OsmSharpTests.Controllers
{
    using System.Web;

    using OsmSharpTests.Core;
    using OsmSharpTests.Models;

    public class RoutingController : ApiController
    {
        public RouteModel Post(RouteQueryModel routeQueryModel)
        {
            var engine = MapEngine.Engine.Instance;
            var route = engine.GetRoute(routeQueryModel.Points);
            if (route == null)
            {
                throw new HttpException(500, "Error creating route.");
            }

            var model = new RouteModel(route);
            model.Id = routeQueryModel.Id;
            return model;
        }
    }
}
