using component.template.domain.Exceptions;
using component.template.domain.Interfaces.Handle;
using component.template.domain.Interfaces.Infrastructure.Repository;
using component.template.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.domain.Models.Handle;
using component.template.domain.Models.Repository.Contexts;
using component.template.infrastructure.Repository.SqlServer;
using component.template.infrastructure.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace component.template.configuration.General
{

    public class DependencyInjectionConfig : IGeneralApiConfig
     {
          private IConfiguration _configuration { get; }
          public DependencyInjectionConfig(IConfiguration configuration)
          {
               _configuration = configuration;
          }

          public void Initialize(IServiceCollection services)
          {
               DatabaseBuilder(services);
               ServiceBuilder(services);
               BusinessBuilder(services);
               DataBuilder(services);
               CommonBuilder(services);
          }

          private void DatabaseBuilder(IServiceCollection services)
          {
               //services.AddDbContext<infrastructure.Context.CosmosContext>(o =>
               //   o.UseCosmos("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", "gpsocial-dev"));

               if (bool.TryParse(_configuration["Database:Cosmos:Active"], out bool cosmosActive))
               {
                    var cosmosConnectionString = _configuration["Database:Cosmos:ConnectionString"] 
                         ?? throw new ConfigurationNotFoundExceptionException("Database:Cosmos:ConnectionString");
                    var cosmosDatabaseName = _configuration["Database:Cosmos:DatabaseName"] 
                         ?? throw new ConfigurationNotFoundExceptionException("Database:Cosmos:DatabaseName");
                    
                    // Registrar DbContextFactory PRIMEIRO (como Pooled para evitar conflito com Scoped)
                    services.AddPooledDbContextFactory<CosmosContext>(o =>
                      o.UseCosmos(cosmosConnectionString, cosmosDatabaseName)
                    );
                    
                    // Depois registrar DbContext (para migrations e uso geral)
                    services.AddDbContext<CosmosContext>(o =>
                      o.UseCosmos(cosmosConnectionString, cosmosDatabaseName)
                    );
               }
               if (bool.TryParse(_configuration["Database:Sql:Active"], out bool sqlActive))
               {
                    var sqlConnectionString = _configuration["Database:Sql:ConnectionString"] 
                         ?? throw new ConfigurationNotFoundExceptionException("Database:Sql:ConnectionString");
                    
                    // Registrar DbContextFactory PRIMEIRO (como Pooled para evitar conflito com Scoped)
                    services.AddPooledDbContextFactory<SqlContext>(options =>
                         options.UseSqlServer(sqlConnectionString, b => 
                              b.MigrationsAssembly("component.template.infrastructure")
                              .MigrationsHistoryTable("__EFMigrationsHistory", "dbo")));
                    
                    // Depois registrar DbContext (para migrations e uso geral)
                    services.AddDbContext<SqlContext>(options =>
                         options.UseSqlServer(sqlConnectionString, b => 
                              b.MigrationsAssembly("component.template.infrastructure")
                              .MigrationsHistoryTable("__EFMigrationsHistory", "dbo")));
               }
          }

          private void ServiceBuilder(IServiceCollection services)
          {
               // services.AddScoped<ServiceReferences.IRelationshipService>(_ =>
               //   new ServiceReferences.RelationshipService(
               //           _configuration["Services:Relationship:Uri"],
               //           new()
               //           {
               //                BaseAddress = new Uri(_configuration["Services:Relationship:Uri"])
               //           }
               //   ));

               //services.AddHttpClient<ServiceReferences.IRelationshipService, ServiceReferences.RelationshipService>((provider, client) =>
               //    client.BaseAddress = new Uri("https://localhost:7114"));
          }

          private void BusinessBuilder(IServiceCollection services)
          {
               services.AddScoped<IErrorHandle, ErrorHandle>();
          }

          private void DataBuilder(IServiceCollection services)
          {
               // Registrar UserRepository com DbContextFactory para paginação otimizada
               services.AddScoped<IUserRepository>(provider =>
               {
                    var context = provider.GetRequiredService<SqlContext>();
                    var factory = provider.GetRequiredService<IDbContextFactory<SqlContext>>();
                    return new UserRepository(context, factory);
               });
               
               services.AddScoped<IUnitOfWork, UnitOfWorkForSql>();
          }

          private void CommonBuilder(IServiceCollection services)
          {
               // services.AddScoped<HttpHelper>();
          }
     }
}