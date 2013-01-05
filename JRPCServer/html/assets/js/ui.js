//
// Fields
//

var dateTimeDivShown = false;
var loadingDivShown = false;
var helpDivShown = true;

//
// Contants
//

var strQueuedMessage = 'Your journey planning request is currently queued. Please wait....';
var strQueuedTitle = 'Queued...';
var strOptimisingMessage = 'The journey planning engine is currently calculating possible journeys. Please wait....';
var strOptimisingTitle = 'Optimising...';
var strFinishedMessage = 'The optimisation process is finished. The results should display now....';
var strFinishedTitle = 'Finished...';
var strUnknownTitle = 'Unknown Status...';


//
// Functions
//

function closeParent()
{
	$(this).parent().hide();
}

function displayValidationError(value) {
    if (value != null) {
        var input = $('input[propName="' + value.target + '"]');
        if (input.length == 0) {
            var input = $('select[propName="' + value.target + '"]');
        }
		
      
		if (value.message == validation_success) {
		    
		    input.removeClass('error');
			$(input).popover('destroy');

		} else {		    
			input.addClass('error');   
			input.popover({
			"animation" : "True",
			"placement" : "bottom",
			"trigger"	: "hover",
			"title"		: "Validation Error",
			"content"	: value.message});

		}

	}

}

function enableSearch()
{
	$('.subnav-inner input').enable();
	$('.subnav-inner button').enable();
	$('.subnav-inner checkbox').enable();
}

function disableSearch()
{
	$('.subnav-inner input').disable();
	$('.subnav-inner button').disable();
	$('.subnav-inner checkbox').disable();
}

function setLoadingDivProgress(progress, iteration, totalIterations) {

    $('#divLoading .progressNumerator').text(iteration);
    $('#divLoading .progressDenominator').text(totalIterations);
    $('#mainProgressBar .bar').width(String(progress * 100.0) + "%");
    
}

function setLoadingDivMode(mode) {
    $('#divLoading .progressHelp').show();
    switch (mode) {
        case "queued":

            $('#divLoading H1').text(strQueuedTitle);
            $('#divLoading .progressHelp').text(strQueuedMessage);
            $('#divLoading .progressInfo').hide();
            break;

        case "optimising":
            $('#divLoading H1').text(strOptimisingTitle);
            $('#divLoading .progressHelp').text(strOptimisingMessage);
            $('#divLoading .progressInfo').show();
            
            break;

        case "finished":
            $('#divLoading H1').text(strFinishedTitle);
            $('#divLoading .progressHelp').text(strFinishedMessage);
            $('#divLoading .progressInfo').hide();
            break;

        default:
            $('#divLoading H1').text(strUnknownTitle);
            $('#divLoading .progressHelp').hide();
            $('#divLoading .progressInfo').hide();
    }
    
    
    
}


function showLoadingDiv() {
    if (!loadingDivShown) {

        $('#divLoading').show();
        dateTimeDivShown = true;
    }


}

function hideLoadingDiv() {
    if (loadingDivShown) {

        $('#divLoading').hide();
        dateTimeDivShown = false;
    }

}

function showHelpDiv() {
    if (!helpDivShown) {

        $('#divHelp').show();
        helpDivShown = true;
    }
}

function hideHelpDiv() {
    if (helpDivShown) {

        $('#divHelp').hide();
        helpDivShown = false;
    }

}


function toggleDateTimeDiv()
{
	if (dateTimeDivShown)
	{
		hideDateTimeDiv();
	}
	else
	{
		showDateTimeDiv();
	}
	dateTimeDivShown  = !dateTimeDivShown;
}

function showDateTimeDiv()
{
	$('#btnMore i').removeClass('icon-chevron-down');
	$('#btnMore i').addClass('icon-chevron-up');
	//$('#divWhen').removeClass('invisible');
	 $('#divWhen').slideDown('slow', function() {
		dateTimeDivShown = true;
  });

}

function hideDateTimeDiv()
{
    if (dateTimeDivShown) {
        $('#btnMore i').addClass('icon-chevron-down');
        $('#btnMore i').removeClass('icon-chevron-up');
        //('#divWhen').addClass('invisible');

        $('#divWhen').slideUp('slow', function() {
            dateTimeDivShown = false;
        });
    }

}