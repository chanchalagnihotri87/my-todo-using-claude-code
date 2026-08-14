$(function () {
    var antiForgeryToken = $('#sprintDetailsAntiForgeryToken').val();

    $('.todo-date-input').each(function () {
        $(this).data('previous-value', $(this).val());
    });

    function getWeekdayLabel(dateStr) {
        return new Date(dateStr + 'T00:00:00').toLocaleDateString('en-US', { weekday: 'short' });
    }

    $(document).on('change', '.todo-date-input', function () {
        var $input = $(this);
        var $dayLabel = $input.siblings('.todo-day-label');
        var todoId = $input.data('todo-id');
        var todoDate = $input.val();
        var previousValue = $input.data('previous-value');

        $dayLabel.text(getWeekdayLabel(todoDate));

        $.ajax({
            url: '/Tasks/UpdateTodoDate',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: todoId, todoDate: todoDate })
        }).done(function () {
            $input.data('previous-value', todoDate);
        }).fail(function () {
            $input.val(previousValue);
            $dayLabel.text(getWeekdayLabel(previousValue));
            alert('Could not update the todo date. Please try again.');
        });
    });

    $(document).on('click', '.add-to-todo-btn', function () {
        var $btn = $(this);
        var taskId = $btn.data('task-id');

        $btn.prop('disabled', true);

        $.ajax({
            url: '/Tasks/AddToTodo',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: taskId })
        }).done(function () {
            location.reload();
        }).fail(function () {
            $btn.prop('disabled', false);
            alert('Could not add the task to Todo. Please try again.');
        });
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
});
