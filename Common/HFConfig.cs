using Microsoft.Extensions.Configuration;

namespace Common
{
    public class HFConfig
    {
        public bool IsProd { get; }
        public SqlConfig SqlConfig { get; }

        public HFConfig()
        {
            IConfiguration config;

            var pre_builder = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var pre_config = pre_builder.Build();

            this.IsProd = Convert.ToBoolean(pre_config["is_prod"]);

            if (IsProd)
            {
                var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.prod.json", optional: false, reloadOnChange: true);

                config = builder.Build();
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile("appsettings.qa.json", optional: false, reloadOnChange: true);

                config = builder.Build();
            }

            //SQL
            SqlConfig = new SqlConfig
            {
                Cn = config.GetConnectionString("TestData")
            };

        }
    }
}