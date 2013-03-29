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
            visible: false
            toggleElement: null

            constructor: (@client, @element, supportedElements = ["*"]) ->
                super(@client,@element,supportedElements)
                @visible = @element.is(":visible")
                
            
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


            toggle: ->
                if @visible
                    @hide()
                else
                    @show() 
                @visible = !@visible
                
            setToggleElement: (element) ->
                if (@toggleElement?)
                    @toggleElement.off("click.hideable")

                @toggleElement = element
                @toggleElement.on("click.hideable", => do @toggle)