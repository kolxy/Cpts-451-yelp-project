using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpQueryEngine
{
    public class User
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public DateTime yelp_since { get; set; }
        public double latitude { get; set; }
        public double longtitude { get; set; }
        public double average_stars { get; set; }
        public Int64 tip_count { get; set; }
        public Int64 total_likes { get; set; }
        public Int64 useful { get; set; }
        public Int64 funny { get; set; }
        public Int64 cool { get; set; }
        public Int64 fans { get; set; }

    }
}
