using System.Data.SqlClient;

namespace _07_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connection = "Server=(local)\\SQLEXPRESS;Initial Catalog=MinionsDB;Integrated Security=true;";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            string query = "SELECT Name FROM Minions ";

            using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            using SqlDataReader reader = sqlCommand.ExecuteReader();

            List<string> names = new List<string>();


            while (reader.Read())
            {
                names.Add((string)reader["Name"]);
            }

            for (int i = 0; i < names.Count / 2; i++)
            {
                if (i + 1 == names.Count / 2 && (i + 1) % 2 != 0)
                {
                    Console.WriteLine(names[i]);
                    Console.WriteLine(names[names.Count - i - 1]);
                    Console.WriteLine(names[i + 1]);
                }
                else
                {
                    Console.WriteLine(names[i]);
                    Console.WriteLine(names[names.Count - i - 1]);
                }
            }
        }
    }
}