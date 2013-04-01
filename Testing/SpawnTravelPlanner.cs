// -----------------------------------------------------------------------
// <copyright file="SpawnTravelPlanner.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SpawnTravelPlanner
    {
        public SpawnTravelPlanner()
        {
            INetworkDataProvider metlinkProvider = new MetlinkDataProvider();
            var testRoutes = new[,]
                {
                    // Long inter-suburban routes
                    {
                        new MetlinkNode(19965, metlinkProvider),
                        new MetlinkNode(19879, metlinkProvider)
                    }, // Coburg - Ringwood (1:29) + 19 = (1:48)
                    {
                        new MetlinkNode(20005, metlinkProvider),
                        new MetlinkNode(19855, metlinkProvider)
                    }, // Epping - Frankston (1:56) + 0 = (1:56) 
                    {
                        new MetlinkNode(19990, metlinkProvider),
                        new MetlinkNode(19921, metlinkProvider)
                    }, // Hurstbridge - Werribee (1:45) + 17 = (2:03)
                    {
                        new MetlinkNode(19844, metlinkProvider),
                        new MetlinkNode(20000, metlinkProvider)
                    }, // Belgrave - Watergardens (1:50) + 4 = (1:54)
                    {
                        new MetlinkNode(19886, metlinkProvider),
                        new MetlinkNode(40221, metlinkProvider)
                    }, // Cranbourne - Craigieburn (1:47) + 11 = (1:58)
                    
                    //average = 115.8 = 6948
                    // Lateral inter-suburban routes.
                    {
                        new MetlinkNode(19965, metlinkProvider),
                        new MetlinkNode(19935, metlinkProvider)
                    }, // Coburg - Heidelberg (0:37) + 3 = (0:40)
                    {
                        new MetlinkNode(20005, metlinkProvider),
                        new MetlinkNode(19855, metlinkProvider)
                    }, // Epping - Frankston (1:56) + 0 = (1:56)
                    {
                        new MetlinkNode(19990, metlinkProvider),
                        new MetlinkNode(19246, metlinkProvider)
                    },
                    // East Kew Terminus - Box Hill (r202) (1:12) + 17 = (1:29)
                    {
                        new MetlinkNode(12018, metlinkProvider),
                        new MetlinkNode(18475, metlinkProvider)
                    }, // Yarravile - Highpoint (r223) (00:28) + 15 = (00:43)
                    {
                        new MetlinkNode(4808, metlinkProvider),
                        new MetlinkNode(19649, metlinkProvider)
                    }, // Greensborough - Boxhill (00:57) + 4 = (01:01)

                    //Average = 69.8 = 4188
                       
                    // Inner-city routes.
                    {
                        new MetlinkNode(19843, metlinkProvider),
                        new MetlinkNode(22180, metlinkProvider)
                    }, // Parliament - Southern Cross (00:06) + 0 = (00:06)
                    {
                        new MetlinkNode(17882, metlinkProvider),
                        new MetlinkNode(19841, metlinkProvider)
                    },
                    // 9-Spring St/Bourke St  - Flagstaff (00:07) + 1 = (00:08)
                    {
                        new MetlinkNode(19489, metlinkProvider),
                        new MetlinkNode(19973, metlinkProvider)
                    }, // Melbourne Uni - North Melbourne  (00:21) + 2 = (00:23)
                    {
                        new MetlinkNode(18034, metlinkProvider),
                        new MetlinkNode(17901, metlinkProvider)
                    },
                    // 2-King St/La Trobe St - Melbourne Town Hall/Collins St (00:05) + 2 = (00:07)
                    {
                        new MetlinkNode(18450, metlinkProvider),
                        new MetlinkNode(19594, metlinkProvider)
                    },
                    // Casino - Royal Childrens Hospital/Flemington Rd (00:16) + 3 = (00:19)

                    //average = 11.4 = 684


                    // Commuter Routes
                    {
                        new MetlinkNode(19965, metlinkProvider),
                        new MetlinkNode(19842, metlinkProvider)
                    }, // Coburg - Melbourne Central (00:20) + 16 = (00:36)
                    {
                        new MetlinkNode(19876, metlinkProvider),
                        new MetlinkNode(19841, metlinkProvider)
                    }, // Lilydale  - Flagstaff (00:53) + 6 = (00:59)
                    {
                        new MetlinkNode(19489, metlinkProvider),
                        new MetlinkNode(19921, metlinkProvider)
                    }, // Werribee - North Melbourne (00:56) + 14 = (1:10)
                    {
                        new MetlinkNode(20005, metlinkProvider),
                        new MetlinkNode(19843, metlinkProvider)
                    }, // Epping - Parliament (00:54) + 0 (00:54)
                    {
                        new MetlinkNode(19855, metlinkProvider),
                        new MetlinkNode(19854, metlinkProvider)
                    } // Frankston - Flinders St (00:57) + 6 (01:03)

                    //average = 56.4 = 3384

                };

            

            for (int i = 0; i < testRoutes.GetLength(0); i++)
            {
                //8:00 AM 7/05/2012
                string request =
                @"http://jp.ptv.vic.gov.au/ptv/XSLT_TRIP_REQUEST2?sessionID=0&language=en&requestID=0&command=&itdLPxx_view=&execInst=&ptOptionsActive=1&itOptionsActive=1&useProxFootSearch=1&anySigWhenPerfectNoOtherMatches=1&type_origin=any&nameState_origin=empty&type_destination=any&nameState_destination=empty&nameDefaultText_origin=Enter+Address&execIdentifiedLoc_origin=1&execStopList_origin=0&nameDefaultText_destination=Enter+Address&execIdentifiedLoc_destination=1&execStopList_destination=0&ttpFrom=20060701&ttpTo=20070630&anyObjFilter_origin=2&name_origin=<originStation>&anyObjFilter_destination=2&name_destination=<destStation>&itdTripDateTimeDepArr=dep&itdDateDay=7&itdDateYearMonth=201205&itdTimeHour=8&itdTimeMinute=00&itdTimeAMPM=am";
                request = request.Replace("<originStation>", testRoutes[i, 0].StopSpecName + " Railway Station");
                request = request.Replace("<destStation>", testRoutes[i, 1].StopSpecName + " Railway Station");
                Thread.Sleep(500);
                System.Diagnostics.Process.Start(request);
                
            }


        }

        //<destStation>
        //<originStation>
        //

    }
}
