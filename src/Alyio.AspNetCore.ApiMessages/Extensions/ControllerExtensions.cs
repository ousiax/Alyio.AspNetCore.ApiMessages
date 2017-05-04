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
        /// <summary>
        /// Create a created message that produces a Created (201) response.
        /// </summary>
        /// <param name="controller">the <see cref="Controller"/>.</param>
        /// <param name="actionName">The name of the action to use for generating the URL.</param>
        /// <param name="routeValues">The content value to format in the entity body.</param>
        /// <param name="id">A <see cref="string"/> value to identity the entity.</param>
        /// <returns>The <see cref="Alyio.AspNetCore.ApiMessages.CreatedMessage"/>.</returns>
        public static CreatedMessage CreatedMessageAtAction(this Controller controller, string actionName, object routeValues, string id)
        {
            string url = controller.LinkAtAction(actionName, routeValues);
            return controller.CreatedMessage(url, id);
        }

        /// <summary>
        /// Create a created message that produces a Created (201) response.
        /// </summary>
        /// <param name="controller">the <see cref="Controller"/>.</param>
        /// <param name="routeName">The name of the route to use for generating the URL.</param>
        /// <param name="routeValues">The content value to format in the entity body.</param>
        /// <param name="id">A <see cref="string"/> value to identity the entity.</param>
        /// <returns>The <see cref="Alyio.AspNetCore.ApiMessages.CreatedMessage"/>.</returns>
        public static CreatedMessage CreatedMessageAtRoute(this Controller controller, string routeName, object routeValues, string id)
        {
            string url = LinkAtRoute(controller, routeName, routeValues);
            return controller.CreatedMessage(url, id);
        }

        /// <summary>
        /// Create a created message that produces a Created (201) response.
        /// </summary>
        /// <param name="controller">the <see cref="Controller"/>.</param>
        /// <param name="uri">The URL used to get the entity.</param>
        /// <param name="id">A <see cref="string"/> value to identity the entity.</param>
        /// <returns>The <see cref="Alyio.AspNetCore.ApiMessages.CreatedMessage"/>.</returns>
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

        /// <summary>
        /// Generates an absolute URL using the specified route name and values.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="routeName">The name of the route that is used to generate the URL.</param>
        /// <param name="routeValues">An object that contains the route values.</param>
        /// <returns>The generated absolute URL.</returns>
        /// <remarks>The protocol and host is obtained from the current request.</remarks>
        public static string LinkAtRoute(this Controller controller, string routeName, object routeValues)
        {
            return controller.Url.Link(routeName, routeValues);
        }

        /// <summary>
        /// Generates an absolute URL using the specified route name and values.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/>.</param>
        /// <param name="actionName">The name of the action method that used to generate URLs.</param>
        /// <param name="routeValues">An object that contains the route values.</param>
        /// <returns>The generated absolute URL.</returns>
        /// <remarks>The protocol and host is obtained from the current request.</remarks>
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
