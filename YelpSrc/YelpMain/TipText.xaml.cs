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
using YelpQueryEngine;
using Npgsql;

namespace YelpMain
{
    /// <summary>
    /// Interaction logic for TipText.xaml
    /// </summary>
    public partial class TipText : Window
    {
        private string bid = "";
        private string text;

        public TipText(string bid)
        {
            this.bid = String.Copy(bid);
            InitializeComponent();
            loadTips();
            addColumnsToGrid();
            addColumnsToGrid2();
            loadFriendReview();
        }

        private void loadTips()
        {
            this.textGird.Items.Clear();
            string sqlstr = $"SELECT u.name, t.user_id, t.timestamp, t.text,t.likes FROM the_user u, tips t WHERE t.business_id = '{this.bid}' AND t.user_id=u.user_id ORDER BY timestamp desc;";
            Console.WriteLine(sqlstr);
            Utils.executeQuery(sqlstr, addGridRow);
        }
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.currentUser.Length <= 0)
            {
                MessageBox.Show("In order to add tip pls first login!");
                return;
            }
            string sqlstr = $@"INSERT INTO tips (business_id, user_id, timestamp, likes, text) values ('{this.bid}', '{Utils.currentUser}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', 0, '{textbox.Text.ToString().Replace("'", "''")}')";
            Console.WriteLine(sqlstr);
            textbox.Text = "";
            Utils.executeQuery(sqlstr, null);
            textGird.Items.Clear();
            loadTips();
        }

        private void addGridRow(NpgsqlDataReader R)
        {

            textGird.Items.Add(new Tips() { 
                username = R.GetString(0), 
                user_id = R.GetString(1), 
                timestamp = R.GetDateTime(2).ToString(), 
                text = R.GetString(3), 
                likes = R.GetInt16(4).ToString() 
            });
        }

        private void addColumnsToGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("username");
            col1.Header = "Name";
            col1.Width = 100;
            textGird.Columns.Add(col1);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("timestamp");
            col3.Header = "Date";
            col3.Width = 100;
            textGird.Columns.Add(col3);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("likes");
            col5.Header = "Likes";
            col5.Width = 30;
            textGird.Columns.Add(col5);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("text");
            col4.Header = "Text";
            col4.Width = 170;
            textGird.Columns.Add(col4);

           

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("user_id");
            col2.Header = "UserID";
            col2.Width = 0;
            textGird.Columns.Add(col2);
        }

        private void addColumnsToGrid2()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("username");
            col1.Header = "Name";
            col1.Width = 80;
            friendTipList.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("timestamp");
            col2.Header = "Date";
            col2.Width = 150;
            friendTipList.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("text");
            col3.Header = "Text";
            col3.Width = 170;
            friendTipList.Columns.Add(col3);

        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.text = textbox.Text;
        }

        private void loadFriendReview()
        {
            if (Utils.currentUser.Length <= 0)
            {
                return;
            }
            string sql = $"select name, timestamp, text from the_user, (select user_id, timestamp, text from tips where business_id = '{this.bid}' and user_id in (select friend_id from user_follow where user_id ='{Utils.currentUser}')) as friend where the_user.user_id = friend.user_id";
            Utils.executeQuery(sql, updateFriendReview);
        }
        
        private void updateFriendReview(NpgsqlDataReader R)
        {
            friendTipList.Items.Add(new Tips()
            {
                username = R.GetString(0),
                timestamp = R.GetDateTime(1).ToString(),
                text = R.GetString(2),
            });
            
        }

        private void likeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (textGird.SelectedIndex < 0)
            {
                MessageBox.Show("In order to like a tip please first select a tip!");
                return;
            }

            if (Utils.currentUser.Length <= 0)
            {
                MessageBox.Show("In order to like a tip pls first login!");
                return;
            }
            Tips selectTip = textGird.SelectedItem as Tips;
            string sql = $"update tips set likes = likes + 1 where business_id = '{this.bid}' and user_id = '{selectTip.user_id}' and timestamp = '{selectTip.timestamp}'";
            Utils.executeQuery(sql, null);
            loadTips();
        }
    }
}
