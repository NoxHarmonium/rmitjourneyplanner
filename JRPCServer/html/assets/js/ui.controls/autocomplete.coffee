###
****************************************************************
*
*   Class:       RmitJourneyPlanner.UI.AutocompleteControl
*   Description: Adds autocomplete funtionality to an input
*                element with the provided datasource.
*   
****************************************************************
###
"use strict"
class RmitJourneyPlanner::UI::Controls::Autocomplete extends RmitJourneyPlanner::UI::Controls::Control
            #assets/img/transportIcons/" + item.stopMode + ".png             
            cache: []
            selectedItem:  null

            constructor: (@client, @element, @dataSource) ->
                super(@client,@element,["input"])            
                           
               

                if (!@element.autocomplete?)
                    throw new Exceptions::MissingDependency(this,"jQuery UI Autocomplete")

                @element.autocomplete( {
                        minLength: 3,
                        source: (request, response) =>
                            term = request.term
                            if (term in @cache) 
                                response(@cache[term].result)
                                return;                        
                        
                            dataSource.Query term, (data) =>                            
                                @cache[term] = data
                                response(data.result)
                        
                        focus: (event, ui) =>
                            @element.val(ui.item.title)
                            return false
                    
                        select: (event, ui) =>
                            @selectedItem = ui.item
                            return false
                        }
                ).data("autocomplete")._renderItem = (ul, item) ->
                    return $("<li></li>")
                        .data("item.autocomplete", item)
                        .append("<a><div class='acEntry'><img class='acThumbnail' src='"+ item.image + "' />&nbsp;&nbsp;<strong>" + item.title + "</strong><br>" + item.detail + "</div></a>")
                        .appendTo(ul);