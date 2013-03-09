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
            autocomplete = new RmitJourneyPlanner::UI::Controls::Autocomplete(client,                                                                   $(this),datasource);

        map = new RmitJourneyPlanner::UI::Controls::MapControl(client, $("#map_canvas"))

        errorBox = new RmitJourneyPlanner::UI::Controls::AlertBox(client, $("#divError")) 
        errorBox.setSuccess()
        errorBox.setTitle("Success!")
        errorBox.setMessage("The error box component works!")
        errorBox.show("slide")


$(window).ready ->
    application = new Application()
    application.start()