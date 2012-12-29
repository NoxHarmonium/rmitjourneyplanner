/* Author:
	Sean Dawson
*/

///
/// Constants
///
var validation_success = "Success";

///
/// Fields
///
var last_request = '';
var cache = { }, lastXhr;


//Autocomplete settings.
/*
var acDefaultBounds = new google.maps.LatLngBounds(
			  new google.maps.LatLng(-36.536123, 143.074951),
			  new google.maps.LatLng(-39.019184, 147.304688));

var acOptions = {
    bounds: acDefaultBounds,
    types: []
};
*/


///
///Functions
///



//Generates a form based on the properties in the data.

function loadProperties(data) {
    for (var i = 0; i < data.result.length; i++) {
        var propertyInfo = data.result[i];
        var newdiv = jQuery(document.createElement('div'));
        newdiv.addClass('control-group');
        var label = jQuery(document.createElement('label'));
        label.addClass('control-label');
        label.attr('for', 'txt' + String(i));
        label.html(propertyInfo.name);

        var controls = jQuery(document.createElement('div'));
        controls.addClass('controls');

			/*
            if (propertyInfo.type == 'DateTime') {
                controls.addClass('input-append');
                controls.addClass('date');

                controls.attr('id', 'date' + String(i));
                controls.attr('data-date', '20-03-2012');
                controls.attr('data-date-format', 'dd-mm-yyyy');

            }
            */

        var input = null;

           	if (propertyInfo.editable || propertyInfo.type == "INetworkNode") {
                         

           	    input = jQuery(document.createElement('input'));
                         

           	    input.addClass('input');
                         

           	    input.attr('value', propertyInfo.value);
                         

           	    if (propertyInfo.type == "INetworkNode") {
                         

           	        var split = propertyInfo.value.split(",");	           		
	           		
	           		
	           		if (split.length > 1) {
                         

           	                                                  	           		
	           		
	           		
	           		    input.attr('value', split[0]);
                         

           	                                                  	           		
	           		
	           		
	           		    input.attr('nodeid', split[1]);
                         

           	                                                  	           		
	           		
	           		
	           		} else {
                         

           	                                                  	           		
	           		
	           		
	           		    input.attr('placeholder', 'Start typing to autocomplete...');
                         

           	                                                  	           		
	           		
	           		
	           		}
                         

           	    }           		
           		
           		
           	} else {


                         

           	    input = jQuery(document.createElement('select'));
                         

           	    var multiLine = false;
                         

           	    var split = propertyInfo.value.split("@");
                         

           	    if (split.length == 1) {
                         

           	        split = propertyInfo.value.split("|");
                         

           	        multiLine = true;
                         

           	    }
                         

           	    var subsplit = split[0].split(",");
                         

           	    var selSplit = split[1].split(",");
                         

           	    for (var o in subsplit) {
                         

           	        var opt = jQuery(document.createElement('option'));
                         

           	        opt.text(subsplit[o]);
                         

           	        if (multiLine) {
                         

           	            input.attr('multiple', 'multiple');

                         

           	        }

                         

           	        for (var p in selSplit) {

                         

           	            //input.append("<option>" + subsplit[o] + "</option>");
                         

           	            if ($.trim(subsplit[o]) == $.trim(selSplit[p])) {
                         

           	                opt.attr('selected', 'selected');
                         

           	            }

                         

           	        }
                         

           	        input.append(opt);

                         

           	    }
                         

           	    //input.find("option[text=\"" + split[1] + "\"]").attr("selected", "selected");//.siblings("option").attr("selected","");
                         

           	    // $(input). option:contains(" + inputText + ")"
                         

           	    //input.append("<option>" + split[1] + "</option>");
                         

           	}


            if (propertyInfo.type == 'Boolean') {
                         

           	 


                input.attr('type', 'checkbox');
                         

           	 


                var b = String(propertyInfo.value).bool;
                         

           	 


                if (b == true) {
                         

           	 


                    input.attr('checked', '');
                         

           	 


                }
                         

           	 


            } 
                         

           	    //else if (propertyInfo.type == "INetworkNode") 
                         

           	    //{
           		           
             
                         

           	    //} 
                         

           	else {
                         

           	 


                input.attr('type', 'text');
                         

           	 


                //input.attr('size', '16');
                         

           	 


            }

            
            input.addClass('propertyField');

            input.attr('propName', propertyInfo.name);

            input.attr('id', 'txt' + String(i));
        /*
            if (propertyInfo.type == 'DateTime') {
                input.attr('style', 'width: 90px;');
                input.attr('readonly', '');
                input.addClass('datePickerInput');

                input.val($.format.date(new Date(), "hh:mm a"));

            }
            */            
           

            //propertyInfo.type

        /*
            <div class="controls input-append date" id="dp3" data-date="20-03-2012" data-date-format="dd-mm-yyyy">
            <input id="txtDate" class="span2 input-xlarge" type="text" value="12-02-2012" readonly >
            <span class="add-on"><i class="icon-th"></i></span>
            </div>						
				
            </div>

            */

        newdiv.append(label);
        newdiv.append(controls);

            //inputWrapper.addClass('span2');
        if (propertyInfo.type == "INetworkNode") {


	           	var inputWrapper = jQuery(document.createElement('div'));
            inputWrapper.append(input);
            input.addClass('locationInput');

				inputWrapper.addClass('input-append');
            input.addClass('span2');
            inputWrapper.addClass('inline-div');
            inputWrapper.append('<span class="add-on"><i class="icon-map-marker"></i></span>');

				controls.append(inputWrapper);
        } else {
            controls.append(input);
        }
        /*
            if (propertyInfo.type == 'DateTime') {
                controls.append('<span class="add-on"><i class="icon-th"></i></span>');
                controls.append('<input type=\'text\' value=\'Text\' class=\'input-append\' data-provide="timepicker" style=\'width: 90px;\'  />');
                controls.append('<span class="add-on"><i class="icon-time"></i></span>');
                controls.datepicker();
            }
            */
        controls.append('<span class="help-inline"></span>');
        if (i % 2 != 0) {
            $("#cg-left").append(newdiv);

            } else {
            $("#cg-right").append(newdiv);
        }
        //$("#divAutoForm").prepend(newdiv);
    }

}

