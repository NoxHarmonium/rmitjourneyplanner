///
/// Fields
///

///
/// Events
///
$('#btnJORefresh').click(function () {
    OptimiserRefresh();
});

$('#btnDelJourney').click(function ()
{
    RPCCall('DequeueJourney', { uuid: selectedJourneyUuid }, function (data) {
        if (CheckForError(data)) {
            return;
        }
    });
});

$('#btnJOEnqueue').click(function () {
    RPCCall('EnqueueJourney', { uuid: selectedJourneyUuid, runs: 1 }, function (data) {
        if (CheckForError(data)) {
            return;
        }
    });
});


$('#btnJOPause').click(function () {
    RPCCall('PauseJourneyOptimiser', {}, function (data) {
        if (CheckForError(data)) {
            return;
        }
    });
});


$('#btnJOResume').click(function () {
    RPCCall('ResumeJourneyOptimiser', {}, function (data) {
        if (CheckForError(data)) {
            return;
        }
    });
});






///
/// Functions
///
/*
    public enum OptimiserState
    {
        Waiting,
        Optimising,
        Saving,
        Cancelling,
        Idle

    }

*/


function OptimiserRefresh() {
    RPCCall('GetOptimisationState', {}, function (data) {
        if (CheckForError(data)) {
            return;
        }

        //Show form / hide loading message.
        $('#frmJourneyOptimiser').show();

        var state = data.result.state;
        $('#txtOptStatus').val(data.result.state);
        $('#btnJOPause').attr('disabled', 'disabled');
        $('#btnJOResume').attr('disabled', 'disabled');

      

        switch (state) {
            case "Waiting":
                //Nothing
                break;
            case "Optimising":
                $('#btnJOPause').removeAttr('disabled');
                break;
            case "Saving":
                //Nothing
                break;
            case "Cancelling":
                //Nothing
                break;
            case "Idle":
                //Nothing
                break;
        
        default:
        }
       

        $('#selOptimiserQueue').empty();
        for (var i in data.result.queue) {
            var queuedJourney = data.result.queue[i];
            var opt = jQuery(document.createElement('option'));
            opt.val(queuedJourney);
            $('#selOptimiserQueue').append(opt);

        }
        $('#txtJPCurrentIter').val(data.result.currentIteration);
        $('#txtJOTotalIters').val(data.result.totalIterations);
        $('#prgJO').css('width', String(Math.round((data.result.currentIteration / data.result.totalIterations) * 100.0)) + "%");




    });




}