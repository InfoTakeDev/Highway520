using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Highway520;
using System.Collections;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        HighwayServer hwserver;
        public MainWindow()
        {
            
            InitializeComponent();
            hwserver = new HighwayServer();
            ArrayList hwlist = hwserver.getHighwayList();
            foreach (GeneralInfo item in hwlist)
            {
                listBox_freeway.Items.Add(item.ID + "-" + item.Name);
                //listBox_freeway.Items.Add(item.ID);
                
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string str = listBox_freeway.SelectedItem.ToString();
            string []tmpstr = str.Split('-');
            GeneralInfo tmphw = new GeneralInfo();
            tmphw.ID=tmpstr[0];
            tmphw.Name=tmpstr[1];
            Highway hwobj = hwserver.getHighwayObject(tmphw);
            ArrayList seclist = hwobj.getSectionList();
            foreach (GeneralInfo item in seclist)
            {
                listBox_section.Items.Add(item.ID + "-" + item.Name);
                // .Show(item.ID + " " + item.Name);
            }
            Highway520.Section secobj = hwobj.getSectionObject((GeneralInfo)seclist[0], (GeneralInfo)seclist[seclist.Count-1]);
            ArrayList nodelist = secobj.getTrafficNodeList();

            foreach (NodeInfo item in nodelist)
            {
                listBox_traffic
.Items.Add(item.Direction + " " + item.Speed.SN_WE + " " + item.Speed.NS_EW + " " + item.Name);
            }
            
            
        }
    }
}
