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
var strFinishedMessage = 'The results of the optimisation are shown below....';
var strFinishedTitle = 'Results...';
var strUnknownTitle = 'Unknown Status...';

var strTransportImagePath = "assets/img/transportIcons/";
var strTransportImageExt = ".png";


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
    switch (mode.toLowerCase()) {
        case "queued":

            $('#divLoading .title').text(strQueuedTitle);
            $('#divLoading .progressHelp').text(strQueuedMessage);
            $('#divLoading .progressInfo').hide();
            break;

        case "optimising":
            $('#divLoading .title').text(strOptimisingTitle);
            $('#divLoading .progressHelp').text(strOptimisingMessage);
            $('#divLoading .progressInfo').show();
            
            break;

        case "finished":
            $('#divLoading .title').text(strFinishedTitle);
            $('#divLoading .progressHelp').text(strFinishedMessage);
            $('#divLoading .progressInfo').hide();
            $('#divLoading div.progress').hide();
            break;

        default:
            $('#divLoading .title').text(strUnknownTitle);
            $('#divLoading .progressHelp').hide();
            $('#divLoading .progressInfo').hide();
    }
    
    
    
}


function showLoadingDiv() {
    if (!loadingDivShown) {

        $('#divLoading').show();
        loadingDivShown = true;
    }


}

function hideLoadingDiv() {
    if (loadingDivShown) {

        $('#divLoading').hide();
        loadingDivShown = false;
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

function hideResultsDiv() {
    $('#divJourneyList').hide();
}

function showResultsDiv() {
    $('#divJourneyList').show();
}

function selectJourney(e) {  
    var sender = $(e.target).parents('tr');
    $('.selectedJourney').removeClass('selectedJourney');
    sender.addClass('selectedJourney');
    var legs = sender.prop('jLegs');
    mapManager.clearMarkers();
    mapManager.clearLines();
    for (i in legs)
    {

        
        var leg = legs[i];

        if (i == 0 || !(leg.Mode == "Walking" && legs[parseInt(i) - 1].Mode == "Walking")) {
            mapManager.addMarker(new google.maps.Marker({
                position: new google.maps.LatLng(leg.StartLocation.Lat, leg.StartLocation.Long),
                icon: strTransportImagePath + leg.Mode + strTransportImageExt
            }));
        }

        var lineColour;

        switch (leg.Mode) {
            case "Train":
                lineColour = "#0000FF";
                break;
            case "Bus":
                lineColour = "#B25800";
                break;
            case "Tram":
                lineColor = "#00FF00";
                break;
            default:
                lineColour = "#333333";
                break;      
        }

        mapManager.addLine(new google.maps.Polyline({
            path: [new google.maps.LatLng(leg.StartLocation.Lat, leg.StartLocation.Long),
                    new google.maps.LatLng(leg.EndLocation.Lat, leg.EndLocation.Long)],
            strokeColor: lineColour,
            strokeOpacity: 1.0,
            strokeWeight: 2
        }));
        
        if (parseInt(i) == legs.length - 1) {
            //Draw finish marker
            mapManager.addMarker(new google.maps.Marker({
                position: new google.maps.LatLng(leg.EndLocation.Lat, leg.EndLocation.Long),
                icon: strTransportImagePath + "Finish" + strTransportImageExt
            }));
        }

    }
}

function showResults(data) {

    var results = data.result;
    var tableSummary = $('#divJourneyList table');
    for (var i in results) {
        var journey = results[i].Critter;
        var trSummary = $('<tr></tr>');
        var tdTitle = $('<td></td>');
        tdTitle.text('Journey ' + i);
        var tdTime = $('<td></td>');
        var d = new Date(journey.Fitness.TotalJourneyMinutes * 60 * 1000);
        tdTime.text(d.getUTCHours() + 'h ' + d.getUTCMinutes() + 'm');
        var tdLegs = $('<td class="journeyModes"></td>');
        for (var j in journey.Legs) {            
            var leg = journey.Legs[j];
            if (j == 0 || !(leg.Mode == "Walking" && journey.Legs[parseInt(j) - 1].Mode == "Walking")) {
                //<img class="miniIcon" src="assets/img/transportIcons/Train.png" />
                var imgLeg = $('<img class="miniIcon"/>');
                imgLeg.attr('src', strTransportImagePath + leg.Mode + strTransportImageExt);
                tdLegs.append(imgLeg);
            }
        }
        trSummary.prop('jLegs', journey.Legs);
        trSummary.click(selectJourney);
        
        trSummary.append(tdTitle);
        trSummary.append(tdTime);
        trSummary.append(tdLegs);
        tableSummary.append(trSummary);
    }
    showResultsDiv();
 

}