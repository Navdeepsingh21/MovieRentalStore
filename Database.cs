using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieRentalStore
{
    public class Database : Connection
    {

        // Add to DataGridView
        public static void AddToDataGridView(string table, DataGridView view)
        {
            using (SqlConnection conn = new SqlConnection(GetConnection()))
            {
                // Establish Connection
                conn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + table, conn);
                DataTable dataTable = new DataTable();
                dataTable.Clear();
                adapter.Fill(dataTable);

                view.AutoGenerateColumns = false;
                view.DataSource = dataTable;
            }
        }

        public static bool HasRentedCopies(int MovieID)
        {
            List<string> moviesList = new List<string>();
            using(SqlConnection conn = new SqlConnection(GetConnection()))
            {
                // Open Connection
                conn.Open();

                SqlCommand _cmd = new SqlCommand("SELECT * FROM Rented WHERE MovieID=@id", conn);
                _cmd.Parameters.AddWithValue("@id", MovieID.ToString());
                SqlDataReader reader;
                reader = _cmd.ExecuteReader();
                while(reader.Read())
                {
                    moviesList.Add(reader["RentalID"].ToString());
                }
                reader.Close();

                return moviesList.Count > 0;
            }
        }

        public bool HasCopiesRented(int MovieID)
        {
            return HasRentedCopies(MovieID);
        }

        public static bool HasUserRentedMovie(int customerID)
        {
            List<string> MoviesList = new List<string>();
            using(SqlConnection conn = new SqlConnection(GetConnection()))
            {
                // Open Connection
                conn.Open();

                SqlCommand _cmd = new SqlCommand("Select * FROM Rented WHERE CustomerID=@id", conn);
                _cmd.Parameters.AddWithValue("@id", customerID);
                SqlDataReader reader;
                reader = _cmd.ExecuteReader();
                while(reader.Read())
                {
                    MoviesList.Add(reader["RentalID"].ToString());
                }
                reader.Close();
                return MoviesList.Count > 0;
            }
        }

        public bool UserHasRentedMovies(int CustomerID)
        {
            return HasUserRentedMovie(CustomerID);
        }

    }
}
