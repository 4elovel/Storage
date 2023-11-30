
using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Data;
namespace Storage;

internal class Program
{
    private static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

    static async Task Main(string[] args)
    {
        using var connection = new SqliteConnection(ConnectionString);

        try
        {
            await connection.OpenAsync();
            Console.WriteLine("Сonnection is successful\n");


            string code = "SELECT * FROM Storage";
            using (var command = new SqliteCommand(code, connection))
            {
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["ProductId"]} | {reader["ProductName"]} | {reader["ProductType"]} | {reader["ProviderId"]} | {reader["Count"]} | {reader["Cost"]} | {reader["SupplyDate"]}");
                    }
                }
            }
            Console.WriteLine("\n\nProductTypes:");
            code = "SELECT * FROM ProductTypes";
            using (var command = new SqliteCommand(code, connection))
            {
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["TypeId"]} | {reader["TypeName"]}");
                    }
                }
            }

            Console.WriteLine("\n\nProviders:");
            code = "SELECT * FROM Providers";
            using (var command = new SqliteCommand(code, connection))
            {
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["ProviderId"]} | {reader["ProviderName"]}");
                    }
                }
            }
            Console.WriteLine("\n");
            code = "SELECT MAX(Count) AS _Count FROM Storage";
            using (var command = new SqliteCommand(code, connection))
            {
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    Console.WriteLine($" \n{result}");
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
            }

            Console.WriteLine("\n");
            code = "SELECT MIN(Count) AS _Count FROM Storage";
            using (var command = new SqliteCommand(code, connection))
            {
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    Console.WriteLine($"\n {result}");
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
            }

            Console.WriteLine("\n");
            code = "SELECT MIN(Cost) AS _Cost FROM Storage";
            using (var command = new SqliteCommand(code, connection))
            {
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    Console.WriteLine($"\n {result}");
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
            }

            Console.WriteLine("\n");
            code = "SELECT MAX(Cost) AS _Cost FROM Storage";
            using (var command = new SqliteCommand(code, connection))
            {
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    Console.WriteLine($"\n {result}\n\n");
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
            }


            code = "SELECT Storage.*, ProductTypes.*\r\nFROM Storage\r\nJOIN ProductTypes ON Storage.ProductType = ProductTypes.TypeId\r\nWHERE Storage.ProviderId = (\r\n    SELECT ProviderId FROM Providers WHERE ProviderName = '@Supp'\r\n);\r\n";
            using (var command = new SqliteCommand(code, connection))
            {
                command.Parameters.AddWithValue("@Supp", "SupplierA");
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["ProductId"]} | {reader["ProductName"]} | {reader["ProductType"]} | {reader["ProviderId"]} | {reader["Count"]} | {reader["Cost"]} | {reader["SupplyDate"]}\n\n");
                    }
                }
            }



            code = "SELECT Storage.*, ProductTypes.*\r\nFROM Storage\r\nJOIN ProductTypes ON Storage.ProductType = ProductTypes.TypeId\r\nWHERE ProductTypes.TypeName = @Cat;\r\n";

            using (var command = new SqliteCommand(code, connection))
            {
                command.Parameters.AddWithValue("@Cat", "Category1");


                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["ProductId"]} | {reader["ProductName"]} | {reader["ProductType"]} | {reader["ProviderId"]} | {reader["Count"]} | {reader["Cost"]} | {reader["SupplyDate"]}");
                    }
                }
            }


            code = "SELECT Storage.*, ProductTypes.*\r\nFROM Storage\r\nJOIN ProductTypes ON Storage.ProductType = ProductTypes.TypeId\r\nORDER BY Storage.SupplyDate ASC\r\nLIMIT 1;\r\n";
            using (var command = new SqliteCommand(code, connection))
            {
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["ProductId"]} | {reader["ProductName"]} | {reader["ProductType"]} | {reader["ProviderId"]} | {reader["Count"]} | {reader["Cost"]} | {reader["SupplyDate"]}");
                    }
                }
            }


            code = "SELECT ProductTypes.TypeName, AVG(Storage.Count) AS avr \r\nFROM Storage\r\nJOIN ProductTypes ON Storage.ProductType = ProductTypes.TypeId\r\nGROUP BY ProductTypes.TypeName;\r\n";
            using (var command = new SqliteCommand(code, connection))
            {
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["TypeName"]} | {reader["avr"]}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await connection.CloseAsync();
            Console.WriteLine("\nConnection is failed");
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
                Console.WriteLine("\nConnection is disconnected");
            }
        }
    }
}