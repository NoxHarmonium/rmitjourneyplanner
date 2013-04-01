###
****************************************************************
*
*   Class:       RmitJourneyPlanner.UI.Datetimepicker
*   Description: Adds date and time picker to an input element.
*   See:         http://trentrichardson.com/examples/timepicker/
*   
****************************************************************
###
"use strict"
class RmitJourneyPlanner::UI::Controls::Datetimepicker extends RmitJourneyPlanner::UI::Controls::Control   


            constructor: (@client, @element, @defaultValue = new Date()) ->
                super(@client,@element,["input"])          
                           
                if (!@element.datetimepicker?)
                    throw new Exceptions::MissingDependency(this,"jQuery UI Timepicker")

                @element.val("Loading...")
                @element.disable()

                # Load the datetime picker only if the datetime format is loaded from the server.
                @client.datetimeFormatLoad.done (dateString, timeString)  =>
                    @element.enable()
                    @element.val(''); 
                    
                    @element.datetimepicker({
                        alwaysSetTime: true, 
                        timeFormat: timeString,
                        dateFormat: dateString.replace("yy","y")
                    });
                    @element.datetimepicker('setDate', @defaultValue)

               
