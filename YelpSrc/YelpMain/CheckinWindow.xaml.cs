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
        string businessId; 
        public CheckinWindow()
        {
            InitializeComponent();
        }

        public CheckinWindow(string businessId)
        {
            InitializeComponent();
            this.businessId = businessId;
            numChecking = new double[12];
            months = new double[12];
            plot();
        }

        private void plot()
        {
            WpfPlot1.Plot.Clear();
            for (int i = 0; i < 12; i++)
            {
                months[i] = i + 1;
                numChecking[i] = 0;
            }
  
            string sql = $"select month, count(*) from checkin where business_id = '{this.businessId}' group by month";
            Utils.executeQuery(sql, getCheckins);

            string title = "Checkin Stats";
            WpfPlot1.Plot.PlotBar(months.ToArray(), numChecking.ToArray(), showValues: true);
            WpfPlot1.plt.XLabel("Months");
            WpfPlot1.plt.YLabel("# of Checkins");
            WpfPlot1.plt.Title(title);
            WpfPlot1.Refresh();
        }


        private void getCheckins(NpgsqlDataReader reader)
        {
            int idx = reader.GetInt32(0) - 1;
            this.numChecking[idx] = (double)reader.GetInt64(1);
        }

        /// <summary>
        /// Here we should be able to checkIn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.currentUser.Length <= 0)
            {
                MessageBox.Show("In order to checkin pls first login!");
                return; 
            }
            string sql = $"insert into checkin (business_id, year, month, day, clock_time) values ('{businessId}', '{Int64.Parse(DateTime.Now.Year.ToString())}'," +
                $"'{Int64.Parse(DateTime.Now.Month.ToString())}', '{Int64.Parse(DateTime.Now.Day.ToString())}', '{DateTime.Now.ToString("HH:mm:ss")}')";
            Utils.executeQuery(sql, null);
            Console.WriteLine(sql);
            plot();
        }
    }
}