function displayValidationError(value) {
    if (value != null) {
        var input = $('input[propName="' + value.target + '"]');
        if (input.length == 0) {
            var input = $('select[propName="' + value.target + '"]');
        }
        var controls = input.parent();
        var helpSpan = controls.find('.help-inline');
        var controlGroup = input.parent().parent();
        if (helpSpan.length == 0) {
            helpSpan = controls.parent().find('.help-inline');
            controlGroup = input.parent().parent().parent();
        }

		controlGroup.addClass('error');

		if (value.message == validation_success) {
         

		                               

		    helpSpan.text('');
         

		                               

		    controlGroup.removeClass('error');
         

		                               

		} else {
         

		                               

		    helpSpan.text(value.message);
         

		                               

		    controlGroup.addClass('error');
         

		                               

		}

	}

}

function checkValidation(data) {


		//Show errors
    if ($.isArray(data.result)) {
        for (var r in data.result) {
            displayValidationError(data.result[r]);
        }

			//$.each(data.result, displayValidationError); 
    } else {
        displayValidationError(data.result);

		}		


}

function attachGeoAutoComplete() {
    $('input.locationInput').each(function(index) {

		/*
		$(this).geo_autocomplete(new google.maps.Geocoder, {
					mapkey: 'ABQIAAAAbnvDoAoYOSW2iqoXiGTpYBTIx7cuHpcaq3fYV4NM0BaZl8OxDxS9pQpgJkMv0RxjVl6cDGhDNERjaQ', 
					selectFirst: false,
					minChars: 3,
					cacheLength: 50,
					//width: 300,
					mapwidth:25,
					mapheight:25,
					maptype: 'roadmap',
					scroll: true,
					scrollHeight: 330,	
					geocoder_region: "Australia"		
				});
		*/

	//.ui-autocomplete-loading { background: white url('images/ui-anim_basic_16x16.gif') right center no-repeat; }
        //$(this).attr('value','');
        $(this).autocomplete({
                minLength: 3,
                source: function(request, response) {
                    var term = request.term;
                    if (term in cache) {
                        response(cache[term].result);
                        return;
                    }

                    lastXhr = RPCCall("QueryStops", { "query": String(request.term) }, function(data, status, xhr) {
                        if (CheckForError(data)) {
                            return;
                        }

					cache[term] = data;
                        //if ( xhr === lastXhr ) {
                        response(data.result);
                        //}
                    });
                },
                focus: function(event, ui) {
                    $(this).val(ui.item.label);
                    return false;
                },
                select: function(event, ui) {
                    $(this).attr('nodeid', ui.item.value);
                    $(this).val(ui.item.label);
                    $(this).attr('validid', 'true');
                    //$( "#project-id" ).val( ui.item.value );
                    //$( "#project-description" ).html( ui.item.desc );
                    //$( "#project-icon" ).attr( "src", "images/" + ui.item.icon );

                    return false;
                }
            }).data("autocomplete")._renderItem = function(ul, item) {
                return $("<li></li>")
                    .data("item.autocomplete", item)
                    .append("<a><address><img class='imgTransportIcon' src='img/transportIcons/" + item.stopMode + ".png' /><strong>" + item.label + "</strong><br>" + item.stopSpecName + "</address></a>")
                    .appendTo(ul);
            };
        ;
    });	


}

