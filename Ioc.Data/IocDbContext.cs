#nullable disable
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;

using Ioc.Core;

namespace Ioc.Data
{
    public class IocDbContext : DbContext, IDbContext
    {
        public IocDbContext() : base("name=DbConnectionString")
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType 
                && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

    }
}
