using CityInfo.API;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders(); // Inbuild logger no need using serilog
//builder.Logging.AddConsole(); // Inbuild logger no need using serilog

builder.Host.UseSerilog(); //register serilog

// Add services to the container.

//builder.Services.AddControllers();

builder.Services.AddControllers(options =>
{
    // options.InputFormatters(options => ...
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

builder.Services.AddProblemDetails();

//builder.Services.AddProblemDetails(options =>
//{
//    options.CustomizeProblemDetails = ctx =>
//    {
//        ctx.ProblemDetails.Extensions.Add("additionalInfo", "Additional info example");
//        ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
//    };
//});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); // mapping file extentions in requestion application/pdf

builder.Services.AddSingleton<CitiesDataStore>();

# if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
# else
builder.Services.AddTransient<IMailService, CloudMailService>();
# endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
