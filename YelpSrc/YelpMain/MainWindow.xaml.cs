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
using YelpQueryEngine;
using Npgsql;

namespace YelpMain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            initState();
            initBusinessInfoTableHeader();
            initUserFriendTableHeader();
            initFriendsLatestTipTableHeader();
            initUserId();
        }

        /// <summary>
        /// Init the states available in db.
        /// </summary>
        private void initState()
        {
            string sqlstr = "SELECT distinct state FROM business ORDER BY state";
            Utils.executeQuery(sqlstr, addStateUI);
        }

        private void addStateUI(NpgsqlDataReader reader)
        {
            stateList.Items.Add(reader.GetString(0));
        }


        private void initUserId()
        {
            string sqlstr = "select distinct user_id from the_user";
            Utils.executeQuery(sqlstr, addUserId);
        }

        private void addUserId(NpgsqlDataReader reader)
        {
            userList.Items.Add(reader.GetString(0));
        }


        /// <summary>
        /// Init the header row for [businessInfoTable]
        /// </summary>
        private void initBusinessInfoTableHeader()
        {
            Dictionary<string, string> colNameBinding = Constants.getBusinessInfoTableHeaderNameAndBinder();
            Dictionary<string, int> colNameWidth = Constants.getBusinessInfoTableHeaderColWidth();
            foreach (var item in colNameBinding)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = item.Key;
                col.Binding = new Binding(item.Value);
                col.Width = colNameWidth[item.Key];
                businessInfoTable.Columns.Add(col);
            }
        }

        // userfriendstable header setup and binding with object.
        private void initUserFriendTableHeader()
        {

            Dictionary<string, string> colNameBinding = Constants.getUserFriendTableBinder();
            Dictionary<string, int> colNameWidth = Constants.getUserFriendTableHeaderColWidth();
            foreach (var item in colNameBinding)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = item.Key;
                col.Binding = new Binding(item.Value);
                col.Width = colNameWidth[item.Key];
                userfriendstable.Columns.Add(col);
            }

        }

        private void initFriendsLatestTipTableHeader()
        {
            Dictionary<string, int> colNameWidth = Constants.getFriendsLatestTipTableColWidth();
            foreach (var item in colNameWidth)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = item.Key;
                col.Width = colNameWidth[item.Key];
                friendslatesttipstable.Columns.Add(col);
            }
        }




        /// <summary>
        /// State selection change event.
        /// Display corresponding cities of selected state.
        /// </summary>
        /// <param name="sender"> obj. </param>
        /// <param name="e">event. </param>
        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityList.Items.Clear();
            if (stateList.SelectedIndex < 0)
            {
                return;
            }
            string sqlstr = $"SELECT distinct city FROM business WHERE state = '{stateList.SelectedItem.ToString()}' ORDER BY city";
            Utils.executeQuery(sqlstr, addCityUI);
        }

        private void addCityUI(NpgsqlDataReader reader)
        {
            cityList.Items.Add(reader.GetString(0));
        }


        /// <summary>
        /// Event that to display the zip codes when selected a city from [cityList]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            zipList.Items.Clear();
            if (cityList.SelectedIndex < 0)
            {
                return;
            }
            string sqlstr = $"SELECT distinct zipcode from business WHERE state = '{stateList.SelectedItem.ToString()}' AND city = '{cityList.SelectedItem.ToString()}' ORDER BY zipcode";
            Utils.executeQuery(sqlstr, addZipCodeUI);
        }
        private void addZipCodeUI(NpgsqlDataReader reader)
        {
            zipList.Items.Add(reader.GetString(0));
        }


        /// <summary>
        /// Event that to display business category when select a zip code from [zipList]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zipList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessCatList.Items.Clear();
            if (zipList.SelectedIndex < 0)
            {
                return;
            }
            string sqlstr = $"select distinct business_category.name from business, business_category where" +
                $" state = '{stateList.SelectedItem.ToString()}' and city = '{cityList.SelectedItem.ToString()}'" +
                $" and zipcode = '{zipList.SelectedItem.ToString()}' and business.business_id = business_category.business_id";
            Utils.executeQuery(sqlstr, addBusinessCat);
        }

        private void addBusinessCat(NpgsqlDataReader reader)
        {
            businessCatList.Items.Add(reader.GetString(0));
        }

        /// <summary>
        /// Add category to the [selectedCatList] from [businessCatList]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (businessCatList.SelectedIndex < 0)
            {
                return;
            }
            string selectedCat = businessCatList.SelectedItem.ToString();
            if (!selectedCatList.Items.Contains(selectedCat))
            {
                selectedCatList.Items.Add(selectedCat);
            }
            Console.WriteLine(selectedCatList.Items.ToString());
        }

        /// <summary>
        /// Remove category from the [selectedCatList]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeCatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCatList.SelectedIndex < 0)
            {
                return;
            }
            selectedCatList.Items.RemoveAt(selectedCatList.SelectedIndex);
        }

        /// <summary>
        /// Event that click the search button, should that the business info on [businessInfoTable].
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchBusBtn_Click(object sender, RoutedEventArgs e)
        {
            businessInfoTable.Items.Clear();
            string sqlstr = ""; // placeholder.

            // reverse order check
            // since city only appear when state selected so on so forth... we check from the bottom up using ifelse
            if (selectedCatList.Items.Count > 0)
            {
                // shows all of busienss with state, city, zipcode provided
                string sql1 = $"select * from business where" +
                    $" state = '{stateList.SelectedItem.ToString()}' and city = '{cityList.SelectedItem.ToString()}'" +
                    $" and zipcode = '{zipList.SelectedItem.ToString()}' and business_id in ";

                List<string> cats = new List<string>();
                foreach (var item in selectedCatList.Items)
                {
                    cats.Add("'" + item.ToString() + "'");
                }
                string string_cats = string.Join(",", cats.ToArray());
                string sql2 = $"(select business_id from business_category where name in ({string_cats}) GROUP BY business_id HAVING count(1) = {selectedCatList.Items.Count})";
                sqlstr = sql1 + sql2;
            }
            // search zip, city, state
            else if (zipList.SelectedIndex >= 0)
            {
                sqlstr = $"select * from business where" +
                    $" state = '{stateList.SelectedItem.ToString()}' and city = '{cityList.SelectedItem.ToString()}'" +
                    $" and zipcode = '{zipList.SelectedItem.ToString()}'";
            }
            // search city, state
            else if (cityList.SelectedIndex >= 0)
            {
                sqlstr = $"select * from business where" +
                    $" state = '{stateList.SelectedItem.ToString()}' and city = '{cityList.SelectedItem.ToString()}'";
            }
            // search state
            else if (stateList.SelectedIndex >= 0)
            {
                sqlstr = $"select * from business where state = '{stateList.SelectedItem.ToString()}'";
            }
            // highest level, show all business
            else
            {
                sqlstr = $"select * from business";
            }

            Utils.executeQuery(sqlstr, showResult);
            busCnt.Text = businessInfoTable.Items.Count.ToString();
            
        }

        private void updateDistance()
        {
            string businessID = Utils.currentBus.business_id;
            string userID = Utils.currentUser;
            string sql = $"select getdistance(business.latitude, business.longitude, the_user.latitude, the_user.longtiude) from business full outer join the_user" +
                $" on 1 = 1 where business_id = '{businessID}' and user_id = '{userID}'";
            Utils.executeQuery(sql, distanceUIupdate);
        }

        private void distanceUIupdate(NpgsqlDataReader reader)
        {
            Utils.currentBus.distance = reader.GetDouble(0);
        }

        private void showResult(NpgsqlDataReader reader)
        {
            Business bus = new Business();
            bus.business_id = reader.GetString(0);
            bus.name = reader.GetString(1);
            bus.city = reader.GetString(2);
            bus.state = reader.GetString(3);
            bus.zipcode = reader.GetString(4);
            bus.address = reader.GetString(5);
            bus.stars = reader.GetFloat(6);
            bus.latitude = reader.GetDouble(7);
            bus.longitude = reader.GetDouble(8);
            bus.num_checkings = reader.GetInt64(9);
            bus.num_tips = reader.GetInt64(10);
            bus.is_open = reader.GetBoolean(11);
            
            /*
            Utils.currentBus = bus;
            updateDistance();
            */
            businessInfoTable.Items.Add(bus);

        }

        public void selectBusiness(object sender, RoutedEventArgs e)
        {
            if (businessInfoTable.SelectedIndex > -1)
            {
                Business bus = (Business)businessInfoTable.SelectedItem;
                LabelName.Text = bus.name;
                LabelAddress.Text = bus.address;
                LabelHours.Text = "Not required for Milestone 2 amirite?";
            } else
            {
                LabelName.Text = "Name";
                LabelAddress.Text = "Address";
            }
        }

        public void showTip(object sender, RoutedEventArgs e)
        {
            if (businessInfoTable.SelectedIndex != -1)
            {
                TipText tt = new TipText(((Business)businessInfoTable.SelectedItem).business_id);
                tt.Show();
            }
        }

        public void nameSearchChange(object sender, RoutedEventArgs e)
        {
            userList.Items.Clear();
            if (nameSearch.Text.Trim().Length > 0)
            {
                string sql = $"select user_id from the_user where name like '%{ nameSearch.Text }%' ";
                Utils.executeQuery(sql, addUserToList);
            }
        }

        private void addUserToList(NpgsqlDataReader reader)
        {
            userList.Items.Add(reader.GetString(0));
        }

        public void userSelect(object sender, RoutedEventArgs e)
        {
            Utils.currentUser = userList.SelectedItem.ToString();
            string sql = $"select * from the_user where user_id = '{Utils.currentUser}' ";
        }

        private void displayUserInfo(NpgsqlDataReader reader)
        {

        }


        /// <summary>
        /// Select business event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectBusiness(object sender, SelectionChangedEventArgs e)
        {
            this.infoList.Items.Clear();
            if (this.businessInfoTable.SelectedIndex < 0)
            {
                return;
            }
            Business bus = (Business)businessInfoTable.SelectedItem;
            string businessId = bus.business_id;
            infoList.Items.Add("\u2022 Category");
            string sqlstr = "select name from business_category where business_id = '" + businessId + "'";
            Utils.executeQuery(sqlstr, addInfoListCat);
            infoList.Items.Add("\u2022 Attributes");
            string sqlstr1 = $"select name from business_attribute where business_id = '{businessId}' and value = 'True'";
            Utils.executeQuery(sqlstr1, addInfoListAtt);
        }

        private void addInfoListCat(NpgsqlDataReader reader)
        {
            infoList.Items.Add("\t" + reader.GetString(0));
        }

        private void addInfoListAtt(NpgsqlDataReader reader)
        {
            infoList.Items.Add("\t" + reader.GetString(0));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// User id selection event, should display user info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userSelect(object sender, SelectionChangedEventArgs e)
        {
            if (this.userList.SelectedIndex < 0)
            {
                return;
            }
            string userId = (string)userList.SelectedItem;
            string sqlstr = $"select name, average_stars, fans, yelp_since, funny, cool, useful, tip_count, total_likes, latitude, longtiude from the_user where user_id = '{userId}'";
            Utils.executeQuery(sqlstr, addUserInfo);
        }
        
        private void addUserInfo(NpgsqlDataReader reader)
        {
            this.user_name.Text = reader.GetString(0);
        }
    }
}
