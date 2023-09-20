using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

// authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region Authentication and Authorization

// authentication with Azure AD, not the same as B2C, keep as a reference
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//         .AddMicrosoftIdentityWebApi(options =>
//         {
//             builder.Configuration.Bind("AzureAd", options);
//             options.TokenValidationParameters.NameClaimType = "name";
//         }, options => { builder.Configuration.Bind("AzureAd", options); });


// authentication with Azure B2C, not the same as Azure AD
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options);
    options.TokenValidationParameters.NameClaimType = "name";

    // by default the api will look for audience of "api://client-id", as the token scope comes in plain we must
    // manually specify its a valid audience
    options.TokenValidationParameters.ValidAudience = builder.Configuration["AzureAdB2C:ClientId"];
},
options => { builder.Configuration.Bind("AzureAdB2C", options); });
// End of the Microsoft Identity platform block

// Specify the access policy to require the scope "read"
builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("AuthZPolicy", policyBuilder =>
        policyBuilder.Requirements.Add(new ScopeAuthorizationRequirement() { RequiredScopesConfigurationKey = $"ReadAccessPolicy:Scopes" }));
});

#endregion

// Add services to the container.

#region Swagger API Generation

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// this is needed to make the documentation show the enum string values in the schema and examples
builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// this allows generating of documentation comments into the swagger definition
builder.Services.AddSwaggerGen(options =>
{
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    // does not generate a separate enum type, will inline to the parent definition schema on the used property
    options.UseInlineDefinitionsForEnums();

    // ensures that all the types in the output definition, including common .NET classes, are unique to this definition
    // this is important when importing multiple API specifications in another project
    options.CustomSchemaIds((current) => $"{nameof(NZEJobServices)}_{current.Name}");

    // authentication, specify api requirements in definition
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    options.OperationFilter<SecurityRequirementsOperationFilter>();

    // for swagger UI to enable specifying access token segment for making calls
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
});

#endregion

// database connection is done through the context options
builder.Services.AddDbContext<JobContext>((options) =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseInMemoryDatabase("job");
    }
    else
    {
        options.UseInMemoryDatabase("job");

        // uncomment below once we have a functioning CosmosDB environment as pipeline builds for RELEASE
        // // stand in for where the app deployment will host tenant details for the backing database
        // // values can be defined in the appsettings.Development.json file for local testing
        // var connectionString = builder.Configuration["COSMOS_CONNECTION_STRING"];
        // var dbName = builder.Configuration["COSMOS_CONFIGURATION"];

        // options.UseCosmos(connectionString, dbName);
    }
});

// Values are pulled from the azure web app
builder.Logging.AddApplicationInsights();

var app = builder.Build();

// Configure the HTTP request pipeline.
//for checking development environment -> if (app.Environment.IsDevelopment())
app.UseSwagger();
app.UseSwaggerUI();
// provides top level catch for unhandled errors, see ErrorController for how to handle response
app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

// allow debug output of incoming access tokens
if (builder.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
}

// !!!!NOTE:!!!! This has to come before app.MapControllers(); or token will be validated but user will be denied
app.UseAuthentication();
app.UseAuthorization();


// !!!!NOTE:!!!! Must come after authentication/authorization or will not work correctly
app.MapControllers();

// can access environment variables through the builder.configuration object, an example below is the application
// insights connection string defined in the web app configuration section
// System.Console.WriteLine(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

// allow cross origin requests
app
    .UseCors(options => options.SetIsOriginAllowed(x => _ = true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.Run();