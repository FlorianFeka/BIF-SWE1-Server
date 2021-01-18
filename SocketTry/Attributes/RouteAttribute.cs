using System;

namespace SocketTry.Attributes
{
    public class RouteAttribute : ControllerDataAttribute
    {
        /// <param name="route">not allowed to be null</param>
        public RouteAttribute(string route)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));
            Route = route;
        }

        public string Route { get; set; }
    }
}
