using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StronglyTypedId
{
    public class SampleContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(builder =>
            {
                builder.ToTable("Orders");
                builder.HasKey(x => x.Id);

                // Circumvent EF validation to configure the property as identity
                builder.Property(x => x.Id).UseIdentityColumn();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO change the connection string
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Database=strongly-typed-db;Integrated Security=True;MultipleActiveResultSets=False;Connection Timeout=30;");
            
            // Replace built in value converter selector
            optionsBuilder.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

            // Throw on client evaluation to ensure linq queries are evaluated on the dB
            optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
        }
    }

    internal static class PropertyTypeBuilderExtension
    {
        public static PropertyBuilder<T> UseIdentityColumn<T>(this PropertyBuilder<T> builder)
        {
            builder.Metadata.SetAnnotation(SqlServerAnnotationNames.ValueGenerationStrategy, SqlServerValueGenerationStrategy.IdentityColumn);
            return builder;
        }
    }
}