using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

using RmitJourneyPlanner.CoreLibraries.DataProviders;
using RmitJourneyPlanner.CoreLibraries.Caching;
using RmitJourneyPlanner.CoreLibraries.Positioning;
using RmitJourneyPlanner.CoreLibraries.Types;
using RmitJourneyPlanner.CoreLibraries.DataAccess;

namespace DataManager
{
    public partial class frmYarraTrams : Form
    {
        private BackgroundWorker worker;
        
        public frmYarraTrams()
        {
            InitializeComponent();
            RmitJourneyPlanner.CoreLibraries.DataAccess.ConnectionInfo.Proxy = 
                new System.Net.WebProxy("http://aproxy.rmit.edu.au:8080",false,null,new NetworkCredential("s3229159","MuchosRowlies1");
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            cmbActions.Enabled = false;
            btnStop.Enabled = true;

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync(cmbActions.SelectedIndex);

            
                
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRun.Enabled = true;
            cmbActions.Enabled = true;
            btnStop.Enabled = false;
            log("Action finished");
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            log((string)e.UserState);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if ((int)e.Argument == 0)
                {
                    //Scrape stop data
                    worker.ReportProgress(0, "Getting client UUID...");
                    TramTrackerAPI api = new TramTrackerAPI();
                    worker.ReportProgress(0, "UUID is now " + api.Guid);
                    worker.ReportProgress(0, "Init cache...");
                    LocationCache cache = new LocationCache("YarraTrams");
                    cache.InitializeCache();

                    int count = 0;

                    worker.ReportProgress(0, "Getting routes...");
                    DataSet mainRoutes = api.GetMainRoutes();

                    foreach (DataRow routeRow in mainRoutes.Tables[0].Rows)
                    {
                        string route = routeRow[0].ToString();
                        worker.ReportProgress(0, "Scraping route: " + route + "(up) ...");
                        DataSet stops = api.GetListOfStopsByRouteNoAndDirection(route, true);
                        foreach (DataRow stop in stops.Tables[0].Rows)
                        {
                            cache.AddCacheEntry(stop["TID"].ToString(), new Location(Convert.ToDouble(stop["Latitude"]), Convert.ToDouble(stop["Longitude"])));
                            count++;
                        }

                        worker.ReportProgress(0, "Scraping route: " + route + "(down) ...");

                        stops = api.GetListOfStopsByRouteNoAndDirection(route, false);
                        foreach (DataRow stop in stops.Tables[0].Rows)
                        {
                            cache.AddCacheEntry(stop["TID"].ToString(), new Location(Convert.ToDouble(stop["Latitude"]), Convert.ToDouble(stop["Longitude"])));
                            count++;
                        }

                    }

                    worker.ReportProgress(0, count.ToString() + " stops cached!");



                }
                else if ((int)e.Argument == 1)
                {
                    worker.ReportProgress(0, "Load dataprovider...");
                    TramNetworkProvider provider = new TramNetworkProvider();

                    worker.ReportProgress(0, "Searching...");
                    //TODO: Set radius to 5? certain data causes bug!
                    //List<INetworkNode> stops = provider.GetNodesAtLocation(new Location("13 Liverpool St, Coburg, Australia"), 1.0);
                    List<INetworkNode> stops = provider.GetNodesAtLocation(new Location(-37.755397, 144.963888), 1.0);
                    
                    
                    foreach (TramStop stop in stops)
                    {
                        worker.ReportProgress(0, "Stop found: " + stop.StopName + " (" + stop.SuburbName + ")");
                    }

                }
                else if ((int)e.Argument == 2)
                {
                    worker.ReportProgress(0, "Initilizing node cache...");
                    NodeCache<TramStop> nCache = new NodeCache<TramStop>("YarraTrams");
                    nCache.InitializeCache();

                }
                else if ((int)e.Argument == 3)
                {
                    worker.ReportProgress(0, @"Finding distance between 12-14-1216 @ 24/11/2011 1:23 PM");
                    TramNetworkProvider provider = new TramNetworkProvider();
                    List<Arc> arcs = provider.GetDistanceBetweenNodes(new TramStop("1214",provider), new TramStop("1216",provider), DateTime.Parse("11/24/2011 1:23 PM"));
                    foreach (Arc arc in arcs)
                    {
                        worker.ReportProgress(0,
                                            String.Format("Arc: [RouteID: {0}, Departure Time: {1}, Total Time: {2}]",
                                            arc.RouteId,
                                            arc.DepartureTime,
                                            arc.Time));
                    }

                }
            }
            catch (Exception ex)
            {
                worker.ReportProgress(0, ex.Message);
                worker.ReportProgress(0, ex.StackTrace);
                
            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            worker.CancelAsync();
            log("Attempting to cancel action...");
           
        }

        

        private void log(string message)
        {
            bool first = true;
            string[] lines = message.Split('\n');
            foreach (string line in lines)
            {
                if (first)
                {
                    lstLog.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToLongTimeString(), line }));
                    first = false;
                }
                else
                {
                    lstLog.Items.Add(new ListViewItem(new string[] {"...", line }));
                }
            }
            lstLog.EnsureVisible(lstLog.Items.Count - 1);

        }

        private void frmYarraTrams_Load(object sender, EventArgs e)
        {
            cmbActions.SelectedIndex = 0;
        }
    }
}
