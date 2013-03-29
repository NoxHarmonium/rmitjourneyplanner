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
     
    client:  new RmitJourneyPlanner::Client()
        
    start: () -> 
        datasource = new RmitJourneyPlanner::Data::DataSources::StopNameDataSource()
        $("input.locationInput").each ->
            autocomplete = new RmitJourneyPlanner::UI::Controls::Autocomplete(@client,                                                                   $(this),datasource);

        btnMore = new RmitJourneyPlanner::UI::Controls::Togglebutton(@client,$("#btnMore"))

        map = new RmitJourneyPlanner::UI::Controls::MapControl(@client, $("#map_canvas"))
        divWhen = new RmitJourneyPlanner::UI::Controls::Hideable(@client, $("#divWhen"))
        divWhen.setToggleElement($("#btnMore"))
        divWhen.setEffectType("slide")
        

        errorBox = new RmitJourneyPlanner::UI::Controls::AlertBox(@client, $("#divError")) 
        errorBox.setSuccess()
        errorBox.setTitle("Success!")
        errorBox.setMessage("The error box component works!")
        errorBox.show("slide")

        btnSearch = $("#btnSearch");
        btnSearch.click(=> do @search)

    search: () ->
        alert("search")


$(window).ready ->
    application = new Application()
    application.start()