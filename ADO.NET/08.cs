using System.Data.SqlClient;

namespace _08_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connection = "Server=(local)\\SQLEXPRESS;Initial Catalog=MinionsDB;Integrated Security=true;";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            List<int> ids = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            string updateQuery = "UPDATE Minions\r\n   SET Name = LOWER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1\r\n WHERE Id = @Id";

            for (int i = 0; i < ids.Count; i++)
            {
                using SqlCommand command = new SqlCommand(updateQuery, sqlConnection);
                command.Parameters.AddWithValue("@Id", ids[i]);
                command.ExecuteNonQuery();
            }

            string selectQuery = "SELECT Name, Age FROM Minions";

            using SqlCommand command2 = new SqlCommand(selectQuery, sqlConnection);

            using SqlDataReader reader = command2.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
            }
        }
    }
}