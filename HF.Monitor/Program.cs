using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Hangfire.RecurringJobAdmin;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);


IConfiguration configEnvironment;

var pre_builder = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var pre_config = pre_builder.Build();
var is_prod = Convert.ToBoolean(pre_config["is_prod"]);

var dashboardTitle = "";

if (is_prod)
{
    var builderEnvironment = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.prod.json", optional: false, reloadOnChange: true);

    configEnvironment = builderEnvironment.Build();

    dashboardTitle = "HangFire PROD";
}
else
{
    var builderEnvironment = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("appsettings.qa.json", optional: false, reloadOnChange: true);

    configEnvironment = builderEnvironment.Build();

    dashboardTitle = "HangFire QA";
}


var hfCn = configEnvironment.GetConnectionString("HangFire");
var user = configEnvironment.GetSection("Access")["user"]?.ToString();
var pass = configEnvironment.GetSection("Access")["pass"]?.ToString();



// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseColouredConsoleLogProvider()
    .UseResultsInContinuations()
    .UseConsole()
    .UseSqlServerStorage(hfCn, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true,
        DashboardJobListLimit = 50000,
        EnableHeavyMigrations = true
    })
    .UseRecurringJobAdmin()
);

builder.Services.AddHangfireServer();

//builder.Services.AddTransient<IServiceCollection, Service>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();


app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("hangfire");
        return Task.FromResult(0);
    });
});

var supportedCultures = new[]
{
    new CultureInfo("es-PE"),
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es-PE"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// authentication to Dashboard
var _dashOption = new DashboardOptions()
{
    DashboardTitle = dashboardTitle,
    Authorization = new [] {
        new HangfireCustomBasicAuthenticationFilter()
        {
            User = user,
            Pass = pass
        }
    }
};

app.UseHangfireDashboard("/hangfire", _dashOption);
app.MapHangfireDashboard();

app.MapRazorPages();

app.Run();
