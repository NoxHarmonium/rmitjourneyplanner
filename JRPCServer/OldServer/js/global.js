

///
/// Constants
///
var url_service = '/extendedjsonws.ashx';
var error_scroll_offset = -50;
var error_scroll_time = 500;

///
/// Fields
///
var googleEnabled = true;
var serverReady = false;

///
/// Global functions
///

// Converts a string representation of a boolean to a boolean.
String.prototype.bool = function() {
    return ( /^True$/i ).test(this);
};

jQuery.fn.exists = function() { return this.length > 0; };

jQuery.fn.disable = function() { return $(this).attr('disabled', 'disabled'); };

jQuery.fn.enable = function() { return $(this).removeAttr('disabled'); };


function showWarning(message) {
    if (message == "") {
        $('#divWarning').hide();
        return;
    }

	$('#divWarning').show();
    $('#divWarning').html(nl2br(message));

}

//Hide page elements and show error

function showError(message) {
    $('#divError').show();
    //$('#frmParameters').hide();
    //$('#divInfo').hide();
    $('#divError').html(nl2br(message));

	//Scroll to error box to highlight error.
    $('html, body').animate({
            scrollTop: $("#divError").offset().top + error_scroll_offset
        }, error_scroll_time);


}


//Callback for ajax errors.

function ajaxError(event, request, settings) {
    showError("<strong>Error accessing server:</strong>\nError " + event.status + ": " + event.statusText);

}

// Convert line breaks into <br /> tags

function nl2br(str, is_xhtml) {
	var breakTag = (is_xhtml || typeof is_xhtml === 'undefined') ? '<br />' : '<br>';
	return (str + '').replace( /([^>\r\n]?)(\r\n|\n\r|\r|\n)/g , '$1' + breakTag + '$2');
}

//Send JSON post request.

function JSONPost(url, data, callback) {
    return $.ajax({
            url: url,
            type: "POST",
            data: data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: callback,
            error: ajaxError
        });
}

// Call jayrock rpc

function RPCCall(method, params, callback) {
    last_request = method;

    var request = { };
    request.method = method;
    request.params = params;
    //request.params.CID = "45d0677d-a336-463b-ad99-c82137d03a00";
    //request.params.baseDN = "ou=people,dc=example,dc=com";
    //request.params.scope = "ONE";
    //request.params.filter = "(givenName=John)";
    request.id = 1;
    request.jsonrpc = "2.0";

    //
    //$.post(url_service , $.toJSON(request), callback, "json");
    return JSONPost(url_service, $.toJSON(request), callback);
}

// Returns true if there is a JSON error object in the data object. Also displays the error
// in an alert. Should be run on all data returned from JSON-RPC.

function CheckForError(data) {
    var error = data.error;

	if (error != undefined) {

		showError("<strong>Error requesting \'" + last_request + "\'... \nException: " + error.name +
		"</strong> \n" + error.message + "\n" + error.stackTrace);		
		
		
		return true;
                           

	}
	$('#divError').hide();
    return false;
}

///
/// Functionality checks
///
if (!window.google) {
	showWarning("<strong>Warning:</strong> Unable to load Google Maps scripts. Google Maps functionality will be disabled.");
    googleEnabled = false;
}


///
/// Initalisation Code
///
$(document).ready(function() {
    //

    RPCCall('LoadProviders', {
        
        /* void */
    }, function(data) {
        if (CheckForError(data)) {
            return;
        }
        google.load("visualization", "1", { packages: ["corechart"] });
        serverReady = true;
        window.refresh();
        window.OptimiserRefresh();
        window.refreshMap();
        window.loadJourneyRuns();
        $('#divInfo').hide();
    });

});