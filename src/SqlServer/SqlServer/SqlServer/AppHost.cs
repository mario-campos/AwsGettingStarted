using Funq;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using SqlServer.ServiceInterface;
using SqlServer.ServiceModel.Types;

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
            
            this.GlobalRequestFilters.Add((req, res, requestDto) => {
                res.ReturnAuthRequired();
            });
            
            this.GlobalRequestFiltersAsync.Add(async (req, res, dto) => {
                if (string.Equals(req.UserHostAddress, "1.1.1.1")) 
                {
                    res.StatusCode = 403;
                    res.StatusDescription = "RateLimitExceeded";
                    res.EndRequest();
                }
            });
        }
    }
}
