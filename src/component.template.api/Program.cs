using System.Text.Json.Serialization;
using Aurora.Mediator.Extensions;
using component.template.api.business.Behaviors;
using component.template.api.configuration;
using component.template.api.configuration.General;
using component.template.api.domain.Filters;
using component.template.api.domain.Helpers;

var builder = WebApplication.CreateBuilder(args);
var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => a.GetTypes().Any(t => t.Namespace != null && t.Namespace.StartsWith("component.template.api")))
    .ToArray();


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ServiceExceptionFilter>();
    options.Filters.Add<ServiceResultFilter>();
})
.AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()
));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddConfiguration(new SwaggerConfiguration(builder.Configuration));
builder.Services.AddConfiguration(new DependencyInjectionConfig(builder.Configuration));
//builder.Services.AddConfiguration(new AuthenticationConfiguration(builder.Configuration));    
builder.Services.AddAuroraMediator(assemblies)
    .AddPipelineBehavior(typeof(LoggingBehavior<,>))
    .UseDefaultValidation(assemblies);
    // .UseDefaultLogging();
    //.AddPipelineBehavior(typeof(ValidationBehaviorWithFluent<,>))  // aqui você pluga FluentValidation;

// registra todos os validators que herdam de AbstractValidator<T> para usar validator com fluent validation
// builder.Services.AddValidatorsFromAssemblies(new[]
// {
//     typeof(CreateUserCommandValidator).Assembly,
//     typeof(Program).Assembly
// });

var app = builder.Build();

HttpHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseMiddleware<component.template.api.domain.Middleware.ServiceHttpContextMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();