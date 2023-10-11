$(".btn-edit").click(function () {
    var tr = $(this).closest('tr');
    tr.find('.editField').show();
    tr.find('.data').hide();
    tr.find('.btn-save, .btn-cancel').show();
    $(this).closest('tr').find('.editField').attr('required', true);
    tr.find('.btn-save').prop('disabled', true);
    $(this).hide();

});

$('tr').on('input', '.editField', function () {
    var tr = $(this).closest('tr');
    var allFilled = true;

    // Check if all required fields are filled
    tr.find('.editField[required]').each(function () {
        if ($(this).val() === '') {
            allFilled = false;
            return false;  // exit the .each() loop
        }
    });

    // Enable or disable the Save button based on the above check
    tr.find('.btn-save').prop('disabled', !allFilled);
});
$(".btn-cancel").click(function () {
    var tr = $(this).closest('tr');
    tr.find('.editField').hide();
    tr.find('.data').show();
    tr.find('.btn-edit').show();
    tr.find('.btn-save').prop('disabled', true);
    tr.find('.btn-save, .btn-cancel').hide();
});

$(".btn-save").click(function () {
    $(this).closest('tr').find('.editField').attr('required', true);
    var tr = $(this).closest('tr');
    var updatedData = {
        Id: tr.find('input[name="Id"]').val(),
        PayrollNumber: tr.find('input[name="PayrollNumber"]').val(),
        Forenames: tr.find('input[name="Forenames"]').val(),
        Surname: tr.find('input[name="Surname"]').val(),
        DateOfBirth: tr.find('input[name="DateOfBirth"]').val(),
        Telephone: tr.find('input[name="Telephone"]').val(),
        Mobile: tr.find('input[name="Mobile"]').val(),
        Address: tr.find('input[name="Address"]').val(),
        Address2: tr.find('input[name="Address2"]').val(),
        Postcode: tr.find('input[name="Postcode"]').val(),
        EmailHome: tr.find('input[name="EmailHome"]').val(),
        StartDate: tr.find('input[name="StartDate"]').val(),
    };

    $.ajax({
        url: '/Home/SaveEditedData',
        type: 'POST',
        data: updatedData,
        success: function (response) {
            if (response.success) {
                
                tr.find('.editField').each(function () {
                    var span = $(this).siblings('.data');
                    span.text($(this).val());
                });
                tr.find('.editField').hide();
                tr.find('.data').show();
                tr.find('.btn-edit').show();
                tr.find('.btn-save, .btn-cancel').hide();
                alert(response.message);
            } else {
                // Handle error. Maybe show an alert or a notification
                alert(response.message || 'An error occurred while saving.');
            }
        },
        error: function () {
            // This is the case if the AJAX request itself failed, e.g., no response from server
            alert('Failed to save. Please try again later.');
        }
    });
    tr.find('.editField').each(function () {
        var span = $(this).siblings('.data');
        span.text($(this).val());
    });
    tr.find('.editField').hide();
    tr.find('.data').show();
    tr.find('.btn-edit').show();
    tr.find('.btn-save, .btn-cancel').hide();
});