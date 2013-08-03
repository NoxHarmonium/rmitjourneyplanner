
"use strict"
class window.RmitJourneyPlanner
    class @::Client
        #Aliases
        Exceptions = @::Exceptions

        constructor: () ->
            @global = new @Global(this)
            @ui = new @UI(this)
            @search = new @Search(this)
            @map = new RmitJourneyPlanner::UI::Controls::MapControl(@client, $("#map_canvas"))
            @dateFormatDataSource = new RmitJourneyPlanner::Data::DataSources::DateFormatDataSource()
            @datetimeFormatLoad = new $.Deferred();
            @dateFormatDataSource.Query (data) => 
                @dateTimeFormatString = data.result
                dtf = data.result.split("|")
                @dateFormatString = dtf[0]
                @timeFormatString = dtf[1]
                @datetimeFormatLoad.resolve(dtf[0],dtf[1])

        class @::Properties
            this.JRPCUrl="http://localhost:8000/simplejsonws.ashx"

        ###
        ****************************************************************
        *
        *   Class:       Rmitjourneyplanner.Client.Global
        *   Description: Initialises the web application and contains 
        *                functions used globally.
        *   
        ****************************************************************
        ###
        class @::Global
    
            # Constants
            @url_service:           "http://localhost:8000/simplejsonws.ashx"
            @error_scroll_offset:   -50
            @error_scroll_time:     500
        
            # Fields
            @googleEnabled:         false
            @serverReady:           false
            @catchJsErrors:         true
    
            #
            # Setup global variables and jQuery extensions
            #
            constructor: (@client) ->
                String.prototype.bool = () -> return (/^True$/i).test this;
                jQuery.fn.exists = () -> return this.length > 0
                jQuery.fn.disable = () -> return $(this).attr 'disabled', 'disabled'
                jQuery.fn.enable = () -> return $(this).removeAttr 'disabled'
        
                # Catch javascript errors and display a custom error message
                if @catchJsErrors
                    window.onerror = (message, url, lineNumber) ->   
                        if !!url      
                            url = 'Unknown'              
                        @showError "<strong>Javascript Error</strong> " + message + "\nLine Number: " + lineNumber + " Url: " + url 
        
                # Check if google is loaded
                if !window.google
                    @showWarning("<strong>Warning:</strong> Unable to load Google Maps scripts. Google Maps functionality will be disabled.");
                    @googleEnabled = false;
                else
                    @googleEnabled = true;        
    
            #
            # Shows a yellow warning message under the navbar
            # (Should only be used if it's a very serious warning) 
            #
            showWarning: (message) ->
                if !!do message.trim
                    $('#divWarning') hide
                    return       
                    
                $('#divWarning') show
                $('#divWarning') html nl2br message
    
            #
            # Shows a red warning message under the navbar
            # (Should only be used if it's a very serious error)
            #
            showError: (message)  ->
                $('#divError') show
                $('#txtErrorText') html nl2br message
        
                $('html, body') animate { 
                    "scrollTop": do $("#divError").offset.top + @error_scroll_offset 
                    }, @error_scroll_time   
    
            #
            # Converts new lines (\n and/or \r) to html breaks.
            #
            nl2br: (str, is_xhtml) ->
                breakTag = if (is_xhtml?) then '<br />' else '<br>'
                return (str + '').replace (/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g) , '$1' + breakTag + '$2'
    
            #
            # Global error function that AJAX calls. 
            #
            ajaxError: (event, request, settings) -> 
                showError "<strong>Error accessing server:</strong>\nError " + event.status + ": " + event.statusText
    
    
            #
            # Retrieves JSON data by using a HTTP POST command.
            #
            JSONPost: (url, data, callback) ->
                return $.ajax   { 
                                    url: url,
                                    type: "POST",
                                    data: data,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: callback,
                                    error: ajaxError 
                                }
    
            #
            # Function that calls a JSON RPC server function.
            #
            RPCCall: (method, params, callback) ->
                last_request = method
        
                request = {}
                request.method = method
                request.params = params
        
                request.id = 1
                request.jsonrpc = "2.0"
        
                return JSONPost url_service, ($.toJSON request), callback     
    

        ###
        ****************************************************************
        *
        *   Class:       Rmitjourneyplanner.Client.Maps
        *   Description: Contains the functionality to do with 
        *                manipulating the Google Maps object.
        *   
        ****************************************************************
        ###
        "use strict"
        class @::Maps
    
            # Fields
            @defaultBounds : new google.maps.LatLngBounds (
                (new google.maps.LatLng -36.536123, 143.074951)
                (new google.maps.LatLng -39.019184, 147.304688))  
    
    
            @mapOptions : {
                bounds: @defaultBounds,
                types: [],
                center: new google.maps.LatLng(-37.810191, 144.962511),
                zoom: 11,
                mapTypeId: google.maps.MapTypeId.ROADMAP}

            @googleMapsId :     "map_canvas"

            #
            # Setup the Google Maps system.
            #
            constuctor: (@client) ->
                if !googleEnabled
                    $('#map_canvas').text('Google maps requires a connection to the internet and is disabled.');
                    return false

                refreshMap();
                $(window).resize () ->        
                    refreshMap();       
    
            #
            # Recalculates the height of the Google Maps canvas
            # (Needed on window resize due to some bugs)
            # 
            refreshMap: () ->
                map = new google.maps.Map(document.getElementById(@googleMapsId),
                    mapOptions)
                h = $(window).height()
                offsetTop = 60 # Calculate the top offset
                $('#' + @googleMapsId).css('height', (h - offsetTop))  
 


        ###
        ****************************************************************
        *
        *   Class:       Rmitjourneyplanner.Client.Search
        *   Description: Contains the functionality to do with 
        *                manipulating the Google Maps object.
        *   
        ****************************************************************
        ###
        "use strict"
        class @::Search   
            #Fields
            userKey:               "abcdef";
            validation_success:    "Success";
            progress_callback_id:  null;
            poll_interval:         500; #ms


            constructor: (@client) ->
                @statusDataSource = new RmitJourneyPlanner::Data::DataSources::StatusDataSource(GlobalProperties.userKey)
                @resultDataSource = new RmitJourneyPlanner::Data::DataSources::ResultDataSource(GlobalProperties.userKey)
                @searchDatasource = new RmitJourneyPlanner::Data::DataSources::SearchDataSource(GlobalProperties.userKey)               
         
            Begin : () -> 
                do @startSearch                    
                @searchDatasource.Query (validationErrors) =>

                    if (validationErrors? && validationErrors.length > 0)
                        throw new Exceptions.DataAccessException(this,"The search method does not send and property values so there should not be any validation errors.")
                        
                    if @progress_callback_id != null
                        clearInterval(@progress_callback_id)
                    
                    if (!@poll_interval?)  
                         throw new Exceptions.ClientException(this,"Poll interval is undefined or null, this causes strange behavior.")

                    @progress_callback_id = setInterval( (=> do @progressCallback), @poll_interval)
                    @progressCallback()

                  
                      
        


            getResults : () ->
                @resultDataSource.Query (data) =>
                    @client.ui.showResults(data);

            progressCallback : () ->
              @statusDataSource.Query (data) =>
                            
                    d = data.result;
          
                    if d.status.toLowerCase() == "unknown"
                        return
        

                    #If validation succeded, open progress bar.
                    @client.ui.showLoadingDiv()
                    @client.ui.hideHelpDiv()
                    @client.ui.setLoadingDivProgress(d.progress, d.iteration, d.totalIterations)
                    @client.ui.setLoadingDivMode(d.status)

                    if d.status.toLowerCase() == "finished" 
                        do @endSearch                        
                        window.clearInterval(@progress_callback_id)
                        @progress_callback_id = null
                        do @getResults


            startSearch : () -> 
                @client.ui.disableSearch()


            endSearch : () ->
                @client.ui.enableSearch()


        ###
        ****************************************************************
        *
        *   Class:       Rmitjourneyplanner.Client.DataManager
        *   Description: This class handles data access and caching 
        *                of JSON data sources.
        *   
        ****************************************************************
        ###
        "use strict"
        class @::DataManager
            @data = {}
    
            constructor: (@client, @userKey) -> 
                console.log "DataManager: Constructor Called"

            loadDetails: (journeyIndex) ->
                 RPCCall 'Search', { "userKey": @userKey, "journeyIndex": journeyIndex }, (data) ->
                    console.log "DataManager: Data loaded"
                    @data = data;
        ###
        ****************************************************************
        *
        *   Class:       RmitJourneyPlanner.UI
        *   Description: This class handles the all the UI related 
        *                functions
        *   
        ****************************************************************
        ###
        "use strict"
        class @::UI
    
            # Fields
            dateTimeDivShown: false;
            loadingDivShown: false;
            helpDivShown:true;

            # Constants
            @strings = {
                strQueuedMessage: 'Your journey planning request is currently queued. Please wait....',
                strQueuedTitle: 'Queued...',
                strOptimisingMessage: 'The journey planning engine is currently calculating possible journeys. Please wait....',
                strOptimisingTitle: 'Optimising...',
                strFinishedMessage: 'The results of the optimisation are shown below....',
                strFinishedTitle: 'Results...',
                strUnknownTitle: 'Unknown Status...',

                strTransportImagePath: "assets/img/transportIcons/",
                strTransportImageExt: ".png"
                }

            constructor: (@client) ->
                 @detailsDatasource = new RmitJourneyPlanner::Data::DataSources::DetailsDataSource(GlobalProperties.userKey)
                

            # Functions    
            enableSearch: () ->
                $('div.subnav input').enable()
                $('div.subnav button').enable()
                $('div.subnav checkbox').enable()


            disableSearch: () ->
	            $('div.subnav input').disable()
	            $('div.subnav button').disable()
	            $('div.subnav checkbox').disable()


            setLoadingDivProgress: (progress, iteration, totalIterations) ->
                $('#divLoading .progressNumerator').text(iteration)
                $('#divLoading .progressDenominator').text(totalIterations)
                $('#mainProgressBar .bar').width(String(progress * 100.0) + "%")
    
            reset: () =>
                do @hideResultsDiv

            setLoadingDivMode: (mode) ->
                $('#divLoading .progressHelp').show()
                switch mode.toLowerCase()
                    when "queued" 
                        $('#divLoading .title').text(UI.strings.strQueuedTitle);
                        $('#divLoading .progressHelp').text(UI.strings.strQueuedMessage);
                        $('#divLoading .progressInfo').hide();

                    when "optimising" 
                        $('#divLoading .title').text(UI.strings.strOptimisingTitle);
                        $('#divLoading .progressHelp').text(UI.strings.strOptimisingMessage);
                        $('#divLoading .progressInfo').show(); 
                        $('#divLoading div.progress').show();  

                    when "finished" 
                        $('#divLoading .title').text(UI.strings.strFinishedTitle);
                        $('#divLoading .progressHelp').text(UI.strings.strFinishedMessage);
                        $('#divLoading .progressInfo').hide();
                        $('#divLoading div.progress').hide();  

                    else 
                        $('#divLoading .title').text(UI.strings.strUnknownTitle);
                        $('#divLoading .progressHelp').hide();
                        $('#divLoading .progressInfo').hide();


            showLoadingDiv: () ->
                if (!@loadingDivShown) 
                    $('#divLoading').show()
                    @loadingDivShown = true


            hideLoadingDiv: () -> 
                if (@loadingDivShown)
                    $('#divLoading').hide()
                    @loadingDivShown = false


            showHelpDiv: () -> 
                if (!@helpDivShown) 
                    $('#divHelp').show()
                    @helpDivShown = true

            hideHelpDiv: () ->
                if (@helpDivShown) 
                    $('#divHelp').hide()
                    @helpDivShown = false

            toggleDateTimeDiv: () ->    
                if (@dateTimeDivShown)        
                    @hideDateTimeDiv()    
                else        
                    @showDateTimeDiv()
                    
                @dateTimeDivShown  = !@dateTimeDivShown

            showDateTimeDiv: () ->    
                $('#btnMore i').removeClass('icon-chevron-down');
                $('#btnMore i').addClass('icon-chevron-up');     
                $('#divWhen'  ).slideDown 'slow', () ->
                    @dateTimeDivShown = true;

            hideDateTimeDiv: () ->
                if (dateTimeDivShown)
                    $('#btnMore i').addClass('icon-chevron-down')
                    $('#btnMore i').removeClass('icon-chevron-up')

                    $('#divWhen').slideUp 'slow', () ->
                        @dateTimeDivShown = false;

            hideResultsDiv: () ->
                $('#divJourneyList').hide()

            showResultsDiv: () ->
                $('#divJourneyList').show()

           

            selectJourney: (e) ->
                sender = $(e.target).parents('tr')
                $('.selectedJourney').removeClass('selectedJourney')
                sender.addClass('selectedJourney')
                @detailsDatasource.Query sender.index(), (data) =>
                    legs = data.result.Critter.Legs;
                    @client.map.ClearMarkers();
                    @client.map.ClearLines();

                    $('div.infoPanel').show();

                    for i in [0..legs.length-1]
                        leg = legs[i];

                        if (i == 0 || !(leg.Mode == "Walking" && legs[parseInt(i) - 1].Mode == "Walking")) 
                            @client.map.AddMarker(new google.maps.Marker({
                                position: new google.maps.LatLng(leg.StartLocation.Lat, leg.StartLocation.Long),
                                icon: UI.strings.strTransportImagePath + leg.Mode + UI.strings.strTransportImageExt
                            }));
                    

                        lineColour = null

                        switch leg.Mode
                            when "Train" then lineColour = "#0000FF";
                            when "Bus" then lineColour = "#B25800";
                            when "Tram" then lineColor = "#00FF00";
                            else lineColour = "#333333";
                    

                        @client.map.AddLine(new google.maps.Polyline({
                            path: [new google.maps.LatLng(leg.StartLocation.Lat, leg.StartLocation.Long),
                                    new google.maps.LatLng(leg.EndLocation.Lat, leg.EndLocation.Long)],
                            strokeColor: lineColour,
                            strokeOpacity: 1.0,
                            strokeWeight: 2
                        }));
        
                        if (parseInt(i) == legs.length - 1)
                            #Draw finish marker
                            @client.map.AddMarker(new google.maps.Marker({
                                position: new google.maps.LatLng(leg.EndLocation.Lat, leg.EndLocation.Long),
                                icon: UI.strings.strTransportImagePath + "Finish" + UI.strings.strTransportImageExt
                            }));
                    
                    

            showResults: (data) ->
                results = data.result
                tableSummary = $('#divJourneyList table')
                do tableSummary.empty
                i = 0
                for journey in results
                    journey = journey.Critter
                    trSummary = $('<tr></tr>')
                    tdTitle = $('<td></td>')
                    tdTitle.text('Journey ' + i++)
                    tdTime = $('<td></td>')
                    d = new Date(journey.Fitness.TotalJourneyMinutes * 60 * 1000)
                    tdTime.text(d.getUTCHours() + 'h ' + d.getUTCMinutes() + 'm')
                    tdLegs = $('<td class="journeyModes"></td>')
                    j = 0
                    for leg in journey.Legs               
                        if (j == 0 || !(leg.Mode == "Walking" && journey.Legs[parseInt(j) - 1].Mode == "Walking"))                    
                            imgLeg = $('<img class="miniIcon"/>')
                            imgLeg.attr('src', UI.strings.strTransportImagePath + leg.Mode + UI.strings.strTransportImageExt)
                            tdLegs.append(imgLeg)
                        j++
            
                    trSummary.data('jLegs', journey.Legs)
                    trSummary.click((e)=> @selectJourney(e))
            
                    trSummary.append(tdTitle)
                    trSummary.append(tdTime)
                    trSummary.append(tdLegs)
                    tableSummary.append(trSummary)
            
                @showResultsDiv()

        ###
        ****************************************************************
        *
        *   Class:       Rmitjourneyplanner.Client.Exceptions
        *   Description: Contains the exceptions used in this 
        *                application.
        *   
        ****************************************************************
        ###
        "use strict"
        class @::Exceptions
            class @::ClientException

                constructor: (@source, @message) ->

                toString: () ->
                    message = "Exception: " + @message + " Source: "
                    name = "unknown"
                    if (@source.constructor?)
                       name = @source.constructor.name;                   
                    return message + name

            class @::NullDataException extends @::ClientException
                this.message = "The returned data was null."

                constructor: (source) ->
                    super source, NullDataException.message

             
         
            class @::NullElementException extends @::ClientException
                this.message = "The supplied element is undefined or null."

                constructor: (source) ->
                    super source, NullElementException.message



            class @::InvalidElementException extends @::ClientException
                this.message = "This UI control does not support the type of the supplied element."

                constructor: (source, @requiredElement = "") ->
                    super source, InvalidElementException.message


            class @::MultipleElementException extends @::ClientException
                this.message = "This UI control does not support wrapping multiple elements."

                constructor: (source, @requiredElement = "") ->
                    super source, MultipleElementException.message

            
            class @::DataAccessException extends @::ClientException
                this.message = "There was an error accessing the data source."

                constructor: (source, @errorDetails = "", @status = "", @requestObject = null) ->
                    super source, DataAccessException.message
                

            class @::JRPCException extends @::ClientException
                this.message = "There was an error executing the remote method."

                constructor: (source, @name, @message="", @stackTrace=""	) ->
                    super source, JRPCException.message


            class @::MissingDependencyException extends @::ClientException
                this.message = "This control is missing a required dependency. Check that the required dependency is included in your web application. Dependency: "

                constructor: (source, @dependency="") ->
                    super source, MissingDependencyException.message + @dependency


