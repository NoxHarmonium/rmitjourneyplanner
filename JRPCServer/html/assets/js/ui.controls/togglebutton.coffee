###
****************************************************************
*
*   Class:       RmitJourneyPlanner.UI.Togglebutton
*   Description: Makes a control switch between 2 child elements
*                on click.
*   
****************************************************************
###
"use strict"
class RmitJourneyPlanner::UI::Controls::Togglebutton extends RmitJourneyPlanner::UI::Controls::Control       

            
            children: null

            constructor: (@client, @element, toggled = false) ->
                super(@client,@element,["*"])
                @visible = @element.is(":visible")
                
                @children = @element.children()            
                first = @children.eq(0)
                second = @children.eq(1)
                
                first.show()
                second.hide();
                
                @element.click => do @toggle

                do @toggle if toggled
                               
            toggle: () ->
                if (@children.length > 0)
                    @children.toggle()
           