using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Hangfire.RecurringJobAdmin;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);

var is_prod = builder.Configuration.GetValue<bool>("is_prod");
var hf_sql_prod = builder.Configuration.GetConnectionString("hf_sql_prod");
var hf_sql_qa = builder.Configuration.GetConnectionString("hf_sql_qa");
var user = builder.Configuration.GetSection("Access")["user"]?.ToString();
var pass = builder.Configuration.GetSection("Access")["pass"]?.ToString();

var dashboardTitle = "";
var hfCn = "";
if (is_prod) //PROD
{
    dashboardTitle = "HangFire PROD";
    hfCn = hf_sql_prod;
}

if (!is_prod) //QA
{
    dashboardTitle = "HangFire QA";
    hfCn = hf_sql_qa;
}


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
