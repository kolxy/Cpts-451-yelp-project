using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpQueryEngine
{
    public class Constants
    {
 
        // ---------------------------------------------------------------------- Search Business UI setting ---------------------------------------------//
        public static Dictionary<string, string> getBusinessInfoTableHeaderNameAndBinder()
        {
            Dictionary<string, string> colNameBinding = new Dictionary<string, string>();
            colNameBinding.Add("Business Name", "name");
            colNameBinding.Add("Address", "address");
            colNameBinding.Add("City", "city");
            colNameBinding.Add("State", "state");
            colNameBinding.Add("Distance (miles)", "distance");
            colNameBinding.Add("Stars", "stars");
            colNameBinding.Add("# of Tips", "num_tips");
            colNameBinding.Add("Total Checkins", "num_checkings");
            colNameBinding.Add("bid", "business_id");
            return colNameBinding;
        }

        public static Dictionary<string, int> getBusinessInfoTableHeaderColWidth()
        { 
            //Total width = 826
            Dictionary<string, int> colNameWidth = new Dictionary<string, int>();
            colNameWidth.Add("Business Name", 125);
            colNameWidth.Add("Address", 200);
            colNameWidth.Add("City", 75);
            colNameWidth.Add("State", 75);
            colNameWidth.Add("Distance (miles)", 110);
            colNameWidth.Add("Stars", 65);
            colNameWidth.Add("# of Tips", 80);
            colNameWidth.Add("Total Checkins", 95);
            colNameWidth.Add("bid", 0);
            return colNameWidth;
        }

        // ---------------------------------------------------------------------- User portal UI setting ---------------------------------------------//
        public static Dictionary<string, string> getUserFriendTableBinder()
        {
            Dictionary<string, string> colNameBinding = new Dictionary<string, string>();
            colNameBinding.Add("Name", "name");
            colNameBinding.Add("TotalLikes", "total_likes");
            colNameBinding.Add("Avg Stars", "average_stars");
            colNameBinding.Add("Yelping Since", "yelp_since");
            return colNameBinding;
        }

        // total width: "409"
        public static Dictionary<string, int> getUserFriendTableHeaderColWidth()
        {
            Dictionary<string, int> colNameWidth = new Dictionary<string, int>();
            colNameWidth.Add("Name", 79);
            colNameWidth.Add("TotalLikes", 75);
            colNameWidth.Add("Avg Stars", 75);
            colNameWidth.Add("Yelping Since", 180);
            return colNameWidth;
        }

        public static Dictionary<string, string> getFriendsLatestTipTableBinder()
        {
            Dictionary<string, string> colNameBinding = new Dictionary<string, string>();
            colNameBinding.Add("User Name", "user_name");
            colNameBinding.Add("Business", "business_name");
            colNameBinding.Add("City", "city");
            colNameBinding.Add("Text", "text");
            colNameBinding.Add("Date", "date");
            return colNameBinding;
        }

        //friendslatesttipstable
        // total width: 907
        public static Dictionary<string, int> getFriendsLatestTipTableColWidth()
        {
            Dictionary<string, int> colNameWidth = new Dictionary<string, int>();
            colNameWidth.Add("User Name", 107);
            colNameWidth.Add("Business", 150);
            colNameWidth.Add("City", 100);
            colNameWidth.Add("Text", 300);
            colNameWidth.Add("Date", 250);
            return colNameWidth;
        }

    }
}
