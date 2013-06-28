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
           
            
            if (listBox_freeway.SelectedItem==null)
            {
                MessageBox.Show("You must select a highway", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.Cursor = Cursors.Wait;
            string str = listBox_freeway.SelectedItem.ToString();
            string []tmpstr = str.Split('-');
            GeneralInfo tmphw = new GeneralInfo();
            tmphw.ID=tmpstr[0];
            tmphw.Name=tmpstr[1];
            Highway hwobj = hwserver.getHighwayObject(tmphw);
            ArrayList seclist = hwobj.getSectionList();
            //foreach (GeneralInfo item in seclist)
            //{
                //listBox_section.Items.Add(item.ID + "-" + item.Name);
                // .Show(item.ID + " " + item.Name);
            //}
            Highway520.Section secobj = hwobj.getSectionObject((GeneralInfo)seclist[0], (GeneralInfo)seclist[seclist.Count-1]);
            ArrayList nodelist = secobj.getTrafficNodeList();
            string msg = "";
            foreach (NodeInfo item in nodelist)
            {
                
                if (item.Direction > 2)
                    msg = item.Name + " - 東向 時速" + item.Speed.SN_WE + "- 西向 時速" + item.Speed.NS_EW ;
                else
                    msg = item.Name + " - 北上 時速" + item.Speed.SN_WE + "- 南下 時速" + item.Speed.NS_EW ;
                listBox_traffic.Items.Add(msg);
            }
            this.Cursor = Cursors.None;
            
        }
    }
}
