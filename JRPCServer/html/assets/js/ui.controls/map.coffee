###
****************************************************************
*
*   Class:       RmitJourneyPlanner.UI.MapControl
*   Description: Adds Google Maps functionality to the specified
*                div
*   
****************************************************************
###
"use strict"
class RmitJourneyPlanner::UI::Controls::MapControl extends RmitJourneyPlanner::UI::Controls::Control
            #Aliases
            Exceptions = RmitJourneyPlanner::Client::Exceptions;


             # Fields
            defaultBounds : new google.maps.LatLngBounds (
                (new google.maps.LatLng -36.536123, 143.074951)
                (new google.maps.LatLng -39.019184, 147.304688))  
            lines: []
            markers:[]
            map: null
    
            mapOptions : {
                bounds: @defaultBounds,
                types: [],
                center: new google.maps.LatLng(-37.810191, 144.962511),
                zoom: 11,
                mapTypeId: google.maps.MapTypeId.ROADMAP}     
           
            constructor: (@client, @element) ->
                super(@client,@element,["div"])   
                # Setup the Google Maps system.                
                if (!google? && google.maps?)
                    throw new Exceptions::MissingDependency(this,"Google Maps API")     

                @refreshMap();

                # Hook up jQuery to adjust map
                $(window).resize () =>        
                    @refreshMap();       
               
          
    
            #
            # Recalculates the height of the Google Maps canvas
            # (Needed on window resize due to some bugs)
            # 
            refreshMap: () ->
                @map = new google.maps.Map(@element[0],
                    @mapOptions)
                h = $(window).height()
                offsetTop = 60 # Calculate the top offset
                @element.css('height', (h - offsetTop))
                           

                
            addMarker: (marker) ->
                marker.setMap(@map)
                @markers.push(marker)

            clearMarkers: () ->
                for marker in @markers
                    market.setMap(null)
                @markers = []

            clearLines: () ->
                 for line in @lines
                    line.setMap(null)
                @lines = []

            addLine: (line) ->
                line.setMap(@map)
                @lines.push(line)
