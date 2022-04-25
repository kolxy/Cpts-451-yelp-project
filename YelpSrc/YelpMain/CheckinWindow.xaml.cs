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
using ScottPlot.WPF;
using YelpQueryEngine;
using Npgsql;

namespace YelpMain
{
    /// <summary>
    /// Interaction logic for CheckinWindow.xaml
    /// </summary>
    public partial class CheckinWindow : Window
    {
        double[] numChecking = new double[12];
        double [] months = new double[12];
        Int64 maxYear = 0;

        public CheckinWindow()
        {
            InitializeComponent();
        }

        public CheckinWindow(string businessId)
        {
            InitializeComponent();
            numChecking = new double[12];
            months = new double[12];

            for (int i = 0; i < 12; i++)
            {
                months[i] = i + 1;
                numChecking[i] = 0;
            }
            string sqlMaxYear = $"select max(year) from checkin where business_id = '{businessId}'";

            string sql = $"select month, count(*) from checkin where " +
                $"year = (select max(year) from checkin where business_id = '{businessId}') and business_id = '{businessId}' group by month";
            Utils.executeQuery(sql, getCheckins);
            Utils.executeQuery(sqlMaxYear, getMaxYear);
            plot();
        }

        private void getCheckins(NpgsqlDataReader reader)
        {
            int idx = reader.GetInt32(0)-1;
            this.numChecking[idx] = (double)reader.GetInt64(1);
        }

        private void getMaxYear(NpgsqlDataReader reader)
        {
            try
            {
                this.maxYear = reader.GetInt64(0);
            }
            catch (Exception e)
            {
                this.maxYear = 0;
            }
        }

        private void plot()
        {
            string title = "Checkin Stats For Most Recent Year: " + maxYear.ToString();
            WpfPlot1.Plot.PlotBar(months.ToArray(), numChecking.ToArray(), showValues: true);
            WpfPlot1.plt.XLabel("Months");
            WpfPlot1.plt.YLabel("# of Checkins");
            WpfPlot1.plt.Title(title);
            WpfPlot1.Refresh();
        }


    }
}
