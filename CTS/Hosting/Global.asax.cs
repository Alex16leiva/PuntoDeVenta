using Funq;
using Hosting.Services;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Web;

namespace Hosting
{
    public class Global : HttpApplication
    {
        public class TestAppHost : AppHostBase
        {
            public TestAppHost() : base("ServiceIngenieria Service", typeof(ServiceIngenieria).Assembly)
            {

            }

            public override void Configure(Container container)
            {
                var con = ServiceStack.ServerIntance.Container.Current;
                container.Adapter = new UnityContainerAdapter(con);
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            new TestAppHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}