###
****************************************************************
*
*   Class:       RmitJourneyPlanner.UI.Alertbox
*   Description: Turns a div element into a bootstrap alert 
*                component.
*   See:         http://twitter.github.com/bootstrap/components.html#alerts
*   
****************************************************************
###
"use strict"
class RmitJourneyPlanner::UI::Controls::AlertBox extends RmitJourneyPlanner::UI::Controls::Hideable    

            types: [ "", "info", "success", "error" ]
            current_type: ""
            title: ""
            message: ""

            constructor: (@client, @element) ->
                super(@client,@element,["div"])

                @element.addClass("alert")
                @element.children().remove()
                @btnClose = $('<button type="button" class="close" data-dismiss="alert">&times;</button>');
                @btnClose.click (e) => @hide(); return false;
                @txtMessage = $('<span></span>')
                @element.append(@btnClose)
                @element.append(@txtMessage)
                @element.hide()
              
    
            setTitle: (title) ->
                @title = title       
                do @render     
    
            setMessage: (message) ->
                @message = message                
                do @render
    
            render: ->
                html = ""
                if (@title != "")
                    html += "<strong>" + @txtMessage.text(@title).html() + "</strong>&nbsp";
                html += @txtMessage.text(@message).html() 
                @txtMessage.html(html)

            setHtml: (message) ->
                @txtMessage.html(message)

            setType: (type) ->
                if type in @types
                    @element.removeClass("alert-" + @current_type);
                    @element.addClass("alert-" + type);
                    @current_type = type

            setError: ->
                @setType("error");

            setWarning: ->
                @setType("")
            
            setSuccess: ->
                @setType("success")

            setError: ->
                @setType("error")
                           
