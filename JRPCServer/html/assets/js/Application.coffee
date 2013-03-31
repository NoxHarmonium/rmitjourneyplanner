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
    propertyEditor: null
    searchPanel: null
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
        #btnSaveProperties


        @propertyDataSource = new RmitJourneyPlanner::Data::DataSources::JourneyPropertyDataSource(GlobalProperties.userKey)
        @propertyEditor = new RmitJourneyPlanner::UI::Controls::PropertyEditor(@client, $("div#myModal > div.modal-body"), @propertyDataSource, 2)
        @propertyEditor.BuildForm();

        @searchPanel =  new RmitJourneyPlanner::UI::Controls::PropertyEditor(@client, $("div#searchPanel"), @propertyDataSource, null)

        btnSaveProperties = $("div#myModal button#btnSaveProperties");
        btnSaveProperties.click (event) =>
            @propertyEditor.SaveForm (result) ->
                if !result
                    $('div#myModal').modal('hide')


    search: () ->
        searchDatasource = new RmitJourneyPlanner::Data::DataSources::SearchDataSource(GlobalProperties.userKey)
        @searchPanel.SaveForm (result) ->
                if !result
                    searchDatasource.Query (validationErrors) ->
                        for validationError in validationErrors
                            console.log(validationError)


$(window).ready ->
    application = new Application()
    application.start()