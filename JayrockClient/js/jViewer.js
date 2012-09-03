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
    var mapPath;
    var nodeCache = new Array();
    var points;
    var pointsLoaded;
    var pointsTotal;
    var legsLoaded = 0;
    var legsTotal = 0;

    ///
    /// Custom functions
    ///    
    refreshMap = function()
    {
	    map = new google.maps.Map(document.getElementById("map_canvas"),
            mapOptions);
	};



	function refreshViewerSlider() {

	    if (selectedJourneyUuid) {
	        RPCCall("GetTotalIterations", { "journeyUuid": selectedJourneyUuid }, function (data) {
	            if (CheckForError(data)) {
	                return;
	            }
	            var iterations = data.result;
	            for (var i = 0; i < iterations; i++ ) {
	                var opt = jQuery(document.createElement('option'));
	                opt.text(i);
	                $('#selIterations').append(opt);
	            }


	        });
	    }      
	    
        
        //TODO: Fix slider
        /*
        if (selectedJourneyUuid) {
	        RPCCall("GetTotalIterations", { "journeyUuid": selectedJourneyUuid }, function(data) {
	            if (CheckForError(data)) {
	                return;
	            }
	            var iterations = data.result;

	            $("#divSlider").slider({
	                range: "max",
	                min: 1,
	                max: iterations,
	                value: 2,
	                slide: function(event, ui) {
	                    //$("#amount").val(ui.value);
	                }
	            });
	            //$("#amount").val($("#slider").slider("value"));

	        });
	    }       
        */
       

	}

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
           
           //Add empty element for easy selection
           var opt = jQuery(document.createElement('option'));
           $('#selJourneyRuns').append(opt);

           for (var i in data.result) {

               var runUuid = data.result[i];
               opt = jQuery(document.createElement('option'));
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

   $('#selIterations').change(function () {
        window.mapManager.loadIteration(parseInt($(this).find(':selected').text()));
   });

   $('#selPopulation').change(function () {
       var member = $(this).find(':selected').text();
       window.mapManager.loadMember(member);

   });

   $('#selGraphObjectives').change(function () {
       var members = $(this).find(':selected');
       if (members.length > 1) {

           $('divGraph').empty();
           var dataSet = new Array();

           var labelA = $(members[0]).text();
           var labelB = $(members[1]).text();

           dataSet.push([labelA, labelB]);
           var _currentData = window.mapManager.getData();

           for (var i in _currentData.population) {
               dataSet.push([_currentData.population[i].Critter.Fitness[labelA],
                            _currentData.population[i].Critter.Fitness[labelB]]);


               opt = jQuery(document.createElement('option'));
               opt.text(i);
               $('#selPopulation').append(opt);
           }


           var data = google.visualization.arrayToDataTable(dataSet);

           var options = {
               title: 'Population Analysis',
               hAxis: { title: labelA, minValue: 0, maxValue: 1 },
               vAxis: { title: labelB , minValue: 0, maxValue: 1 },
               legend: 'none'
           };

           var chart = new google.visualization.ScatterChart(document.getElementById('divGraph'));
           chart.draw(data, options);

       }

   });

   ///
   /// Helper functions
   ///

   function GetNodeData(id, callback, customValue) {
       var cacheData = nodeCache[id];
       if (cacheData) {
           callback(cacheData, customValue);
            return null;
       }

       return RPCCall("GetNodeData", { "id": id }, function (data) {
           if (CheckForError(data)) {
               return;
           }

           nodeCache[id] = data.result;
           callback(data.result,customValue);
           

       });

   }

   function parseTimeSpan(timeSpan) {

       var split = String(timeSpan).split(":");
       var hours = split[0];
       var minutes = split[1];
       var seconds = split[2];

       if (!seconds) {
           seconds = 0;
       }
       if (!minutes) {
           minutes = 0;
       }
       if (!hours) {
           hours = 0;
       }

       return { "hours": hours, "minutes": minutes, "seconds": seconds };

   }
    
    function trimSeconds(timeString) {
        var split = String(timeString).split(" ");


        return split[0].substring(0, split[0].length - 3) + " " + split[1];

    }

    


    /**
    * jQuery.fn.sortElements
    * --------------
    * @param Function comparator:
    *   Exactly the same behaviour as [1,2,3].sort(comparator)
    *   
    * @param Function getSortable
    *   A function that should return the element that is
    *   to be sorted. The comparator will run on the
    *   current collection, but you may want the actual
    *   resulting sort to occur on a parent or another
    *   associated element.
    *   
    *   E.g. $('td').sortElements(comparator, function(){
    *      return this.parentNode; 
    *   })
    *   
    *   The <td>'s parent (<tr>) will be sorted instead
    *   of the <td> itself.
    */
    jQuery.fn.sortElements = (function () {

        var sort = [].sort;

        return function (comparator, getSortable) {

            getSortable = getSortable || function () { return this; };

            var placements = this.map(function () {

                var sortElement = getSortable.call(this),
                parentNode = sortElement.parentNode,

                // Since the element itself will change position, we have
                // to have some way of storing its original position in
                // the DOM. The easiest way is to have a 'flag' node:
                nextSibling = parentNode.insertBefore(
                    document.createTextNode(''),
                    sortElement.nextSibling
                );

                return function () {

                    if (parentNode === this) {
                        throw new Error(
                        "You can't sort elements if any one is a descendant of another."
                    );
                    }

                    // Insert before flag:
                    parentNode.insertBefore(this, nextSibling);
                    // Remove flag:
                    parentNode.removeChild(nextSibling);

                };

            });

            return sort.call(this, comparator).each(function (i) {
                placements[i].call(getSortable.call(this));
            });

        };

    })();



    ///
    /// Main Class
    ///
    function MapManager() {
        var _iteration = 0;
        var _journeyUuid;
        var _runUuid;
        var _currentData;
        var _currentMember;

        this.getData = function() {
            return _currentData;

        };
        this.generateLegDiv = function (leg, index) {

            //"Legs":[{"Start":19965,"End":19842,"Mode":"Train","TotalTime":"00:45:00","Route":"111","DepartTime":"2012-09-03T14:00:50.5030000+10:00"}]


            var baseDiv = $(
            '<div class="well well-jl"> ' +
                '<ul> ' +
                    '<li class="liJLTop"> ' +
                        '<span class="divJLTime topSpan">6:45 AM</span> ' +
                        '<img class="imgTransportIcon topSpan" src="img/transportIcons/Tram.png"> ' +
                        '<span class="divStationName topSpan">Swanston St Stop 123</span> ' +
                    '</li> ' +
                    '<li class="liJLMiddle"> ' +
                        '<span class="divJLTime midSpan">   </span> ' +
                        '<img src="img/downArrow.png"  /> <span class="divTotTime">Walk (20 Mins)</span> ' +
                    '</li> ' +
                    '<li class="liJLBottom botSpan"> ' +
                        '<span class="divJLTime botSpan">7:05 AM</span> ' +
                        '<img class="imgTransportIcon botSpan" src="img/transportIcons/Train.png"> ' +
                        '<span class="divStationName botSpan">Melbourne Central </span> ' +
                    '</li> ' +
                '</ul> ' +
            '</div>');


            GetNodeData(leg.Start,
                function (originData) {
                    GetNodeData(leg.End,
                        function (destData) {
                            baseDiv.find('.divStationName.topSpan').text(originData.name);
                            baseDiv.find('.divStationName.topSpan').addClass(originData.mode.toLowerCase());
                            baseDiv.find('.divStationName.botSpan').text(destData.name);
                            baseDiv.find('.divStationName.botSpan').addClass(destData.mode.toLowerCase());


                            var departTime = new Date(leg.DepartTime);

                            baseDiv.find('.divJLTime.topSpan').text(trimSeconds(departTime.toLocaleTimeString()));
                            var timeSpan = parseTimeSpan(leg.TotalTime);

                            var arriveTime = new Date(departTime.getTime() + ((timeSpan.hours * 60) + timeSpan.minutes) * 60000);



                            baseDiv.find('.divTotTime').text(leg.Mode + ' (' + leg.Route + ') ' + leg.TotalTime.split('.')[0]);

                            baseDiv.find('.divJLTime.botSpan').text(trimSeconds(arriveTime.toLocaleTimeString()));


                            baseDiv.find('img.topSpan').attr('src', 'img/transportIcons/' + originData.mode + '.png');
                            baseDiv.find('img.botSpan').attr('src', 'img/transportIcons/' + destData.mode + '.png');

                            baseDiv.attr('data-order', index);

                            $('#divJViewerL').append(baseDiv);

                            legsLoaded++;

                            if (legsLoaded == legsTotal) {
                                $('.well-jl').sortElements(function (a, b) {

                                    var contentA = parseInt($(a).attr('data-order'));
                                    var contentB = parseInt($(b).attr('data-order'));
                                    return (contentA < contentB) ? -1 : (contentA > contentB) ? 1 : 0;
                                });
                            }


                        });



                });





            /*
           
            */





        };

        this.drawPolyLines = function () {
            if (mapPath) {
                mapPath.setMap(null);
            }
            points = new Array();
            pointsTotal = _currentMember.Route.length;
            pointsLoaded = 0;


            for (var r in _currentMember.Route) {

                GetNodeData(_currentMember.Route[r],
                    function (data, x) {
                        points[x] = new google.maps.LatLng(data.latitude, data.longitude);
                        pointsLoaded++;
                        if (pointsLoaded == pointsTotal) {
                            mapPath = new google.maps.Polyline({
                                path: points,
                                strokeColor: "#FF0000",
                                strokeOpacity: 1.0,
                                strokeWeight: 1
                            });

                            mapPath.setMap(map);
                        }
                    }, r);



            }

        };
        
        //Loads a run file
        this.loadRun = function (journeyUuid, runUuid) {
            _iteration = 0;
            _journeyUuid = journeyUuid;
            _runUuid = runUuid;

            this.clear();
            this.loadIteration(_iteration);

        };

        // Loads a specific member from the loaded run.
        this.loadMember = function (index) {
   
            _currentMember = _currentData.population[index].Critter;
            console.log("Loading member: " + index);
            window.mapManager.reDraw();
        };

        // Clears all generated content.
        this.clear = function () {

            $('#divJViewerL').empty();
        };


        // Generates the dynamic content from the data.
        this.draw = function () {

            legsLoaded = 0;
            legsTotal = _currentMember.Legs.length;
            for (var l in _currentMember.Legs) {

                this.generateLegDiv(_currentMember.Legs[l], l);

            }
            this.drawPolyLines();




        };
        
        // Clears all generated content and then generates.
        this.reDraw = function () {
            this.clear();
            this.draw();
        };
        
        
        //Load a specific iteration from the loaded run file.
        this.loadIteration = function (iterationNum) {


            _iteration = iterationNum;

            RPCCall("GetIteration", { "journeyUuid": _journeyUuid, "runUuid": _runUuid, "iteration": _iteration }, function (data) {
                if (CheckForError(data)) {
                    return;
                }
                _currentData = eval('(' + data.result + ')');

                $('#selPopulation').empty();

                // Empty element for ease of use.
                var opt = jQuery(document.createElement('option'));
                $('#selPopulation').append(opt);



                $('#selGraphObjectives').empty();
                for (var i in _currentData.population[0].Critter.Fitness) {
                    opt = jQuery(document.createElement('option'));
                    var keys = Object.keys(_currentData.population[0].Critter.Fitness);
                    
                    opt.text(i);
                    $('#selGraphObjectives').append(opt);
                }







            });

        };

    }

}
else
{
	$('#map_canvas').text('Google maps requires a connection to the internet and is disabled.');
}
