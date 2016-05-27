using System;
using System.Data;
using System.Data.SqlClient;


namespace BandExplorer
{
    class DatabaseConnection
    {
        private SqlConnection con;
        private SqlDataAdapter adapter;

        public DatabaseConnection(String connString)
        {
            con = new SqlConnection(connString);
            String selectCommand = "SELECT * FROM TABLE";
            String insertCommand = "INSERT INTO Customers (CustomerID, CompanyName) VALUES (@CustomerID, @CompanyName)";
            String updateCommand = "UPDATE Customers SET CustomerID = @CustomerID, CompanyName = @CompanyName WHERE CustomerID = @oldCustomerID";
            String deleteCommand = "DELETE FROM Customers WHERE CustomerID = @CustomerID";

            adapter = new SqlDataAdapter(new SqlCommand(selectCommand, con));

            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            // Create the other commands.
            adapter.InsertCommand = new SqlCommand(insertCommand, con);
            adapter.UpdateCommand = new SqlCommand(updateCommand, con);
            adapter.DeleteCommand = new SqlCommand(deleteCommand, con);

            // Create the parameters.
            adapter.InsertCommand.Parameters.Add("@CustomerID", SqlDbType.Char, 5, "CustomerID");
            adapter.InsertCommand.Parameters.Add("@CompanyName", SqlDbType.VarChar, 40, "CompanyName");
            adapter.UpdateCommand.Parameters.Add("@CustomerID", SqlDbType.Char, 5, "CustomerID");
            adapter.UpdateCommand.Parameters.Add("@CompanyName", SqlDbType.VarChar, 40, "CompanyName");
            adapter.UpdateCommand.Parameters.Add("@oldCustomerID", SqlDbType.Char, 5, "CustomerID").SourceVersion = DataRowVersion.Original;
            adapter.DeleteCommand.Parameters.Add("@CustomerID", SqlDbType.Char, 5, "CustomerID").SourceVersion = DataRowVersion.Original;
        }

        /// <summary>
        /// Function to get a data set. It will use the class variables Sql & connection_string
        /// </summary>
        public DataSet GetDataSet(String sqlString)
        {
            con.Open();
            adapter = new SqlDataAdapter(sqlString, con);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "ResultSet");
            con.Close();
            return ds;
        }

        public void UpdateDatabase(DataSet dataSet)
        {
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
            commandBuilder.DataAdapter.Update(dataSet.Tables[0]);
        }

        public SqlDataReader GetDataReader(String sql)
        {
            SqlDataReader rdr = null;

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0]);
                }
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }

                if (con != null)
                {
                    con.Close();
                }
            }
            return null;
        }
    }
}
