using System.Data.SqlClient;

namespace _05_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connection = "Server=(local)\\SQLEXPRESS;Initial Catalog=MinionsDB;Integrated Security=true;";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();



            string updateTownsToUpper = "UPDATE Towns " +
                "SET Name = UPPER(Name) " +
                "WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

            string selectNames = "SELECT t.Name " +
                "FROM Towns as t " +
                "JOIN Countries AS c ON c.Id = t.CountryCode " +
                "WHERE c.Name = @countryName";

            string countryName = Console.ReadLine();

            using SqlCommand command = new SqlCommand(updateTownsToUpper, sqlConnection);

            command.Parameters.AddWithValue("@countryName", countryName);

            command.ExecuteNonQuery();



            int counter = 0;

            using SqlCommand command2 = new SqlCommand(selectNames, sqlConnection);

            command2.Parameters.AddWithValue("@countryName", countryName);

            using SqlDataReader reader = command2.ExecuteReader();

            List<string> townNames = new List<string>();

            while (reader.Read())
            {
                string townName = (string)reader["Name"];

                counter++;
                townNames.Add(townName);
            }

            Console.WriteLine($"{counter} town names were affected.");
            Console.WriteLine($"[{string.Join(", ", townNames)}]");
        }
    }
}