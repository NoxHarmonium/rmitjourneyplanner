if (googleEnabled) {

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
    var chart;
    var nodeCache = new Array();
    var points;
    var pointsLoaded;
    var pointsTotal;
    var legsLoaded = 0;
    var legsTotal = 0;

    ///
    /// Custom functions
    ///    
    refreshMap = function() {
        map = new google.maps.Map(document.getElementById("map_canvas"),
            mapOptions);
		var h = $(window).height(),
        offsetTop = 60; // Calculate the top offset
		$('#map_canvas').css('height', (h - offsetTop));
    };


    

    ///
    /// Event triggers
    ///
    $(document).ready(function() {
        window.mapManager = new MapManager();
		refreshMap();
		//Enable resize of the left panel.
		//$('#divInfoPanel').resizable( {
		//stop: function(e, ui) {
        //    refreshMap();
        //}
		//});
    });
	
	$(window).resize(function() {        
		refreshMap();		
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

        return RPCCall("GetNodeData", { "id": id }, function(data) {
            if (CheckForError(data)) {
                return;
            }

            nodeCache[id] = data.result;
            callback(data.result, customValue);


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
    jQuery.fn.sortElements = (function() {

        var sort = [].sort;

        return function(comparator, getSortable) {

            getSortable = getSortable || function() { return this; };

            var placements = this.map(function() {

                var sortElement = getSortable.call(this),
                    parentNode = sortElement.parentNode,
                // Since the element itself will change position, we have
                // to have some way of storing its original position in
                // the DOM. The easiest way is to have a 'flag' node:
                    nextSibling = parentNode.insertBefore(
                        document.createTextNode(''),
                        sortElement.nextSibling
                );

                return function() {

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

            return sort.call(this, comparator).each(function(i) {
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
        
          
        this.drawPolyLines = function() {
            if (mapPath) {
                mapPath.setMap(null);
            }
            points = new Array();
            pointsTotal = _currentMember.Route.length;
            pointsLoaded = 0;


            for (var r in _currentMember.Route) {

                GetNodeData(_currentMember.Route[r],
                    function(data, x) {
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
        this.loadRun = function(journeyUuid, runUuid) {
            _iteration = 0;
            _journeyUuid = journeyUuid;
            _runUuid = runUuid;

            this.clear();
            this.loadIteration(_iteration);

        };

        // Loads a specific member from the loaded run.
        this.loadMember = function(index) {

            _currentMember = _currentData.population[index].Critter;
            console.log("Loading member: " + index);
            window.mapManager.reDraw();
        };

        // Clears all generated content.
        this.clear = function() {

            $('#divJViewerL').empty();
        };


        // Generates the dynamic content from the data.
        this.draw = function() {

            legsLoaded = 0;
            legsTotal = _currentMember.Legs.length;
            for (var l in _currentMember.Legs) {

                this.generateLegDiv(_currentMember.Legs[l], l);

            }
            this.drawPolyLines();


        };

        // Clears all generated content and then generates.
        this.reDraw = function() {
            this.clear();
            this.draw();
        };


        //Load a specific iteration from the loaded run file.
        this.loadIteration = function(iterationNum) {

            if (iterationNum)
                _iteration = iterationNum;

            RPCCall("GetIteration", { "journeyUuid": _journeyUuid, "runUuid": _runUuid, "iteration": _iteration }, function(data) {
                if (CheckForError(data)) {
                    return;
                }
                _currentData = eval('(' + data.result + ')');

                $('#selPopulation').empty();

                // Empty element for ease of use.
                var opt = jQuery(document.createElement('option'));
                $('#selPopulation').append(opt);
                for (var i in _currentData.population) {
                    opt = jQuery(document.createElement('option'));
                    opt.text(i);
                    opt.val(i);
                    $('#selPopulation').append(opt);
                }

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

} else {
    $('#map_canvas').text('Google maps requires a connection to the internet and is disabled.');
}