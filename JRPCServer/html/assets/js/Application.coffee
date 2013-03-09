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
    
    constructor: () ->
        
    start: () ->
        client = new RmitJourneyPlanner::Client()
        datasource = new RmitJourneyPlanner::Data::DataSources::StopNameDataSource()
        $("input.locationInput").each ->
            autocomplete = new RmitJourneyPlanner::UI::Controls::AutocompleteControl(client,                                                                   $(this),datasource);

        map = new RmitJourneyPlanner::UI::Controls::MapControl(client, $("#map_canvas"))


$(window).ready ->
    application = new Application()
    application.start()