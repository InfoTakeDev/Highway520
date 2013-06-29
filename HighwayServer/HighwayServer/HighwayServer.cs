using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections; // for ArrayList
namespace Highway520
{
    public struct GeneralInfo
    {
        public string Name { get; set; }
        public string ID { get; set; }// ID equals to position
    }

    public struct SpeedInfo
    {
        public string NS_EW { get; set; }
        public string SN_WE { get; set; }
    }

    public struct NodeInfo
    {
        //public string from { get; set; }
        //public string to { get; set; }
        public string Name { get; set; }
        public SpeedInfo Speed;
        public string Incident { get; set; }
        // 1,2 for South to North/South to North, 3,4 for East to West/East to West
        public int Direction { get; set; }
    }

    public class Section
    {
        private string HWID;
        private string StartID;
        private string EndID;
        public Section(string hwid, string startid, string endid)
        {
            this.HWID = hwid;
            if (Convert.ToInt32(endid) < Convert.ToInt32(startid))
            {
                this.StartID = endid;
                this.EndID = startid;
            }
            else
            {
                this.StartID = startid;
                this.EndID = endid;
            }

        }
        public string getURI()
        {
            return HighwayServer.BaseURI + "/traffic/getsectraffic/fid/" + this.HWID + "/from/" + StartID + "/end/" + EndID;
        }

        public ArrayList getTrafficNodeList()
        {
            Tool tl = new Tool();
            string sbstr = tl.queryInfo(this.getURI());
            ArrayList nodelist = tl.parseSpeed(sbstr);
            return nodelist;
        }
    }

    public class Highway
    {
        private string HighwayID;
        public Highway(string hwid)
        {
            this.HighwayID = hwid;
        }

        public string getURI()
        {
            return  HighwayServer.BaseURI + "/common/getnodsecs/fid/" + this.HighwayID + "?lc=1&id=end_selt";
        }

        // getSectoinList
        public ArrayList getSectionList()
        {
            Tool tl = new Tool();
            string sbstr = tl.queryInfo(this.getURI());
            ArrayList seclist = tl.ParseInfo(sbstr);
            return seclist;
        }
        // getSectionInformation returns traffic information
        public Section getSectionObject(GeneralInfo startsec, GeneralInfo endsec)
        {
            Section sec = new Section(this.HighwayID, startsec.ID, endsec.ID);
            return sec;
        }

    }

    public class HighwayServer
    {
        public const string BaseURI = "http://1968.freeway.gov.tw";
        public HighwayServer()
        {
        }

        public string getURI()
        {
            return BaseURI + "/common/getfrees?id=sec_selt&df=1"; 
        }

        public ArrayList getHighwayList()
        {
            Tool tl = new Tool();
            string sbstr = tl.queryInfoInDB(this.getURI());
            ArrayList hwlist = tl.ParseInfo(sbstr);
            return hwlist;
        }
        // return a highway object
        public Highway getHighwayObject(GeneralInfo highway)
        {
            Highway hw = new Highway(highway.ID);
            return hw;
        }

    }

    // use xml file
    class HighwayDB
    {
        // initialize database.
        public HighwayDB()
        {
        }

        public void updateSectionDB()
        {


            //Console.WriteLine(Uri.EscapeDataString(URI));
            //using (StreamWriter sw = new StreamWriter(Uri.EscapeDataString(URI) + ".xml"))   //小寫TXT     
            //{
                //sw.Write(output);
            //}
        }
        public void updateHighwayDB()
        {
            HighwayServer hwserver = new HighwayServer();

            string uri = hwserver.getURI();
            Tool tl = new Tool();
            string sbstr = tl.queryInfo(uri);
            Console.Write("Update Highway list...");
            using (StreamWriter sw = new StreamWriter(Uri.EscapeDataString(uri) + ".xml"))   //小寫TXT     
            {
                sw.Write(sbstr);
            }
            Console.WriteLine("\t\tDone.");
            /*foreach (GeneralInfo item in hwlist)
            {
                Console.WriteLine(item.ID + "-" + item.Name);


                Highway hwobj = hwserver.getHighwayObject(item);
                ArrayList seclist = hwobj.getSectionList();

                //listBox_freeway.Items.Add(item.ID);

            }*/

            //Console.WriteLine(Uri.EscapeDataString(URI));
            //using (StreamWriter sw = new StreamWriter(Uri.EscapeDataString(URI) + ".xml"))   //小寫TXT     
            //{
            //sw.Write(output);
            //}
            // you have to update section DB if you update highway DB.
        }


    }

