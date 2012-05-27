/* Author:
	Sean Dawson
*/

var currentTab = $('#firstTab');
$('#txtError').hide();
//$('#btnStartSearch').button();


$("LI").click(function(e)
{
	currentTab.removeClass('active');
	$(this).addClass('active');
	$(currentTab.attr('title')).hide();
	$($(this).attr('title')).show();
	currentTab = $(this);

});

$("#btnStartSearch").click(function (e) {

    $('#btnStartSearch').button('loading');

    $('#txtError').load('aspx/InitSearch.aspx?', { txtOrigin: $('#txtOrigin').val(), txtDestination: $('#txtDestination').val(), txtDate: $('#txtDate').val(), txtTime: $('#txtTime').val(), chkBiDir: $('#chkBiDir').val() }, function () {
        var text = $.trim($('#txtError').text());

        if (text == '') {
            $('#txtError').hide();

            window.location.href = "results.html";

        } else {
            $('#txtError').show();
            $('#btnStartSearch').button('reset');
            //$('#btnStartSearch').val() = "Search";

        }
    });


});



