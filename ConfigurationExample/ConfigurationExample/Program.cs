using ConfigurationExample;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//Supply an object of WeatherApiOptions (with 'weatherapi' section)
builder.Services.Configure<WeatherApiOption>(builder.Configuration.GetSection("weatherapi"));
var app = builder.Build();


//Load MyOwnConfig.json
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("MyOwnConfig.json",optional:true,reloadOnChange:true);
});
app.UseStaticFiles();
app.UseRouting();


app.MapControllers();
app.Run();
