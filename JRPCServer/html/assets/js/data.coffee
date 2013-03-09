"use strict"
RmitJourneyPlanner = window.RmitJourneyPlanner
class RmitJourneyPlanner::Client::Data
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
            