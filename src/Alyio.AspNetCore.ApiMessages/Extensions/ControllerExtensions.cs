using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Extension methods for the <see cref="Controller"/>.
    /// </summary>
    public static class ControllerExtensions
    {
        public static CreatedMessage CreatedMessageAtAction(this Controller controller, string actionName, object routeValues, string id)
        {
            string url = controller.LinkAtAction(actionName, routeValues);
            return controller.CreatedMessage(url, id);
        }

        public static CreatedMessage CreatedMessageAtRoute(this Controller controller, string routeName, object routeValues, string id)
        {
            string url = LinkAtRoute(controller, routeName, routeValues);
            return controller.CreatedMessage(url, id);
        }

        public static CreatedMessage CreatedMessage(this Controller controller, string uri, string id)
        {
            controller.Response.StatusCode = 201;
            controller.Response.Headers[HeaderNames.Location] = uri;
            return new CreatedMessage
            {
                Id = id,
                Links = new List<Link> {
                    new Link {
                        Href = uri,
                        Rel = "self"
                    }
                }
            };
        }

        public static string LinkAtRoute(this Controller controller, string routeName, object routeValues)
        {
            return controller.Url.Link(routeName, routeValues);
        }

        public static string LinkAtAction(this Controller controller, string actionName, object routeValues)
        {
            return controller.Url.Action(
                               new UrlActionContext
                               {
                                   Action = actionName,
                                   Values = routeValues
                               });
        }
    }
}
