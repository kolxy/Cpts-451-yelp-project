using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpQueryEngine
{
    public class Constants
    {
        public static Dictionary<string, string> getBusinessInfoTableHeaderNameAndBinder()
        {
            Dictionary<string, string> colNameBinding = new Dictionary<string, string>();
            colNameBinding.Add("Business Name", "businessName");
            colNameBinding.Add("Address", "address");
            colNameBinding.Add("City", "city");
            colNameBinding.Add("State", "state");
            colNameBinding.Add("Distance (miles)", "distance");
            colNameBinding.Add("Stars", "stars");
            colNameBinding.Add("# of Tips", "numTip");
            colNameBinding.Add("Total Checkins", "totalCheckins");
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


    }
}
