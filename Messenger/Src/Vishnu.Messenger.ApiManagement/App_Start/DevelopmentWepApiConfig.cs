using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using Unity;
using Unity.Lifetime;
using Vishnu.Messenger.ApiManagement.Models;
using Vishnu.Messenger.ApiManagement.Resolvers;

namespace Vishnu.Messenger.ApiManagement
{
    public class DevelopmentWepApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();
            container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
            name: "UsersLocal",
            routeTemplate: "api/v2/users/{id}",
            defaults: new { controller = "UsersLocal", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}