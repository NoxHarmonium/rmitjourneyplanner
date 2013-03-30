###
****************************************************************
*
*   Class:       RmitJourneyPlanner.UI.PropertyEditor
*   Description: Generates a property edit form based on data
*                sent from the server.
*   
****************************************************************
###
"use strict"
class RmitJourneyPlanner::UI::Controls::PropertyEditor extends RmitJourneyPlanner::UI::Controls::Control                #Aliases
            Exceptions = RmitJourneyPlanner::Client::Exceptions;

            #Constants
            validation_success = "Success";

            #Fields
            data: null

            constructor: (@client, @element, @dataSource, @numColumns) ->
                super(@client,@element,["*"])
                if !(@dataSource instanceof RmitJourneyPlanner::Data::DataSources::JourneyPropertyDataSource)
                    throw new Exceptions::InvalidElementException(this, "JourneyPropertyDataSource")
                do @BuildForm


            DisplayValidationError: (value) ->
                if (value != null) 
                    input = @element.find('input[propName="' + value.target + '"]')
                    if (input.length == 0) 
                        input = @element.find('select[propName="' + value.target + '"]')
                    
                    controls = input.parent()
                    helpSpan = controls.find('.help-inline')
                    controlGroup = input.parent().parent()
                    if (helpSpan.length == 0) 
                        helpSpan = controls.parent().find('.help-inline')
                        controlGroup = input.parent().parent().parent()
                    
                    controlGroup.addClass('error')
                    
                    if (value.message == validation_success) 
                        helpSpan.text('')
                        controlGroup.removeClass('error')
                        return false
                    else
                        helpSpan.text(value.message)
                        controlGroup.addClass('error')
                        return true

            ValidateData: (data) ->        
                if ($.isArray(data.result)) 
                    for r in data.result
                        @DisplayValidationError(r)              
                else 
                    @DisplayValidationError(data.result)


           

            SaveForm: (resultCallback = ->) ->    
                propVals = new Array()
                _this = @;
                @element.find('.propertyField').each (index) ->

                    value = null
                    if !$(this).is("select")
                        value = null
                        if $(this).hasClass('locationInput')
                            value = $(this).attr('nodeid')
                        else 
                            value = $(this).val()
                    else
                        selected = $(this).find('option').filter(":selected")
                        if selected.length > 1
                            value = ""
                            for select in selected
                                value += $(select).val() + ",";
                            
                            value = value.substring(0, value.length - 1)

                        else
                            value = selected.val()

                    propVal = {
                        name: $(this).attr('propName'),
                        value: String(value)
                    }
                    propVals.push(propVal)

                results = null               

                _validateFunction = (data) -> 
                    results = $.proxy(_this.ValidateData, _this)(data)
                    if $.isArray(results)
                        for result in results
                            return resultCallback(true) if result is true
                        return resultCallback(false)
                    return resultCallback(results)

                _this.dataSource.Set(_validateFunction, propVals)

              

            BuildForm: (data = null) ->
                if !data?
                    @dataSource.Get (data) => 
                        @data = data
                        @BuildForm(data)
                    return
                
                columns = []
                for i in [0..@numColumns]
                    columns.push(jQuery(document.createElement('div')))
                i = 0
                for propertyInfo in data.result
                    newdiv = jQuery(document.createElement('div'));
                    newdiv.addClass('control-group');
                    label = jQuery(document.createElement('label'));
                    label.addClass('control-label');
                    label.attr('for', 'txt' + String(i));
                    label.html(propertyInfo.name);

                    controls = jQuery(document.createElement('div'));
                    controls.addClass('controls');        

                    input = null;

                    if (propertyInfo.editable || propertyInfo.type == "INetworkNode")         
                        input = jQuery(document.createElement('input'));
                        input.addClass('input');
                        input.attr('value', propertyInfo.value);
                        if (propertyInfo.type == "INetworkNode") 
                            split = propertyInfo.value.split(",");
                            if (split.length > 1) 
                                input.attr('value', split[0]);
                                input.attr('nodeid', split[1]);
                            else
                                input.attr('placeholder', 'Start typing to autocomplete...');
                    else
                        input = jQuery(document.createElement('select'));
                        multiLine = false;
                        split = propertyInfo.value.split("@");
                        if (split.length == 1) 
                            split = propertyInfo.value.split("|");
                            multiLine = true;

                        subsplit = split[0].split(",");
                        selSplit = split[1].split(",");
                        for o in subsplit
                            opt = jQuery(document.createElement('option'));
                            opt.text(o);
                            if (multiLine) 
                                input.attr('multiple', 'multiple');

                            for p in selSplit
                                if ($.trim(o) == $.trim(p))
                                    opt.attr('selected', 'selected');
                            
                            input.append(opt);


                    if (propertyInfo.type == 'Boolean') 
                        input.attr('type', 'checkbox');
                        b = String(propertyInfo.value).bool;
                        if (b == true) 
                            input.attr('checked', '');
                    else 
                        input.attr('type', 'text');
                        
                    input.addClass('propertyField');
                    input.attr('propName', propertyInfo.name);
                    input.attr('id', 'txt' + String(i));

                    controls.append(input)
                    newdiv.append(label)
                    newdiv.append(controls)
                    
                    if (propertyInfo.type == "INetworkNode") 

                        inputWrapper = jQuery(document.createElement('div'));
                        inputWrapper.append(input);
                        input.addClass('locationInput');

                        inputWrapper.addClass('input-append');
                        input.addClass('span2');
                        inputWrapper.addClass('inline-div');
                        inputWrapper.append('<span class="add-on"><i class="icon-map-marker"></i></span>');
                    
                    controls.append('<span class="help-inline"></span>');
                    
                   
                    columns[i++ % @numColumns].append(newdiv);
                
                @element.empty()
                @element.append(columns)
                 
               

