//
// Search functionality
//


//
// Fields
//
var userKey = "abcdef";
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
                endSearch();
                return false;
            }
        }

    } else {

        displayValidationError(data.result);
        
        if (data.result.message != validation_success) {
            showDateTimeDiv();
            endSearch();
            return false;
        }
		
	}
    return true;

}

function submitSearch() {
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



    RPCCall('Search', { "userKey": userKey, "propVals": propVals }, function (data) {
        if (CheckForError(data)) {
            return;
        }

        if (checkValidation(data)) {
            //If validation succeded, open progress bar.
            showLoadingDiv();
            hideHelpDiv();
            setInterval(progressCallback, 500);
            progressCallback();
        }




    });
}



function progressCallback() {

    RPCCall('GetStatus', { "userKey": userKey }, function (data) {
        if (CheckForError(data)) {
            return;
        }

        //data.result

        $('#mainProgressBar .bar').width(String(data.result.progress * 100.0) + "%");


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
    showHelpDiv();
}



// The main function
function search()
{
	startSearch();
	submitSearch();
	
}