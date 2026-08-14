function initAddSprintModal() {
    var $modalEl = $('#addSprintModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#sprintAntiForgeryToken').val();
    var $form = $('#addSprintForm');
    var $error = $('#addSprintError');

    $(document).on('click', '.add-sprint-btn', function () {
        $error.addClass('d-none').text('');
        $('#newSprintName').val('');
        $('#newSprintDescription').val('');
        $('#newSprintStartDate').val('');
        $('#newSprintEndDate').val('');
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var name = $('#newSprintName').val().trim();
        var description = $('#newSprintDescription').val().trim();
        var startDate = $('#newSprintStartDate').val();
        var endDate = $('#newSprintEndDate').val();
        if (!name || !startDate || !endDate) {
            return;
        }

        $.ajax({
            url: '/Sprints/Create',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ name: name, description: description, startDate: startDate, endDate: endDate })
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not add the sprint. Please try again.');
        });
    });
}

function initEditSprintModal() {
    var $modalEl = $('#editSprintModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#sprintAntiForgeryToken').val();
    var $form = $('#editSprintForm');
    var $error = $('#editSprintError');

    $(document).on('click', '.edit-sprint-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#editSprintId').val($btn.data('id'));
        $('#editSprintName').val($btn.data('name'));
        $('#editSprintDescription').val($btn.data('description'));
        $('#editSprintStartDate').val($btn.data('start-date'));
        $('#editSprintEndDate').val($btn.data('end-date'));
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var payload = {
            id: parseInt($('#editSprintId').val(), 10),
            name: $('#editSprintName').val(),
            description: $('#editSprintDescription').val(),
            startDate: $('#editSprintStartDate').val(),
            endDate: $('#editSprintEndDate').val()
        };

        $.ajax({
            url: '/Sprints/Edit',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify(payload)
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not save the sprint. Please try again.');
        });
    });
}

function initDeleteSprintModal() {
    var $modalEl = $('#deleteSprintModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#sprintAntiForgeryToken').val();
    var $error = $('#deleteSprintError');

    $(document).on('click', '.delete-sprint-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#deleteSprintId').val($btn.data('id'));
        $('#deleteSprintName').text($btn.data('name'));
        modal.show();
    });

    $('#confirmDeleteSprintBtn').on('click', function () {
        $error.addClass('d-none').text('');
        var sprintId = parseInt($('#deleteSprintId').val(), 10);

        $.ajax({
            url: '/Sprints/Delete',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: sprintId })
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not delete the sprint. Please try again.');
        });
    });
}

$(function () {
    initAddSprintModal();
    initEditSprintModal();
    initDeleteSprintModal();
});
