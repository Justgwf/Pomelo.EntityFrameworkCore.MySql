using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindQueryMySqlFixture<TModelCustomizer> : NorthwindQueryRelationalFixture<TModelCustomizer>
        where TModelCustomizer : IModelCustomizer, new()
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlNorthwindTestStoreFactory.Instance;

        protected override bool ShouldLogCategory(string logCategory)
            => logCategory == DbLoggerCategory.Query.Name || logCategory == DbLoggerCategory.Database.Command.Name;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder)
                .EnableDetailedErrors();

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            var northwindContext = (NorthwindRelationalContext)context;

            modelBuilder
                .Entity<OrderQuery>()
                .HasNoKey()
                .ToQuery(() => northwindContext.Orders
                    .FromSqlRaw("select * from `Orders`")
                    .Select(o => new OrderQuery { CustomerID = o.CustomerID }));

            modelBuilder
                .Entity<CustomerView>()
                .HasNoKey()
                .ToQuery(() => northwindContext.CustomerQueries.FromSqlInterpolated(
                    $"SELECT CONCAT(`c`.`CustomerID`, {string.Empty}) as CustomerID, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region` FROM `Customers` AS `c`"));
        }
    }
}
