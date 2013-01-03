//
// Search functionality
//


//
// Fields
//
var selectedJourneyUuid = "default";
var validation_success = "Success";

//
// Functions
//


function checkValidation(data) {


		//Show errors
    if ($.isArray(data.result)) {
        for (var r in data.result) {
            displayValidationError(data.result[r]);
            if (data.result[r].message != validation_success)
            {
                showDateTimeDiv();
            }
        }

    } else {

        displayValidationError(data.result);
        
        if (data.result.message != validation_success) {
            showDateTimeDiv();
        }
		
	}		


}

function saveProperties() {
    var propVals = new Array();
    $('.propertyField').each(function(index) {
        //alert(index + ': ' + $(this).val());	

        var value;
        //if ($(this).prop('disabled') == false)
        if (!$(this).is("select")) {
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
            value: value
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



function startSearch()
{
    hideDateTimeDiv();
	disableSearch();
}

function endSearch()
{
	//showDateTimeDiv()	
	enableSearch();
}



// The main function
function search()
{
	startSearch();
	saveProperties();
	endSearch();
}