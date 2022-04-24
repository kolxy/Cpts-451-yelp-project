using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpQueryEngine
{
    public class Business
    {
        public string business_id { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public string address { get; set; }
        public float stars { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public Int64 num_checkings { get; set; }
        public Int64 num_tips { get; set; }
        public bool is_open { get; set; }
        public double distance { get; set; }
    }
}
