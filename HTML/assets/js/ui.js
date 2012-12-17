//
// Fields
//

var dateTimeDivShown = false;



//
// Functions
//

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