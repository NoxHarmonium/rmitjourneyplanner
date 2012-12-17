//
// Fields
//

var dateTimeDivShown = false;



//
// Functions
//

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
    // Animation complete.
  });

}

function hideDateTimeDiv()
{

	$('#btnMore i').addClass('icon-chevron-down');
	$('#btnMore i').removeClass('icon-chevron-up');
	//('#divWhen').addClass('invisible');
	
	$('#divWhen').slideUp('slow', function() {
    // Animation complete.
  });

}