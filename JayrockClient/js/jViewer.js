if (googleEnabled)
{

    ///
    /// Fields
    ///
    var defaultBounds = new google.maps.LatLngBounds(
			  new google.maps.LatLng(-36.536123, 143.074951),
			  new google.maps.LatLng(-39.019184, 147.304688));

    var mapOptions = {
        bounds: defaultBounds,
        types: [],
        center: new google.maps.LatLng(-37.810191, 144.962511),
        zoom: 11,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    
    
    var map;


    ///
    /// Custom functions
    ///    
    refreshMap = function()
    {
	    map = new google.maps.Map(document.getElementById("map_canvas"),
            mapOptions);
	};
    
    
   
   function loadJourneyRuns() {
       if (selectedJourneyUuid == undefined
            || selectedJourneyUuid == ""
                || selectedJourneyUuid == null) {

           $('#selJourneyRuns').attr('disabled', 'disabled');
           $('#selPopulation').attr('disabled', 'disabled');
           return;

       } else {
           $('#selJourneyRuns').removeAttr('disabled');
           
       }
       
       RPCCall("GetRuns", { "uuid": selectedJourneyUuid }, function (data) {
           if (CheckForError(data)) {
               return;
           }
           
           $('#selJourneyRuns').empty();
           
           for (var i in data.result) {

               var runUuid = data.result[i];
               var opt = jQuery(document.createElement('option'));
               opt.text(runUuid);
               $('#selJourneyRuns').append(opt);


           }

       });
       
       
   }
   
    ///
    /// Event triggers
    ///
   $(document).ready(function () {
       window.mapManager = new MapManager();
   });

   $('#selJourneyRuns').change(function () {
       if (!$(this).attr('disabled') && $(this).text() != "") {
           $('#selPopulation').removeAttr('disabled');
           window.mapManager.loadRun(selectedJourneyUuid, $(this).find(':selected').text());
       } else {
           $('#selPopulation').attr('disabled', 'disabled');
       }


   });

   $('#selPopulation').change(function () {
       var member = $(this).find(':selected').text();
       window.mapManager.loadMember(member);

   });


    ///
    /// Main Class
    ///
    
    function MapManager() {
        var _iteration = 0;
        var _journeyUuid;
        var _runUuid;
        var _currentData;
        var _currentMember;

        this.loadRun = function (journeyUuid, runUuid) {
            _iteration = 0;
            _journeyUuid = journeyUuid;
            _runUuid = runUuid;

            this.clear();
            this.loadIteration(_iteration);

        };

        this.loadMember = function (index) {
   
            _currentMember = _currentData.population[index];
            console.log("Loading member: " + index);
            this.reDraw();
        };

         this.clear = function() {
            

        };

        this.reDraw = function () {
            this.clear();
            


        };

        this.loadIteration = function (iterationNum) {
          
            
            _iteration = iterationNum;

            RPCCall("GetIteration", { "journeyUuid": _journeyUuid, "runUuid": _runUuid, "iteration": _iteration }, function (data) {
                if (CheckForError(data)) {
                    return;
                }
                _currentData = eval('(' + data.result + ')');

                $('#selPopulation').empty();
                for (var i in _currentData.population) {

                    var opt = jQuery(document.createElement('option'));
                    opt.text(i);
                    $('#selPopulation').append(opt);
                }


            });

        };

    }

}
else
{
	$('#map_canvas').text('Google maps requires a connection to the internet and is disabled.');
}
