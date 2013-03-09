###
****************************************************************
*
*   Class:       Application
*   Description: Executes on DOM ready and initialises the web
*                application
*   
****************************************************************
###
class Application
    
    constructor() ->
        
    start: () ->
        client = new RmitJourneyPlanner::Client()
        datasource = new RmitJourneyPlanner::Client::Data::DataSources::StopNameDataSource()
        $("input.locationInput").each ->
        autocomplete = new RmitJourneyPlanner::Client::UI::Controls::AutocompleteControl(client,                                                                   $(this),datasource);


$(window).ready ->
    application = new Application()
    application.start()