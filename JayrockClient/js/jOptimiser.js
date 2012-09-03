///
/// Fields
///

///
/// Functions
///
function OptimiserRefresh() {
    RPCCall('GetOptimisationState', {}, function (data) {
        if (CheckForError(data)) {
            return;
        }

        //Show form / hide loading message.
        $('#frmJourneyOptimiser').show();
        $('#txtOptStatus').val(data.result.state);
        $('#selOptimiserQueue').empty();
        for (var i in data.result.queue) {
            var queuedJourney = data.result.queue[i];
            var opt = jQuery(document.createElement('option'));
            opt.val(queuedJourney);
            $('#selOptimiserQueue').append(opt);

        }



    });




}