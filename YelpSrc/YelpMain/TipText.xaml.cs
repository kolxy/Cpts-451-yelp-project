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
using System.Windows.Shapes;
using Npgsql;

namespace Milestone1
{
    /// <summary>
    /// Interaction logic for TipText.xaml
    /// </summary>
    public partial class TipText : Window
    {
        private string bid = "";
        private string userid = "---1lKK3aKOuomHnwAkAow";
        private string text;

        public TipText(string bid)
        {
            this.bid = String.Copy(bid);
            InitializeComponent();
            loadTips();
            addColumnsToGrid();
        }

        private void loadTips()
        {
            string sqlstr = "SELECT u.name, t.userid, t.tipdate, t.tiptext,t.likes FROM usertable u, tips t WHERE t.businessid = '" + this.bid + "' AND t.userid=u.userid ORDER BY name;";
            Console.WriteLine(sqlstr);
            executeQuery(sqlstr, addGridRow);
        }
        public class Tips
        {
            public string username { get; set; }
            public string userid { get; set; }
            public string tipdate { get; set; }
            public string tiptext { get; set; }
            public string likes { get; set; }
        }
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            //string sqlStrUser = "CREATE OR REPLACE FUNCTION UpdateNumTips() RETURNS trigger AS '" + "Begin update business set numtips = (select count(*) from business b,tips t where b.businessid=t.businessid AND b.businessid=" + this.bid+");" + " retur null;"+" END"+"' LANGUAGE plpgsql;"+" CREATE TRIGGER TriNumTips AFTER INSERT ON Tips FRO EACH STATEMENT EXECUTE PROCEDURE UpdateNumTips();";
            string sqlStrBussines = "UPDATE business SET numtips = numtips + 1 WHERE businessid='" + this.bid+"';";
            executeQuery(sqlStrBussines, null);
            string sqlStrUser = "UPDATE usertable SET tipcount = tipcount + 1 WHERE userid = '" + this.userid + "'; ";
            this.Close();
        }

        private void addGridRow(NpgsqlDataReader R)
        {

            textGird.Items.Add(new Tips() { username = R.GetString(0), userid = R.GetString(1), tipdate = R.GetDateTime(2).ToString(), tiptext = R.GetString(3), likes = R.GetInt16(4).ToString() });
        }

        private void addColumnsToGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("username");
            col1.Header = "Name";
            col1.Width = 100;
            textGird.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("userid");
            col2.Header = "UserID";
            col2.Width = 100;
            textGird.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("tipdate");
            col3.Header = "Date";
            col3.Width = 100;
            textGird.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("tiptext");
            col4.Header = "Text";
            col4.Width = 100;
            textGird.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("likes");
            col5.Header = "Likes";
            col5.Width = 100;
            textGird.Columns.Add(col5);
        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.text = textbox.Text;
        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = Milestone2; password=123456";
        }

        private void executeQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        if (myf != null)
                        {
                            while (reader.Read())
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