    class Tool
    {
        Int32 Timeout = 10000; // set default timeout to 10 second

       
        public void setTimeout(Int32 timeout)
        {
            this.Timeout = timeout;
        }


        // return "" for timeout or URI error
        public string queryInfoInDB(string URI)
        {
            // read comtent in db
            try
            {
                using (StreamReader sr = new StreamReader(Uri.EscapeDataString(URI)+".xml"))
                {
                    return sr.ReadToEnd().Trim();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return "";
            }
            
        }
        
        //public caculateTime(){
        //}

        public string queryInfo(string URI)
        {
            
            try
            {
               
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
                req.Timeout = this.Timeout;
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string output = sr.ReadToEnd().Trim();
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return "";
            }
        }



        public ArrayList ParseInfo(string dom)
        {
            ArrayList infoList = new ArrayList();
            if (dom == "")
                return infoList;
            XDocument doc = XDocument.Parse(dom);
            
            foreach (XElement el in doc.Root.Elements())
            {
                GeneralInfo gi = new GeneralInfo();
                gi.Name = el.Value;
                gi.ID = el.Attribute("value").Value;
                infoList.Add(gi);
            }
            return infoList;
        }

        public ArrayList parseSpeed(string dom)
        {
            ArrayList NodeList = new ArrayList();
            if (dom == "")
                return NodeList;

            int idx = dom.IndexOf("<script type=");
            int direction = 1;
            if (idx != -1)
            {
                /*
			    1,2 for North/South
			    3,4 for West/East
		        */
                Console.WriteLine(dom);
                foreach (string line in dom.Substring(idx).Split('\n'))
                {

                    if (line.IndexOf("direction =") != -1)
                    {
                        Console.WriteLine(line);
                        direction = Convert.ToInt16(line.Substring(11 + 1).TrimEnd(';'));
                        Console.WriteLine("dir=" + direction);
                    }
                }
                dom = "<root>" + dom.Substring(0, idx) + "</root>";
            }

            XDocument doc = XDocument.Parse(dom);
            

            foreach (XElement childtr in doc.Root.Elements())
            {
                NodeInfo ni = new NodeInfo();
                ni.Direction = direction;
                foreach (XElement childtd in childtr.Elements())
                {
                    switch (childtd.Attribute("class").Value)
                    {
                        case "speed speedLeft":
                            ni.Speed.SN_WE = childtd.Value.Trim();
                            break;
                        case "sec_name":
                            // parse the information
                            ni.Name = childtd.Value.Trim(); ;
                            break;
                        case "speed speedRight":
                            ni.Speed.NS_EW = childtd.Value.Trim(); ;
                            break;
                    }
                }
                //Console.WriteLine("{0} {1} {2} {3}", ni.direction, ni.speed.SN_WE, ni.speed.NS_EW, ni.name);
                NodeList.Add(ni);
            }
            return NodeList;
        }

    }


    class Program
    {
        static void Main(string []args)
        {
            DateTime startTime = DateTime.Now;
            Console.WriteLine("test");
            HighwayDB db = new HighwayDB();
            //db.updateHighwayDB();
            HighwayServer hs = new HighwayServer();
            ArrayList hwlist = hs.getHighwayList();


            foreach (GeneralInfo item in hwlist)
            {
               Console.WriteLine(item.ID + "-" + item.Name);
            }

            // cacuate the time eslape
            DateTime endTime = DateTime.Now;
            TimeSpan span = endTime.Subtract(startTime);
            Console.WriteLine("Time Difference (milliseconds): " + span.Milliseconds);
            Console.WriteLine("Time Difference (seconds): " + span.Seconds);
            Console.WriteLine("Time Difference (minutes): " + span.Minutes);
            Console.ReadLine();
        }
    }
}
