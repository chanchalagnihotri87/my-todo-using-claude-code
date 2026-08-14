function initAddTaskModal() {
    var $modalEl = $('#addTaskModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#taskAntiForgeryToken').val();
    var $form = $('#addTaskForm');
    var $error = $('#addTaskError');

    $(document).on('click', '.add-task-btn', function () {
        $error.addClass('d-none').text('');
        $('#newTaskName').val('');
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var objectiveId = $('#tasksObjectiveId').val();
        var name = $('#newTaskName').val().trim();
        if (!name) {
            return;
        }

        $.ajax({
            url: '/Tasks/Create',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ objectiveId: objectiveId, name: name })
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not add the task. Please try again.');
        });
    });
}

function initEditTaskModal() {
    var $modalEl = $('#editTaskModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#taskAntiForgeryToken').val();
    var $form = $('#editTaskForm');
    var $error = $('#editTaskError');

    $(document).on('click', '.edit-task-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#editTaskId').val($btn.data('id'));
        $('#editTaskName').val($btn.data('name'));
        $('#editTaskStatus').val($btn.data('status'));
        $('#editTaskSprintId').val($btn.data('sprint-id') || '');
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var sprintIdValue = $('#editTaskSprintId').val();

        var payload = {
            id: parseInt($('#editTaskId').val(), 10),
            name: $('#editTaskName').val(),
            status: parseInt($('#editTaskStatus').val(), 10),
            sprintId: sprintIdValue ? parseInt(sprintIdValue, 10) : null
        };

        $.ajax({
            url: '/Tasks/Edit',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify(payload)
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not save the task. Please try again.');
        });
    });
}

function initDeleteTaskModal() {
    var $modalEl = $('#deleteTaskModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#taskAntiForgeryToken').val();
    var $error = $('#deleteTaskError');

    $(document).on('click', '.delete-task-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#deleteTaskId').val($btn.data('id'));
        $('#deleteTaskName').text($btn.data('name'));
        modal.show();
    });

    $('#confirmDeleteTaskBtn').on('click', function () {
        $error.addClass('d-none').text('');
        var taskId = parseInt($('#deleteTaskId').val(), 10);

        $.ajax({
            url: '/Tasks/Delete',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: taskId })
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not delete the task. Please try again.');
        });
    });
}

$(function () {
    var antiForgeryToken = $('#taskAntiForgeryToken').val();

    initAddTaskModal();
    initEditTaskModal();
    initDeleteTaskModal();

    $('.task-sprint-select').each(function () {
        $(this).data('previous-value', $(this).val());
    });

    var TODO_STATUS_COMPLETED = 3;

    $('.task-status-select').each(function () {
        $(this).data('previous-value', $(this).val());
    });

    $(document).on('change', '.task-status-select', function () {
        var $select = $(this);
        var taskId = $select.data('task-id');
        var status = parseInt($select.val(), 10);
        var previousValue = $select.data('previous-value');
        var $name = $select.closest('tr').find('span');

        $name.toggleClass('text-muted', status === TODO_STATUS_COMPLETED);

        $.ajax({
            url: '/Tasks/UpdateStatus',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: taskId, status: status })
        }).done(function () {
            $select.data('previous-value', $select.val());
        }).fail(function () {
            $select.val(previousValue);
            $name.toggleClass('text-muted', parseInt(previousValue, 10) === TODO_STATUS_COMPLETED);
            alert('Could not update the task. Please try again.');
        });
    });

    $(document).on('change', '.task-sprint-select', function () {
        var $select = $(this);
        var taskId = $select.data('task-id');
        var sprintId = $select.val() ? parseInt($select.val(), 10) : null;
        var previousValue = $select.data('previous-value') || '';

        $.ajax({
            url: '/Tasks/UpdateSprint',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: taskId, sprintId: sprintId })
        }).done(function () {
            $select.data('previous-value', $select.val());
        }).fail(function () {
            $select.val(previousValue);
            alert('Could not update the sprint. Please try again.');
        });
    });
});
