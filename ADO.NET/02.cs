using System.Data.SqlClient;

namespace ADO.NET_Exercises
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connection = "Server=(local)\\SQLEXPRESS;Initial Catalog=MinionsDB;Integrated Security=true;";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            string query = "SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount " +
                "FROM Villains AS v " +
                "JOIN MinionsVillains AS mv ON v.Id = mv.VillainId " +
                "GROUP BY v.Id, v.Name " +
                "HAVING COUNT(mv.VillainId) > 3 " +
                "ORDER BY COUNT(mv.VillainId)";

            using SqlCommand command = new SqlCommand(query, sqlConnection);

            using SqlDataReader sqlDataReader = command.ExecuteReader();

            while (sqlDataReader.Read())
            {
                string name = (string)sqlDataReader["Name"];
                int minionsCount = (int)sqlDataReader["MinionsCount"];

                Console.WriteLine($"{name} - {minionsCount}");
            }
        }
    }
}