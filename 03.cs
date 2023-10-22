using System.Data.SqlClient;

namespace _03_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connection = "Server=(local)\\SQLEXPRESS;Initial Catalog=MinionsDB;Integrated Security=true;";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            string queryForSelectingVilianWithId = "SELECT Name FROM Villains WHERE Id = @Id";

            string query = "SELECT ROW_NUMBER() OVER (ORDER BY m.Name) AS RowNum, m.Name, " +
                "m.Age " +
                "FROM MinionsVillains AS mv " +
                "JOIN Minions As m ON mv.MinionId = m.Id " +
                "WHERE mv.VillainId = @Id " +
                "ORDER BY m.Name";

            using SqlCommand command = new SqlCommand(queryForSelectingVilianWithId, sqlConnection);

            int id = int.Parse(Console.ReadLine());

            command.Parameters.AddWithValue("@Id", id);

            var result = command.ExecuteScalar();

            if (result is null)
            {
                Console.WriteLine($"No villain with ID {id} exists in the database.");
            }
            else
            {
                Console.WriteLine($"Vilian: {result}");

                using SqlCommand command2 = new SqlCommand(query, sqlConnection);

                command2.Parameters.AddWithValue("@Id", id);

                using SqlDataReader reader = command2.ExecuteReader();


                if (reader.Read() == false)
                {
                    Console.WriteLine("(no minions)");
                }
                else
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["RowNum"]}. {reader["Name"]} {reader["Age"]}");
                    }
                }

            }
        }
    }
}