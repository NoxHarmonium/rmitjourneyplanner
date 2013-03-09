###
****************************************************************
*
*   Class:       RmitJourneyPlanner.UI.Alertbox
*   Description: Makes a control able to show and hide with 
*                different effect.
*   See:         http://twitter.github.com/bootstrap/components.html#alerts
*   
****************************************************************
###
"use strict"
class RmitJourneyPlanner::UI::Controls::Hideable extends RmitJourneyPlanner::UI::Controls::Control       

            effectTypes: [ "", "none", "fade", "slide" ]
            defaultEffectType: ""

            constructor: (@client, @element, supportedElements = ["*"]) ->
                super(@client,@element,supportedElements)
                
            
            setEffectType: (effectType) ->
                @defaultEffectType = effectType
                       
            show: (effectType = @defaultEffectType) ->
                if (effectType in @effectTypes)
                    @setEffectType(effectType)
                    switch effectType
                        when "fade"
                            @element.fadeIn()
                        when "slide"
                            @element.slideDown()
                        else
                             @element.show()
                    
               

            hide: (effectType = @defaultEffectType) ->
                  if (effectType in @effectTypes)
                    @setEffectType(effectType)
                    switch effectType
                        when "fade"
                            @element.fadeOut()
                        when "slide"
                            @element.slideUp()
                        else
                             @element.hide()