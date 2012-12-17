//
// Fields
//

var dateTimeDivShown = false;



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

	$('#btnMore i').addClass('icon-chevron-down');
	$('#btnMore i').removeClass('icon-chevron-up');
	//('#divWhen').addClass('invisible');
	
	$('#divWhen').slideUp('slow', function() {
		dateTimeDivShown = false;
  });

}