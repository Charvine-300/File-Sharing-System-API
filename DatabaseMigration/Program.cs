using Microsoft.Data.SqlClient;
using Npgsql;
using NpgsqlTypes;



var sourceConnectionString =
    @"Data Source=localhost\SQLEXPRESS;Initial Catalog=FinalYearProjectDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

var destinationConnectionString = @"Host=aws-1-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.shldiqikdimfijrtwcum;Password=Engineerphil200$;SSL Mode=Require;Trust Server Certificate=true";

await using var sqlConnection = new SqlConnection(sourceConnectionString);
await using var postgresConnection = new NpgsqlConnection(destinationConnectionString);


if (string.IsNullOrWhiteSpace(sourceConnectionString))
{
    Console.WriteLine("SOURCE_SQLSERVER_CONNECTION is not set.");
    return;
}

if (string.IsNullOrWhiteSpace(destinationConnectionString))
{
    Console.WriteLine("SUPABASE_CONNECTION is not set.");
    return;
}


try
{
    Console.WriteLine("Connecting to SQL Server...");
    await sqlConnection.OpenAsync();

    Console.WriteLine("Connecting to Supabase...");
    await postgresConnection.OpenAsync();

    Console.WriteLine("Both database connections successful.");
    Console.WriteLine();

    await using var transaction =
        await postgresConnection.BeginTransactionAsync();

    try
    {
        await MigrateAuditLogs(
            sqlConnection,
            postgresConnection,
            transaction);

        await transaction.CommitAsync();

        Console.WriteLine();
        Console.WriteLine("==========================================");
        Console.WriteLine("AUDIT LOG MIGRATION COMPLETED SUCCESSFULLY");
        Console.WriteLine("==========================================");
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine("==========================================");
        Console.WriteLine("AUDIT LOG MIGRATION FAILED");
        Console.WriteLine("==========================================");
        Console.WriteLine(ex);

        await transaction.RollbackAsync();

        Console.WriteLine();
        Console.WriteLine("All PostgreSQL changes have been rolled back.");
    }
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine("DATABASE CONNECTION FAILED");
    Console.WriteLine(ex);
}


// ============================================================
// AUDIT LOGS
// ============================================================

static async Task MigrateAuditLogs(
    SqlConnection source,
    NpgsqlConnection destination,
    NpgsqlTransaction transaction)
{
    Console.WriteLine("Migrating AuditLogs...");

    const string selectSql = """
        SELECT
            Id,
            Action,
            Description,
            Actor,
            ActionType,
            ActorType,
            CreatedAt,
            ModifiedAt,
            CreatedBy,
            ModifiedBy,
            Deleted,
            DeletedAt
        FROM dbo.AuditLogs
        ORDER BY CreatedAt;
        """;

    const string insertSql = """
        INSERT INTO "AuditLogs"
        (
            "Id",
            "Action",
            "Description",
            "Actor",
            "ActionType",
            "ActorType",
            "CreatedAt",
            "ModifiedAt",
            "CreatedBy",
            "ModifiedBy",
            "Deleted",
            "DeletedAt"
        )
        VALUES
        (
            @Id,
            @Action,
            @Description,
            @Actor,
            @ActionType,
            @ActorType,
            @CreatedAt,
            @ModifiedAt,
            @CreatedBy,
            @ModifiedBy,
            @Deleted,
            @DeletedAt
        )
        ON CONFLICT ("Id") DO NOTHING;
        """;

    await using var command =
        new SqlCommand(selectSql, source);

    await using var reader =
        await command.ExecuteReaderAsync();

    var sourceCount = 0;
    var insertedCount = 0;
    var skippedCount = 0;

    while (await reader.ReadAsync())
    {
        sourceCount++;

        await using var insert =
            new NpgsqlCommand(
                insertSql,
                destination,
                transaction);

        insert.Parameters.AddWithValue(
            "Id",
            reader.GetGuid(
                reader.GetOrdinal("Id")));

        insert.Parameters.AddWithValue(
            "Action",
            reader.GetString(
                reader.GetOrdinal("Action")));

        AddNullableParameter(
            insert,
            "Description",
            reader["Description"]);

        insert.Parameters.AddWithValue(
            "Actor",
            reader.GetString(
                reader.GetOrdinal("Actor")));

        insert.Parameters.AddWithValue(
            "ActionType",
            reader.GetInt32(
                reader.GetOrdinal("ActionType")));

        insert.Parameters.AddWithValue(
            "ActorType",
            reader.GetInt32(
                reader.GetOrdinal("ActorType")));

        AddNullableParameter(
            insert,
            "CreatedAt",
            reader["CreatedAt"]);

        AddNullableParameter(
            insert,
            "ModifiedAt",
            reader["ModifiedAt"]);

        AddNullableParameter(
            insert,
            "CreatedBy",
            reader["CreatedBy"]);

        AddNullableParameter(
            insert,
            "ModifiedBy",
            reader["ModifiedBy"]);

        insert.Parameters.AddWithValue(
            "Deleted",
            reader.GetBoolean(
                reader.GetOrdinal("Deleted")));

        AddNullableParameter(
            insert,
            "DeletedAt",
            reader["DeletedAt"]);

        var affected =
            await insert.ExecuteNonQueryAsync();

        if (affected == 1)
        {
            insertedCount++;
        }
        else
        {
            skippedCount++;
        }
    }

    Console.WriteLine($"  AuditLogs found in SQL Server: {sourceCount}");
    Console.WriteLine($"  AuditLogs inserted into Supabase: {insertedCount}");
    Console.WriteLine($"  AuditLogs already existing/skipped: {skippedCount}");
}


// ============================================================
// HELPER
// ============================================================

static void AddNullableParameter(
    NpgsqlCommand command,
    string name,
    object value)
{
    command.Parameters.AddWithValue(
        name,
        value == DBNull.Value
            ? DBNull.Value
            : value);
}