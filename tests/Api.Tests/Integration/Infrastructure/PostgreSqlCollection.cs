using Xunit;

namespace Api.Tests.Integration.Infrastructure;

[CollectionDefinition("PostgreSql")]
public class PostgreSqlCollection :
    ICollectionFixture<PostgreSqlTestFixture>
{
}