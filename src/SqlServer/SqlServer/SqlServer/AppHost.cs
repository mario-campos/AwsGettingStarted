﻿using Funq;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using SqlServer.ServiceInterface;
using SqlServer.ServiceModel.Types;
using ServiceStack.Host.Handlers;
using ServiceStack.Web;

namespace SqlServer
{
    public class AppHost : AppSelfHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("SqlServer", typeof(MyServices).Assembly)
        {

        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            container.Register<IDbConnectionFactory>(c => new OrmLiteConnectionFactory(
            AppSettings.GetString("ConnectionString"), SqlServerDialect.Provider));

            using (var db = container.Resolve<IDbConnectionFactory>().Open())
            {
                if (db.CreateTableIfNotExists<Customer>())
                {
                    //Add seed data
                }
            }
            
            void allowOriginFilter(IRequest httpReq, IResponse httpRes)
            {
                var origin = httpReq.Headers.Get(HttpHeaders.Origin);
                if (origin != null)
                {
                    httpRes.AddHeader(HttpHeaders.AllowOrigin, origin);
                }
            }
            
            this.PreRequestFilters.Add(allowOriginFilter);
            
            this.GlobalRequestFilters.Add((req, res, requestDto) => {
                res.ReturnAuthRequired();
            });
            
            RegisterTypedRequestFilter<CreateCustomer>((req, res, dto) => {
                res.ReturnAuthRequired();
            });
        }
    }
}
