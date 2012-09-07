///
/// Fields
///
var currentOption;
var pendingChanges = false;
var selectedJourneyUuid;

///
/// Functions
///
function refresh()
{
    RPCCall("GetJourneys", {}, function (data) {
        if (CheckForError(data)) {
            return;
        }

        var journeys = $(data.result);
        var sel = $('#selJourneyList');
        sel.html('');

        $.each(journeys, function (index) {
            var newOpt = jQuery(document.createElement('option'));
            newOpt.attr('value', this.uuid);
            if (this.description == null) {
                this.decription = "";
            }
            newOpt.attr('description', this.description);
            newOpt.text(this.shortName);
            sel.append(newOpt);

        });
    });
}

function disableControls()
{
	$('#txtUuid').attr('value','');
	$('#txtJourneyName').attr('value','');
	$('#txtDescription').attr('value','');
	
	$('#txtJourneyName').disable();
	$('#txtDescription').disable();

}


///
/// Event Handlers
///
function jDetailChange()
{
	pendingChange = true;
}
$('#txtJourneyName').change(jDetailChange);
$('#txtDescription').change(jDetailChange);



$('#btnNewJourney').click(function() {
	RPCCall("NewJourney",{ "shortName" : "NewJourney", "description" : null },function (data)
	{
		if (CheckForError(data))
		{
			return;		
		}
		//alert("New journey created with UUID: " + data.result);
		refresh();
	});

});

$('#btnCloneJourney').click(function () {
    var selected = $('#selJourneyList>option:selected');

    if (selected.exists()) {

        RPCCall("CloneJourney", { "journeyUuid": selected.attr('value') }, function (data) {
            if (CheckForError(data)) {
                return;
            }
            //alert("New journey created with UUID: " + data.result);
            refresh();
        });
    }

});

$('#btnDelJourney').click(function() {
	
	var selected = $('#selJourneyList>option:selected');
	
	if (selected.exists())
	{
	
		RPCCall("DeleteJourney",{ "uuid" : selected.attr('value')  },function (data)
		{
			if (CheckForError(data))
			{
				return;		
			}
			//alert("New journey created with UUID: " + data.result);
			refresh();
		});
	}


});

$('#selJourneyList').change(function () {
    var selected = $('#selJourneyList>option:selected');
    if (selected.exists()) {
        $('#txtJourneyName').enable();
        $('#txtDescription').enable();
        $('#txtUuid').attr('value', selected.attr('value'));
        $('#txtJourneyName').attr('value', selected.text());
        $('#txtDescription').attr('value', selected.attr('description'));
        currentOption = selected;
        selectedJourneyUuid = selected.attr('value');
        GetProperties(selected.attr('value'));

    }
    else {
        disableControls();
    }
    OptimiserRefresh();
    loadJourneyRuns();
    refreshViewerSlider();

});

$('#txtJourneyName').bind('keyup', function() { 
	//alert('ff');
	//currentOption.text($(this).attr('value'));
	
} );

$('#btnSaveJourneyName').click(function () {
	var uuid = $('#txtUuid').attr('value');
	var journeyName = $('#txtJourneyName').attr('value');
	var description = $('#txtDescription').attr('value');
	
	RPCCall("SetJourneyName",{ "uuid" : uuid, "shortName" : journeyName, "description" : description },function (data)
	{
		if (CheckForError(data))
		{
			return;
		}
		
		pendingChanges = false;
		refresh();
	    
	});
});

///
/// Main Code
///
//refresh();
disableControls();