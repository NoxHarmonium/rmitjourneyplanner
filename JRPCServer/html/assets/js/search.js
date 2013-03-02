//
// Search functionality
//


//
// Fields
//
var userKey = "abcdef";
var validation_success = "Success";
var progress_callback_id = null;

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
            if (progress_callback_id != null) {
                clearInterval(progress_callback_id);
            }
            progress_callback_id = setInterval(progressCallback, 500);
            progressCallback();
        }




    });
}

function getResults() {

    RPCCall('GetResult', { "userKey": userKey }, function (data) {
        if (CheckForError(data)) {
            return;
        }       
        
        showResults(data);

    });
}

function progressCallback() {

    RPCCall('GetStatus', { "userKey": userKey }, function (data) {
        if (CheckForError(data)) {
            return;
        }

        //data.result
        var d = data.result;

        if (d.status.toLowerCase() == "unknown") {
            return;
        }

        //If validation succeded, open progress bar.
        showLoadingDiv();
        hideHelpDiv();

        setLoadingDivProgress(d.progress, d.iteration, d.totalIterations);

        setLoadingDivMode(d.status);

        if (d.status.toLowerCase() == "finished") {
            clearInterval(progress_callback_id);
            progress_callback_id = null;
            getResults();
        }


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