function attachInputValidation() {

	//Validate input change
    $('input.propertyField').change(function() {
        var value;
        if ($(this).hasClass('locationInput') && $(this).attr('validid') == 'true') {
            value = $(this).attr('nodeid');
            $(this).attr('validid', 'false');
        } else {
            value = $(this).val();
        }

			var propVal = {
            name: $(this).attr('propName'),
            value: value
        };

			var propVals = { "propVal": propVal };

			RPCCall('ValidateField', propVals, function(data) {
            if (CheckForError(data)) {
                return;
            }
            checkValidation(data);

		});


});

}

function saveProperties() {
    var propVals = new Array();
    $('.propertyField').each(function(index) {
        //alert(index + ': ' + $(this).val());	

        var value;
        //if ($(this).prop('disabled') == false)
        if (!$(this).is("select")) {
            var value;
            if ($(this).hasClass('locationInput')) {
                value = $(this).attr('nodeid');
            } else {
                value = $(this).val();
            }
            /*
	        FORMAT:
	        { "propVals" : [{ "propVal" : {"name":"CrossoverRate","value":"0.7"} }] }
	        */
        } else {
            var selected = $(this).find('option').filter(":selected");
            if (selected.length > 1) {
                value = "";
                for (var i = 0; i < selected.length; i++) {
                    value += $(selected[i]).val() + ",";

                }
                value = value.substring(0, value.length - 1);

            } else {
                value = selected.val();
            }
        }


        var propVal = {
            name: $(this).attr('propName'),
            value: string(value)
        };
        propVals.push(propVal);


    });


    RPCCall('SetProperties', { "journeyUuid": selectedJourneyUuid, "propVals": propVals }, function(data) {
        if (CheckForError(data)) {
            return;
        }

		checkValidation(data);

	});
}

function GetProperties(uuid) {
    RPCCall('GetProperties', { "journeyUuid": uuid }, function(data) {
        if (CheckForError(data)) {
            return;
        }

        //Show form / hide loading message.
        $('#frmParameters').show();


        $("#cg-left").empty();
        $("#cg-right").empty();

        loadProperties(data);
        attachInputValidation();
        attachGeoAutoComplete();
        updateNavBar();
    });
}

///
/// Events
///

//Submit button code
$('#btnSaveProperties').click(function() {

	saveProperties();

});


//Data load events
$('#divInfo').show();



   




///
/// Code
///