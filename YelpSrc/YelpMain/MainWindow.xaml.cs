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
        Dictionary<string, bool> attFilter;

        public MainWindow()
        {
            InitializeComponent();
            initState();
            initBusinessInfoTableHeader();
            initUserFriendTableHeader();
            initFriendsLatestTipTableHeader();
            initUserId();
            attFilter = new Dictionary<string, bool>();
            initFilter();
            initSortBar();
        }

        private void initSortBar()
        {
            sortList.Items.Add("Name (default)");
            sortList.Items.Add("Highest rated");
            sortList.Items.Add("Most number of tips");
            sortList.Items.Add("Most checkings");
            sortList.Items.Add("Nearest");
            sortList.SelectedIndex = 0;
        }

        private void initFilter()
        {
            List<string> attList = new List<string> { "BusinessAcceptsCreditCards" , "RestaurantsReservations", "WheelchairAccessible",
            "OutdoorSeating", "GoodForKids", "RestaurantsGoodForGroups", "RestaurantsDelivery", "RestaurantsTakeOut", "WiFi", "BikeParking",
            "breakfast", "brunch", "lunch", "dinner", "desert", "latenight","RestaurantsPriceRange1", "RestaurantsPriceRange2", "RestaurantsPriceRange3", "RestaurantsPriceRange4"};
            foreach (string att in attList)
            {
                this.attFilter.Add(att, false);
            }
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
            Dictionary<string, string> colNameBinding = Constants.getFriendsLatestTipTableBinder();
            Dictionary<string, int> colNameWidth = Constants.getFriendsLatestTipTableColWidth();
            foreach (var item in colNameBinding)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = item.Key;
                col.Binding = new Binding(item.Value);
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
            markAttributes();

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
                string att = getAllFiltersInTupleStr();
                int cnt = getCntAttChecked();
                string sql1 = $"(select business_id from business_attribute where name in ({att}) GROUP BY business_id HAVING count(1) = {cnt.ToString()})";
                string sql2 = $"select * from business and business_id in ";
                sqlstr = sql2 + sql1;

            }

            Utils.executeQuery(sqlstr, showResult);
            busCnt.Text = businessInfoTable.Items.Count.ToString();
            
        }

        private void markAttributes()
        {
            // set dictionary to false each time search
            foreach (var key in attFilter.Keys.ToList()) { attFilter[key] = false; }

            // mark true when checkbox checked
            if ((bool)(this.RestaurantsPriceRange1.IsChecked)) { this.attFilter["RestaurantsPriceRange1"] = true; }
            if ((bool)(this.RestaurantsPriceRange2.IsChecked)) { this.attFilter["RestaurantsPriceRange2"] = true; }
            if ((bool)(this.RestaurantsPriceRange3.IsChecked)) { this.attFilter["RestaurantsPriceRange3"] = true; }
            if ((bool)(this.RestaurantsPriceRange4.IsChecked)) { this.attFilter["RestaurantsPriceRange4"] = true; }
            if ((bool)(this.BusinessAcceptsCreditCards.IsChecked)) { this.attFilter["BusinessAcceptsCreditCards "] = true; }
            if ((bool)(this.RestaurantsReservations.IsChecked)) { this.attFilter["RestaurantsReservations"] = true; }
            if ((bool)(this.OutdoorSeating.IsChecked)) { this.attFilter["OutdoorSeating "] = true; }
            if ((bool)(this.GoodForKids.IsChecked)) { this.attFilter["GoodForKids"] = true; }
            if ((bool)(this.RestaurantsGoodForGroups.IsChecked)) { this.attFilter["RestaurantsGoodForGroups"] = true; }
            if ((bool)(this.RestaurantsDelivery.IsChecked)) { this.attFilter["RestaurantsDelivery"] = true; }
            if ((bool)(this.RestaurantsTakeOut.IsChecked)) { this.attFilter["RestaurantsTakeOut"] = true; }
            if ((bool)(this.WiFi.IsChecked)) { this.attFilter["WiFi "] = true; }
            if ((bool)(this.BikeParking.IsChecked)) { this.attFilter["BikeParking"] = true; }
            if ((bool)(this.breakfast.IsChecked)) { this.attFilter["breakfast "] = true; }
            if ((bool)(this.brunch.IsChecked)) { this.attFilter["brunch "] = true; }
            if ((bool)(this.lunch.IsChecked)) { this.attFilter["lunch "] = true; }
            if ((bool)(this.dinner.IsChecked)) { this.attFilter["dinner "] = true; }
            if ((bool)(this.desert.IsChecked)) { this.attFilter["desert "] = true; }
            if ((bool)(this.latenight.IsChecked)) { this.attFilter["latenight"] = true; }
        }

        private string getAllFiltersInTupleStr()
        {
            List<string>  strList = new List<string>();
            foreach(string key in attFilter.Keys.ToList()){ 
                if (attFilter[key] == true)
                {
                    strList.Append("'" + key + "'");
                }
            }
            string ans = string.Join(",", strList.ToArray());
            MessageBox.Show(ans);
            return ans;
        }

        private int getCntAttChecked()
        {
            int cnt = 0;
            foreach (string key in attFilter.Keys.ToList()) {if (attFilter[key] == true){cnt += 1;}}
            return cnt;
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

        public void showTip(object sender, RoutedEventArgs e)
        {
            if (businessInfoTable.SelectedIndex != -1)
            {
                TipText tt = new TipText(((Business)businessInfoTable.SelectedItem).business_id);
                tt.Show();
            }
        }
        private void checkInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (businessInfoTable.SelectedIndex < 0)
            {
                return;
            }

            CheckinWindow ckWin = new CheckinWindow(((Business)businessInfoTable.SelectedItem).business_id);
            ckWin.Show();

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
            LabelName.Text = bus.name;
            LabelAddress.Text = bus.address;
            string businessId = bus.business_id;
            infoList.Items.Add("\u2022 Category");
            string sqlstr = "select name from business_category where business_id = '" + businessId + "'";
            Utils.executeQuery(sqlstr, addInfoListCat);
            infoList.Items.Add("\u2022 Attributes");
            string sqlstr1 = $"select name, value from business_attribute where business_id = '{businessId}' ";
            Utils.executeQuery(sqlstr1, addInfoListAtt);
            string sqlstr3 = $"select day, open, close from business_hour where business_id = '{businessId}'";
            Utils.executeQuery(sqlstr3, addHour);
        }

        private void addInfoListCat(NpgsqlDataReader reader)
        {
            infoList.Items.Add("\t" + reader.GetString(0));
        }

        private void addInfoListAtt(NpgsqlDataReader reader)
        {
            infoList.Items.Add("\t" + reader.GetString(0) + ": " + reader.GetString(1));
        }

        private void addHour(NpgsqlDataReader reader)
        {
            LabelHours.Text = (reader.GetString(0) + ":         Opens: " + reader.GetTimeSpan(1).ToString() + "         Closes: " + reader.GetTimeSpan(2).ToString()).ToString();
        }


        /// <summary>
        /// User id selection event, should display user info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void userSelect(object sender, RoutedEventArgs e)
        {
            if (userList.SelectedIndex >= 0)
            {
                userfriendstable.Items.Clear();
                friendslatesttipstable.Items.Clear();
                Utils.currentUser = userList.SelectedItem.ToString();
                string sql = $"select * from the_user where user_id = '{Utils.currentUser}' ";
                Utils.executeQuery(sql, displayUserInfo);
                sql = $"select name, total_likes, average_stars, yelp_since from user_follow, the_user where user_follow.friend_id = the_user.user_id and user_follow.user_id = '{Utils.currentUser}' ";
                Utils.executeQuery(sql, addFriendToList);
                sql = $@"SELECT
                        u.NAME AS user_name,
	                    b.NAME AS business_name,
	                    b.city,
	                    T.TEXT,
	                    T.TIMESTAMP
                    FROM
                        user_follow AS f,
	                    the_user AS u,
	                    tips AS T,
	                    (SELECT user_id, MAX(TIMESTAMP) AS TIMESTAMP FROM tips GROUP BY user_id ) AS latest,
                        business AS b
                    WHERE
                        f.friend_id = u.user_id
                        AND T.user_id = u.user_id
                        AND b.business_id = T.business_id
                        AND f.user_id = '{Utils.currentUser}'
                        AND T.user_id = latest.user_id
                        AND T.TIMESTAMP = latest.TIMESTAMP 
                    ORDER BY
                        T.timestamp DESC"; 
                Utils.executeQuery(sql, addLatestTipToList);
            }
        }

        private void displayUserInfo(NpgsqlDataReader reader)
        {
            user_name.Text = reader.GetString(reader.GetOrdinal("name"));
            user_stars.Text = reader.GetDouble(reader.GetOrdinal("average_stars")).ToString();
            user_fans.Text = reader.GetDouble(reader.GetOrdinal("fans")).ToString();
            user_since.Text = reader.GetTimeStamp(reader.GetOrdinal("yelp_since")).ToString();
            vote_funny.Text = reader.GetInt16(reader.GetOrdinal("funny")).ToString();
            vote_cool.Text = reader.GetInt16(reader.GetOrdinal("cool")).ToString();
            vote_useful.Text = reader.GetInt16(reader.GetOrdinal("useful")).ToString();
            user_tipcounts.Text = reader.GetInt16(reader.GetOrdinal("tip_count")).ToString();
            user_tiplikes.Text = reader.GetInt16(reader.GetOrdinal("total_likes")).ToString();
            user_lat.Text = reader.GetDouble(reader.GetOrdinal("latitude")).ToString();
            user_long.Text = reader.GetDouble(reader.GetOrdinal("longtiude")).ToString();
        }

        private void addFriendToList(NpgsqlDataReader reader)
        {
            userfriendstable.Items.Add(new
            {
                name = reader.GetString(reader.GetOrdinal("name")),
                total_likes = reader.GetInt32(reader.GetOrdinal("total_likes")).ToString(),
                average_stars = reader.GetDouble(reader.GetOrdinal("average_stars")).ToString(),
                yelp_since = reader.GetTimeStamp(reader.GetOrdinal("yelp_since")).ToString()
            });
        }

        private void addLatestTipToList(NpgsqlDataReader reader)
        {
            friendslatesttipstable.Items.Add(new
            {
                user_name = reader.GetString(reader.GetOrdinal("user_name")),
                business_name = reader.GetString(reader.GetOrdinal("business_name")),
                city = reader.GetString(reader.GetOrdinal("city")),
                text = reader.GetString(reader.GetOrdinal("text")),
                date = reader.GetTimeStamp(reader.GetOrdinal("timestamp")).ToString(),
            });
        }

        public void editClick(object sender, RoutedEventArgs e)
        {
            user_lat.IsReadOnly = false;
            user_long.IsReadOnly = false;
            user_lat.Background = Brushes.White;
            user_long.Background = Brushes.White;
        }

        public void coordUpdate(object sender, RoutedEventArgs e)
        {
            user_lat.IsReadOnly = true;
            user_long.IsReadOnly = true;
            user_lat.Background = Brushes.LightGray;
            user_long.Background = Brushes.LightGray;
            string sql = $"UPDATE the_user set latitude = '{user_lat.Text}', longtiude = '{user_long.Text}' where user_id = '{Utils.currentUser}'";
            Utils.executeQuery(sql, doNothing);
        }

        public void doNothing(NpgsqlDataReader reader)
        {
            Console.WriteLine("I love database");
        }
    }
}
