using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieRentalStore
{
    public partial class Form1 : Form
    {
        private int CustomerRowIndex, MovieRowIndex, RentalRowIndex;
        private int MovieID, customerID, RentalRowID;

        public Form1()
        {
            InitializeComponent();
        }

        private void AddMovie_Click(object sender, EventArgs e)
        {
            if(movieTitleText.Text != "" || rentingCostValue.Text != "" )
            {
                // Checks if user is trying to add new user or modify the existing one
                if(addMovieSelected.Checked)
                {
                    object[] inputFields = { movieTitleText.Text, movieReleaseDateValue.Value, movieRatingsValue.Value, movieCopiesValue.Value, rentingCostValue.Text, comboBox1.SelectedItem.ToString() };
                    string[] fieldNames = { "@title", "@date", "@ratings", "@copies", "@rentingCost", "@genre" };

                    using (SqlConnection conn = new SqlConnection(Connection.GetConnection()))
                    {
                        // Open Connection
                        conn.Open();

                        SqlCommand _cmd;
                        _cmd = new SqlCommand("INSERT INTO Movies (MovieTitle, MovieReleaseDate, MovieRatings, MovieCopies, MovieRentingCost, MovieGenre) VALUES(@title, @date, @ratings, @copies, @rentingCost, @genre)", conn);

                        for (int i = 0; i < fieldNames.Length; i++)
                        {
                            _cmd.Parameters.AddWithValue(fieldNames[i], inputFields[i]);
                        }

                        _cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string[] inputs = { "@title", "@date", "@ratings", "@copies", "@cost", "@genre", "@midd" };
                    object[] fieldValues = { movieTitleText.Text, movieReleaseDateValue.Value, movieRatingsValue.Value, movieCopiesValue.Value, rentingCostValue.Text, comboBox1.SelectedItem.ToString(), MovieID.ToString() };

                    using (SqlConnection conn = new SqlConnection(Connection.GetConnection()))
                    {
                        // Open Connection
                        conn.Open();

                        SqlCommand _cmd;
                        _cmd = new SqlCommand("UPDATE Movies SET MovieTitle=@title, MovieReleaseDate=@date, MovieRatings=@ratings, MovieCopies=@copies, MovieRentingCost=@cost, MovieGenre=@genre WHERE MovieID=@midd", conn);

                        for (int i = 0; i < inputs.Length; i++)
                        {
                            _cmd.Parameters.AddWithValue(inputs[i], fieldValues[i]);
                        }

                        _cmd.ExecuteNonQuery();
                    }
                }

                ResetMovieFields();

                Database.AddToDataGridView("Movies", MoviesList);
            }
            else
            {
                MessageBox.Show("One or More fields are Empty!");
            }
        }

        private void DeleteMovie_Click(object sender, EventArgs e)
        {
            // Checks if user has rented any movies before User can be deleted
            if(Database.HasRentedCopies(MovieID))
            {
                Functions.DisplayMessage("One of more copies of this movie are rented!");
            }
            else
            {
                // Delete Customer from list
                using (SqlConnection conn = new SqlConnection(Database.GetConnection()))
                {
                    // Open Connection
                    conn.Open();

                    string query = "DELETE FROM Movies WHERE MovieID=@midd";
                    SqlCommand _cmd = new SqlCommand(query, conn);
                    _cmd.Parameters.AddWithValue("@midd", MovieID.ToString());
                    _cmd.ExecuteNonQuery();
                }
                Database.AddToDataGridView("Movies", MoviesList);
                ResetMovieFields();
            }
        }

        // When the form is loaded
        private void Form1_Load(object sender, EventArgs e)
        {
            // Setting default selection value
            addMovieSelected.Checked = true;
            modifyMovieSelected.Checked = false;

            // Add Movie Genre's to ComboBox
            Functions.AddMoviesGenre(comboBox1);

            // Add Movies to GridView
            Database.AddToDataGridView("Movies", MoviesList);

            // Add Customers to GridView
            Database.AddToDataGridView("Customers", CustomersList);

            // Add Rented Movies to GridView
            Database.AddToDataGridView("Rented", RentalsList);

            TimeSpan spanTime = DateTime.Now.Subtract(movieReleaseDateValue.Value);
            int totalDays = Convert.ToInt32(spanTime.TotalDays);
            float totalYears = totalDays / 364.5f;
            if(totalYears >= 5.0f)
            {
;                rentingCostValue.Text = "2.0";
            }
            else
            {
                rentingCostValue.Text = "5.0";
            }

            // Set Default Dates
            rentingDate.Value = DateTime.Now;
            returningDate.Value = DateTime.Now.AddDays(1);
        }

        private void Add_ModifyCustomerBtn_Click(object sender, EventArgs e)
        {
            if(firstNameField.Text != "" || lastNameField.Text != "" || addressField.Text != "" || phoneNumberField.Text != "")
            {
                if (addNewCustomerRadio.Checked)
                {
                    object[] inputFields = { firstNameField.Text, lastNameField.Text, addressField.Text, phoneNumberField.Text };
                    string[] fieldNames = { "@first", "@last", "@address", "@phone" };

                    using (SqlConnection conn = new SqlConnection(Connection.GetConnection()))
                    {
                        // Open Connection
                        conn.Open();

                        SqlCommand _cmd;
                        _cmd = new SqlCommand("INSERT INTO Customers (FirstName, LastName, Address, PhoneNumber) VALUES(@first, @last, @address, @phone)", conn);

                        for (int i = 0; i < fieldNames.Length; i++)
                        {
                            _cmd.Parameters.AddWithValue(fieldNames[i], inputFields[i]);
                        }

                        _cmd.ExecuteNonQuery();
                    }
                    Database.AddToDataGridView("Customers", CustomersList);
                }
                else
                {
                    object[] inputFields = { firstNameField.Text, lastNameField.Text, addressField.Text, phoneNumberField.Text, customerID };
                    string[] fieldNames = { "@first", "@last", "@address", "@phone", "@cust" };

                    using (SqlConnection conn = new SqlConnection(Connection.GetConnection()))
                    {
                        // Open Connection
                        conn.Open();

                        SqlCommand _cmd;
                        _cmd = new SqlCommand("UPDATE Customers SET FirstName=@first, LastName=@last, Address=@address, PhoneNumber=@phone WHERE CustomerID=@cust", conn);

                        for (int i = 0; i < fieldNames.Length; i++)
                        {
                            _cmd.Parameters.AddWithValue(fieldNames[i], inputFields[i]);
                        }

                        _cmd.ExecuteNonQuery();
                    }
                    Database.AddToDataGridView("Customers", CustomersList);
                }

                ResetCustomerFields();

            }
            else
            {
                MessageBox.Show("One or More fields are Empty!");
            }
        }

        private void DeleteCustomerBtn_Click(object sender, EventArgs e)
        {
            if(Database.HasUserRentedMovie(customerID))
            {
                Functions.DisplayMessage("This Customer has Rented Movies!");
            }
            else
            {
                // Delete Customer from list
                using (SqlConnection conn = new SqlConnection(Database.GetConnection()))
                {
                    // Open Connection
                    conn.Open();

                    string query = "DELETE FROM Customers WHERE CustomerID=@cust";
                    SqlCommand _cmd = new SqlCommand(query, conn);
                    _cmd.Parameters.AddWithValue("@cust", customerID);
                    _cmd.ExecuteNonQuery();
                }
                Database.AddToDataGridView("Customers", CustomersList);

                // Makes sure to clear the fields once the user is deleted.
                ResetCustomerFields();
            }
        }

        private void AddNewCustomerRadio_CheckedChanged(object sender, EventArgs e)
        {
            add_ModifyCustomerBtn.Text = "Add Customer";
        }

        private void ModifyCustomerRadio_CheckedChanged(object sender, EventArgs e)
        {
            add_ModifyCustomerBtn.Text = "Modify Customer";
        }

        private void AddMovieSelected_CheckedChanged(object sender, EventArgs e)
        {
            addMovie.Text = "Add Movie";
        }

        private void ModifyMovieSelected_CheckedChanged(object sender, EventArgs e)
        {
            addMovie.Text = "Modify Movie";
        }

        private void CustomersList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.CustomerRowIndex = e.RowIndex;

            // To avoid crashing if incorrect index is selected we run this condition
            if(this.CustomerRowIndex < 0)
            {
                Console.WriteLine("Invalid Index Selected!");
            }
            else
            {
                DataGridViewRow row = CustomersList.Rows[this.CustomerRowIndex];

                // Check's whether row is selected or not
                if(row.Selected)
                {
                    // Check's if the user didn't select the auto generated row
                    if (this.CustomerRowIndex < CustomersList.Rows.Count - 1)
                    {
                        // Enable delete button so user has the functionality to delete
                        deleteCustomerBtn.Enabled = true;

                        customerID = Convert.ToInt32(row.Cells[0].Value);
                        firstNameField.Text = row.Cells[1].Value.ToString();
                        lastNameField.Text = row.Cells[2].Value.ToString();
                        addressField.Text = row.Cells[3].Value.ToString();
                        phoneNumberField.Text = row.Cells[4].Value.ToString();
                    }
                    else
                    {
                        deleteCustomerBtn.Enabled = false;
                        Console.WriteLine("Null operator doesn't exist!");

                        // Clear all fields
                        ResetCustomerFields();
                    }
                }
                else
                {
                    // Clear all fields
                    ResetCustomerFields();
                }
            }
        }

        private void ResetMovieFields()
        {
            movieTitleText.Text = "";
            movieRatingsValue.Value = 0.5m;
            rentingCostValue.Text = "";
            comboBox1.SelectedIndex = 0;
            movieCopiesValue.Value = 0;
            MovieID = 0;

            deleteMovie.Enabled = false;
        }

        private void RentingDate_ValueChanged(object sender, EventArgs e)
        {
            if(rentingDate.Value.Date >= returningDate.Value.Date)
            {
                returningDate.Value = rentingDate.Value.AddDays(1);
            }
        }

        private void ReturningDate_ValueChanged(object sender, EventArgs e)
        {
            if (returningDate.Value.Date <= rentingDate.Value.Date)
            {
                returningDate.Value = rentingDate.Value.AddDays(1);
            }
            else
            {
                // Do nothing
            }
        }

        private void Rent_Click(object sender, EventArgs e)
        {
            if(customerID > 0 && MovieID > 0)
            {
                using (SqlConnection conn = new SqlConnection(Connection.GetConnection()))
                {
                    // Open Connection
                    conn.Open();

                    // Only issues movies if the copies of movie are available
                    if(movieCopiesValue.Value > 0)
                    {
                        using(SqlCommand _cmd = new SqlCommand("UPDATE Movies SET MovieCopies=@copies WHERE MovieID=@id", conn))
                        {
                            _cmd.Parameters.AddWithValue("@copies", Convert.ToInt32(movieCopiesValue.Value - 1));
                            _cmd.Parameters.AddWithValue("@id", MovieID);
                            _cmd.ExecuteNonQuery();
                        }

                        // Add Movie to Rented Movies table
                        using(SqlCommand _cmd = new SqlCommand("INSERT INTO Rented (CustomerID, MovieID, RentFrom, RentTill) VALUES(@cid, @mid, @from, @till)", conn))
                        {
                            _cmd.Parameters.AddWithValue("@cid", customerID);
                            _cmd.Parameters.AddWithValue("@mid", MovieID);
                            _cmd.Parameters.AddWithValue("@from", rentingDate.Value);
                            _cmd.Parameters.AddWithValue("@till", returningDate.Value);
                            _cmd.ExecuteNonQuery();
                        }

                        // Calculate renting period in days
                        TimeSpan time = returningDate.Value.Subtract(rentingDate.Value);

                        // Display as pop-up message
                        Functions.DisplayMessage(firstNameField.Text + " has rented " + movieTitleText.Text + " for " + Convert.ToInt32(time.TotalDays) + " days!");

                        // Update all the tables
                        Database.AddToDataGridView("Movies", MoviesList);
                        Database.AddToDataGridView("Customers", CustomersList);
                        Database.AddToDataGridView("Rented", RentalsList);
                    }
                    else
                    {
                        Functions.DisplayMessage("All copies of " + movieTitleText.Text + " are rented!");
                    }
                }
            }
            else
            {
                if(customerID <= 0)
                {
                    Functions.DisplayMessage("Please Select the customer!");
                }
                else
                {
                    Functions.DisplayMessage("Please Select the Movie!");
                }
            }
        }

        private void RentalsList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.RentalRowIndex = e.RowIndex;

            // To avoid crashing if incorrect index is selected we run this condition
            if (this.RentalRowIndex < 0)
            {
                Console.WriteLine("Invalid Index Selected!");
            }
            else
            {
                DataGridViewRow row = RentalsList.Rows[this.RentalRowIndex];

                // Check's whether row is selected or not
                if (row.Selected)
                {
                    // Check's if the user didn't select the auto generated row
                    if (this.RentalRowIndex < RentalsList.Rows.Count - 1)
                    {
                        // Enable delete button so user has the functionality to delete
                        deleteCustomerBtn.Enabled = true;

                        RentalRowID = Convert.ToInt32(row.Cells[0].Value);
                        customerID = Convert.ToInt32(row.Cells[2].Value);
                        rentingDate.Value = Convert.ToDateTime(row.Cells[3].Value);
                        returningDate.Value = Convert.ToDateTime(row.Cells[4].Value);
                    }
                    else
                    {
                        deleteCustomerBtn.Enabled = false;
                        Console.WriteLine("Null operator doesn't exist!");

                        // Clear all fields
                        ResetCustomerFields();
                    }
                }
                else
                {
                    // Clear all fields
                    ResetCustomerFields();
                }
            }
        }

        private void MovieReleaseDateValue_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan spanTime = DateTime.Now.Subtract(movieReleaseDateValue.Value);
            int totalDays = Convert.ToInt32(spanTime.TotalDays);
            float totalYears = totalDays / 364.5f;
            if (totalYears >= 5.0f)
            {
                ; rentingCostValue.Text = "2.0";
            }
            else
            {
                rentingCostValue.Text = "5.0";
            }
        }

        private void Return_Click(object sender, EventArgs e)
        {
            // Delete Customer from list
            using (SqlConnection conn = new SqlConnection(Database.GetConnection()))
            {
                // Open Connection
                conn.Open();

                string query = "DELETE FROM Rented WHERE RentalID=@ridd";
                SqlCommand _cmd = new SqlCommand(query, conn);
                _cmd.Parameters.AddWithValue("@ridd", RentalRowID);
                _cmd.ExecuteNonQuery();
            }

            Database.AddToDataGridView("Rented", RentalsList);

            using (SqlConnection conn = new SqlConnection(Connection.GetConnection()))
            {
                // Open Connection
                conn.Open();

                // Execute Query
                using(SqlCommand _cmd = new SqlCommand("UPDATE Movies SET MovieCopies=@copies WHERE MovieID=@id", conn))
                {
                    int copies = 0;
                    SqlCommand _getcopies = new SqlCommand("SELECT MovieCopies FROM Movies WHERE MovieID=@mid", conn);
                    _getcopies.Parameters.AddWithValue("@mid", MovieID);
                    SqlDataReader reader = _getcopies.ExecuteReader();
                    while(reader.Read())
                    {
                        copies = Convert.ToInt32(reader["MovieCopies"]);
                    }
                    reader.Close();

                    _cmd.Parameters.AddWithValue("@copies", copies + 1);
                    _cmd.Parameters.AddWithValue("@id", MovieID);
                    _cmd.ExecuteNonQuery();
                }

                // Update Tables to keep the table data in sync
                Database.AddToDataGridView("Movies", MoviesList);
            }

            using (SqlConnection connect = new SqlConnection(Connection.GetConnection()))
            {
                connect.Open();

                string rentingPrice = "";
                SqlCommand _getMovie = new SqlCommand("SELECT MovieRentingCost FROM Movies WHERE MovieID=@movID", connect);
                _getMovie.Parameters.AddWithValue("@movID", MovieID);

                SqlDataReader movieReader = _getMovie.ExecuteReader();
                while(movieReader.Read())
                {
                    rentingPrice = Convert.ToString(movieReader["MovieRentingCost"]);
                }
                movieReader.Close();

                _getMovie.ExecuteNonQuery();

                TimeSpan timer = returningDate.Value.Subtract(rentingDate.Value);
                Double rentalCost = Convert.ToInt32(timer.TotalDays) * Convert.ToDouble(rentingPrice);
                MessageBox.Show(firstNameField.Text + " has Returned copy of " + movieTitleText.Text + " and is charged: $" + rentalCost);
            }
        }

        private void ResetCustomerFields()
        {
            firstNameField.Text = "";
            lastNameField.Text = "";
            addressField.Text = "";
            phoneNumberField.Text = "";
            customerID = 0;
            deleteCustomerBtn.Enabled = false;
        }

        private void ResetRentalsFields()
        {
            rentingDate.Value = DateTime.Now;
            returningDate.Value = DateTime.Now.AddDays(1);
            RentalRowID = 0;
        }

        private void MoviesList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.MovieRowIndex = e.RowIndex;

            // To avoid crashing if incorrect index is selected we run this condition
            if (this.MovieRowIndex < 0)
            {
                Console.WriteLine("Invalid Index Selected!");
                ResetMovieFields();
            }
            else
            {
                DataGridViewRow row = MoviesList.Rows[this.MovieRowIndex];

                // Check's whether row is selected or not
                if (row.Selected)
                {
                    // Check's if the user didn't select the auto generated row
                    if (this.MovieRowIndex < MoviesList.Rows.Count - 1)
                    {
                        // Enable delete button so user has the functionality to delete
                        deleteMovie.Enabled = true;

                        // Assign values to all the form fields
                        MovieID = Convert.ToInt32(row.Cells[0].Value);
                        movieTitleText.Text = row.Cells[1].Value.ToString();
                        movieReleaseDateValue.Value = Convert.ToDateTime(row.Cells[2].Value);
                        movieRatingsValue.Value = Convert.ToDecimal(row.Cells[3].Value);
                        movieCopiesValue.Value = Convert.ToInt32(row.Cells[4].Value);
                        rentingCostValue.Text = row.Cells[5].Value.ToString();
                        if (comboBox1.Items.Contains(row.Cells[6].Value.ToString()))
                        {
                            comboBox1.SelectedItem = row.Cells[6].Value.ToString();
                        }
                        else
                        {
                            comboBox1.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        deleteMovie.Enabled = false;
                        Console.WriteLine("Null operator doesn't exist!");
                        ResetMovieFields();
                    }
                }
                else
                {
                    ResetMovieFields();
                }
            }
        }
    }
}
