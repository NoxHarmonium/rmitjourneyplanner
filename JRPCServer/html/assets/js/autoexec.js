///
/// Initalisation Code
///
$(document).ready(function() {
	

    attachGeoAutoComplete();
	var d = new Date();
	$('#txtWhen').val(d.getHours()+ ":" + d.getMinutes());

	$('#btnMore').click(toggleDateTimeDiv);
	$('#btnCloseDivHow').click(hideDateTimeDiv);
	$('#btnSearch').click(search);
	
	$('.close').click(closeParent);
	
	//$('#example').tooltip();
	//$('#example').tooltip('show');
	
	
});

