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

$(window).ready ->
    application = new Application()
    application.start()