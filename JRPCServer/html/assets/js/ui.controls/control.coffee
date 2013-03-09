###
****************************************************************
*
*   Class:       RmitJourneyPlanner.UI.Control
*   Description: The base class for all UI controls.
*   
****************************************************************
###
"use strict"
class RmitJourneyPlanner::UI
    class @::Controls
        #Aliases
        Exceptions = RmitJourneyPlanner::Client::Exceptions;
    
        class @::Control      
            constructor: (
                @client, 
                @element,
                @supportedElements = [], 
                @supportsMultipleElements = false
            ) ->
                if (!@element?)
                    throw new Exceptions::NullElementException(this)
            
                # Make sure the object is wrapped in jQuery.
                @element = $(@element);

                if (@element.length == 0)
                    throw new Exceptions::NullElementException(this)

                if (@element.length > 1 && !@supportsMultipleElements)
                    throw new Exceptions::MultipleElementException(this)

                for supportedElement in @supportedElements
                    if (!@element.is(supportedElement))
                        throw new Exceptions::InvalidElementException(this,supportedElement);

