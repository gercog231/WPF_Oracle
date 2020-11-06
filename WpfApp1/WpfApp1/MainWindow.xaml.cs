using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OracleConnection con = null;
        public MainWindow()
        {
            this.setConnection();
            InitializeComponent();
        }

        private void updateDataGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ID, QUALTITY, QUANTITY FROM PRODUCT";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            myDataGrid.ItemsSource = dt.DefaultView;
            dr.Close();
        }
        private void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            con = new OracleConnection(connectionString);
            try {
                con.Open();
            }
            catch (Exception exp) { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.updateDataGrid();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            String sql = "INSERT INTO PRODUCT(ID, QUALTITY, QUANTITY)" +
                "VALUES(:ID, :QUALTITY, :QUANTITY)";
            this.AUD(sql, 0);
            add_btn.IsEnabled = false;
            update_btn.IsEnabled = true;
            del_btn.IsEnabled = true;
        }

        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            String sql = "UPDATE PRODUCT SET QUALTITY = :QUALTITY, QUANTITY = :QUANTITY WHERE ID = :ID";
            this.AUD(sql, 1);
        }

        private void del_btn_Click(object sender, RoutedEventArgs e)
        {
            String sql = "DELETE FROM PRODUCT " +
                "WHERE ID = :ID";
            this.AUD(sql, 2);
            this.resetAll();
        }
        private void resetAll()
        {
            id_txtbx.Text = "";
            qualtity_txtbx.Text = "";
            quantity_txtbx.Text = "";

            add_btn.IsEnabled = true;
            del_btn.IsEnabled = false;
            update_btn.IsEnabled = false;
        }
        private void reset_btn_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();
        }
        private void AUD(String sql_stmt, int state)
        {
            String msg = "";
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;

            switch (state)
            {
                case 0:
                    msg = "Row Inserted Successfully!";
                    cmd.Parameters.Add("ID", OracleDbType.Int32, 6).Value = Int32.Parse(id_txtbx.Text);
                    cmd.Parameters.Add("QUALTITY", OracleDbType.Varchar2, 5).Value = qualtity_txtbx.Text;
                    cmd.Parameters.Add("QUANTITY", OracleDbType.Int32, 6).Value = Int32.Parse(quantity_txtbx.Text);
                    break;
                case 1:
                    msg = "Row Updated Successfully!";
                    
                    cmd.Parameters.Add("QUALTITY", OracleDbType.Varchar2, 5).Value = qualtity_txtbx.Text;
                    cmd.Parameters.Add("QUANTITY", OracleDbType.Int32, 6).Value = Int32.Parse(quantity_txtbx.Text);

                    cmd.Parameters.Add("ID", OracleDbType.Int32, 6).Value = Int32.Parse(id_txtbx.Text);
                    break;
                case 2:
                    msg = "Row Deleted Successfully!";
                    cmd.Parameters.Add("ID", OracleDbType.Int32, 6).Value = Int32.Parse(id_txtbx.Text);
                    break;
            }
            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                    this.updateDataGrid();
                }
            }
            catch(Exception expe)
            {

            }
        }

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                id_txtbx.Text = dr["ID"].ToString();
                qualtity_txtbx.Text = dr["QUALTITY"].ToString();
                quantity_txtbx.Text = dr["QUANTITY"].ToString();

                add_btn.IsEnabled = false;
                update_btn.IsEnabled = true;
                del_btn.IsEnabled = true;
            }
        }
    }
}
