using System;
using Npgsql;

namespace YelpQueryEngine
{
    public class Utils
    {
        public static String currentUser = "";
        public static Business currentBus;

        public static string buildConnectionStr()
        {

            return "Host = localhost; Username = postgres; Database = yelp451; password=123456";

        }

        /// <summary>
        /// Helper for query steps.
        /// </summary>
        /// <param name="sqlstr">sql statement</param>
        /// <param name="myf">delagate function </param>
        public static void executeQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            using (var connection = new NpgsqlConnection(buildConnectionStr()))
            {
                connection.Open();

                // command object for queries.
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;

                    // set sqlstr to cmd attribute
                    cmd.CommandText = sqlstr;

                    // try catch handler when perform sql query.
                    try
                    {
                        // reader object that holds the collection of data from query.
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            // function to do with data
                            myf(reader);
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
         

    }
}
