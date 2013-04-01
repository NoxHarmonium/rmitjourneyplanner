"use strict"
RmitJourneyPlanner = window.RmitJourneyPlanner
class RmitJourneyPlanner::Data
    class @::DataSources 
        #Aliases
        Exceptions = RmitJourneyPlanner::Client::Exceptions;

        #--------------------------------------------#
        # Base Classes                               #
        #--------------------------------------------#    

        class @::DataSource        
            constructor: (@type) ->         

            #
            # Basic data sanity checks
            #
            CheckForError: (data) ->  
                if (!data?)
                    throw new Exceptions::NullDataException(this)

        class @::JsonDataSource extends @::DataSource 
        
            constructor: (@url) ->
                super("JSON")

            OnAjaxError: (jqXHR, textStatus, errorThrown) ->
                throw new Exceptions::DataAccessException(this, errorThrown, textStatus, jqXHR)
            

            #
            # Retrieves JSON data by using a HTTP POST command.
            #
            JSONPost: (url, data, callback) ->
                return $.ajax   { 
                                    url: @url,
                                    type: "POST",
                                    data: data,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: callback,
                                    error: @OnAjaxError 
                                }
    


            #
            # Basic data sanity checks
            #
            CheckForError: (data) ->  
               super data

            #
            # Makes a data request to the JSON datasource.
            #
            RequestData: (data={}, callback=->) ->
                @JSONPost(@url, data, callback)


        class @::JRPCDataSource extends @::JsonDataSource

            constructor: (@url, @methodName) ->      
                super @url

            #
            # Returns true if there is a JSON error object in the data object. Also displays the error
            # in an alert. Should be run on all data returned from JSON-RPC.
            #
            CheckForError: (data) ->            
                super data
                error = data.error;
                if error?
                    throw new Exceptions::JRPCException(this, error.name, error.message, error.stackTrace)       
              

            #
            # Makes a data request to the JRPC server.
            #
            RequestData: (data={},callback=->) ->        
        
                request = {}
                request.method = @methodName
                request.params = data
        
                request.id = 1
                request.jsonrpc = "2.0"
        
                super $.toJSON(request), (data, status, xhr) =>                
                    @CheckForError(data) 
                    callback(data)
           

        class @::JRPCBiDirDataSource extends @::JsonDataSource
            constructor: (@get, @set) ->
                if !(@get instanceof RmitJourneyPlanner::Data::DataSources::JRPCDataSource) || !(@set instanceof RmitJourneyPlanner::Data::DataSources::JRPCDataSource)
                    throw new Exceptions::InvalidElementException(this, "JRPCDataSource")

                Get: (data={}, callback=->) ->
                    @get.RequestData(data,callback)

                Set: (data={}, callback=->) ->
                    @set.RequestData(data,callback)


        #--------------------------------------------#
        # JRPC Data Sources                          #
        #--------------------------------------------#    

        class @::StopNameDataSource extends @::JRPCDataSource
            constructor: () ->
                super(
                    RmitJourneyPlanner::Client::Properties.JRPCUrl,
                    "QueryStops"
                    )

            RequestData: (data={}, callback=->) ->
                super data, callback

            Query: (stopName, callback) ->
                @RequestData({ "query": String(stopName) }, callback);


        class @::GetPropertyDataSource extends @::JRPCDataSource
            constructor: (@uuid) ->
                super(
                    RmitJourneyPlanner::Client::Properties.JRPCUrl,
                    "GetProperties"
                    )

            RequestData: (data={}, callback=->) ->
                super data, callback

            Query: (callback) ->
                @RequestData({ "journeyUuid": @uuid, "action": "Query" }, callback);
            


        class @::SetPropertyDataSource extends @::JRPCDataSource
            constructor: (@uuid) ->
                super(
                    RmitJourneyPlanner::Client::Properties.JRPCUrl,
                    "SetProperties"
                    )

            RequestData: (data={}, callback=->) ->
                super data, callback

            Query: (callback, propVals) ->
                @RequestData({ "journeyUuid": @uuid, "propVals": propVals }, callback);
            
            
        class @::JourneyPropertyDataSource extends @::JRPCBiDirDataSource
            constructor: (@uuid) ->
                super(
                    new DataSources::GetPropertyDataSource(@uuid),
                    new DataSources::SetPropertyDataSource(@uuid)
                    )

            Get: (callback) ->
                @get.Query(callback)

            Set: (callback, propVals) ->
                @set.Query(callback,propVals)

        class @::StatusDataSource extends @::JRPCDataSource
            constructor: (@uuid) ->
                super(
                    RmitJourneyPlanner::Client::Properties.JRPCUrl,
                    "GetStatus"
                    )

            RequestData: (data={}, callback=->) ->
                super data, callback

            Query: (callback, propVals = []) ->
                @RequestData({ "userKey": @uuid }, callback);

        class @::ResultDataSource extends @::JRPCDataSource
            constructor: (@uuid) ->
                super(
                    RmitJourneyPlanner::Client::Properties.JRPCUrl,
                    "GetResult"
                    )

            RequestData: (data={}, callback=->) ->
                super data, callback

            Query: (callback, propVals = []) ->
                @RequestData({ "userKey": @uuid }, callback);

        class @::DetailsDataSource extends @::JRPCDataSource
            constructor: (@uuid) ->
                super(
                    RmitJourneyPlanner::Client::Properties.JRPCUrl,
                    "GetDetails"
                    )

            RequestData: (data={}, callback=->) ->
                super data, callback

            Query: (journeyIndex, callback) ->
                @RequestData({ "userKey": @uuid, "journeyIndex": journeyIndex  }, callback);

        class @::SearchDataSource extends @::JRPCDataSource
            constructor: (@uuid) ->
                super(
                    RmitJourneyPlanner::Client::Properties.JRPCUrl,
                    "Search"
                    )

            RequestData: (data={}, callback=->) ->
                super data, callback

            Query: (callback, propVals = []) ->
                @RequestData({ "userKey": @uuid, "propVals": propVals }, callback);
    
        class @::DateFormatDataSource extends @::JRPCDataSource
            constructor: () ->
                super(
                    RmitJourneyPlanner::Client::Properties.JRPCUrl,
                    "GetServerDatetimeFormat"
                    )

            RequestData: (data={}, callback=->) ->
                super data, callback

            Query: (callback, propVals = []) ->
                @RequestData({ }, callback);

                