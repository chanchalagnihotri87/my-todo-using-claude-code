var todosViews = ['table', 'board'];

function setTodosView(mode) {
    todosViews.forEach(function (view) {
        var isActive = view === mode;
        $('#' + view + 'View').toggleClass('d-none', !isActive);
        $('#' + view + 'ViewBtn').toggleClass('active', isActive);
    });
    localStorage.setItem('todosViewMode', mode);
}

function updateTodosColumnCount($column) {
    var count = $column.find('.kanban-card').length;
    $column.find('.kanban-count-badge').text(count);
    $column.find('.kanban-empty-hint').toggle(count === 0);
}

function initTodosKanbanDragAndDrop(antiForgeryToken) {
    var $board = $('#boardView');
    if ($board.length === 0) {
        return;
    }

    var $draggedCard = null;
    var $originalColumn = null;
    var $originalNextSibling = null;

    $board.on('dragstart', '.kanban-card', function (e) {
        $draggedCard = $(this);
        $originalColumn = $draggedCard.closest('.kanban-column');
        $originalNextSibling = $draggedCard.next();
        $draggedCard.addClass('dragging');
        e.originalEvent.dataTransfer.effectAllowed = 'move';
        e.originalEvent.dataTransfer.setData('text/plain', $draggedCard.data('task-id'));
    });

    $board.on('dragend', '.kanban-card', function () {
        $(this).removeClass('dragging');
    });

    $board.on('dragover', '.kanban-card', function (e) {
        if (!$draggedCard) {
            return;
        }

        var $target = $(this);
        if ($target.is($draggedCard)) {
            return;
        }

        var rect = this.getBoundingClientRect();
        var isAfter = (e.originalEvent.clientY - rect.top) > rect.height / 2;
        if (isAfter) {
            $target.after($draggedCard);
        } else {
            $target.before($draggedCard);
        }
    });

    $board.find('.kanban-column').each(function () {
        var $column = $(this);

        $column.on('dragover', function (e) {
            e.preventDefault();
            e.originalEvent.dataTransfer.dropEffect = 'move';
            $column.addClass('drag-over');
        });

        $column.on('dragleave', function () {
            $column.removeClass('drag-over');
        });

        $column.on('drop', function (e) {
            e.preventDefault();
            $column.removeClass('drag-over');

            if (!$draggedCard) {
                return;
            }

            if ($draggedCard.closest('.kanban-column')[0] !== $column[0]) {
                $column.find('.kanban-cards').append($draggedCard);
            }

            updateTodosColumnCount($originalColumn);
            updateTodosColumnCount($column);

            var taskId = $draggedCard.data('task-id');
            var newStatus = $column.data('status');

            var $failedColumn = $originalColumn;
            var $failedNextSibling = $originalNextSibling;
            var $failedCard = $draggedCard;

            $.ajax({
                url: '/Tasks/UpdateStatus',
                method: 'POST',
                contentType: 'application/json',
                headers: { 'X-CSRF-TOKEN': antiForgeryToken },
                data: JSON.stringify({ id: taskId, status: newStatus })
            }).fail(function () {
                if ($failedNextSibling && $failedNextSibling.length) {
                    $failedNextSibling.before($failedCard);
                } else {
                    $failedColumn.find('.kanban-cards').append($failedCard);
                }
                updateTodosColumnCount($failedColumn);
                updateTodosColumnCount($column);
                alert('Could not update the task. Please try again.');
            });

            $draggedCard = null;
            $originalColumn = null;
            $originalNextSibling = null;
        });
    });
}

function initTodoTagToggles(antiForgeryToken) {
    $(document).on('click', '.tag-badge', function () {
        var $badge = $(this);
        var todoId = $badge.data('todo-id');
        var tag = $badge.data('tag');
        var url = tag === 'urgent' ? '/Todos/ToggleUrgent' : (tag === 'important' ? '/Todos/ToggleImportant' : '/Todos/ToggleFrog');
        var $allBadgesForTag = $('.tag-badge[data-todo-id="' + todoId + '"][data-tag="' + tag + '"]');
        var wasActive = $badge.hasClass('tag-active');

        $allBadgesForTag.toggleClass('tag-active', !wasActive).toggleClass('tag-inactive', wasActive);

        $.ajax({
            url: url,
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: todoId })
        }).done(function () {
            if (tag === 'frog' && !wasActive) {
                $('.tag-frog.tag-active').not($allBadgesForTag).toggleClass('tag-active', false).toggleClass('tag-inactive', true);
            }
        }).fail(function () {
            $allBadgesForTag.toggleClass('tag-active', wasActive).toggleClass('tag-inactive', !wasActive);
            alert('Could not update the tag. Please try again.');
        });
    });
}

$(function () {
    var antiForgeryToken = $('#todosAntiForgeryToken').val();
    var TODO_STATUS_COMPLETED = 3;

    $('.view-toggle-btn').on('click', function () {
        setTodosView($(this).data('view'));
    });

    var savedTodosView = localStorage.getItem('todosViewMode');
    if (todosViews.includes(savedTodosView)) {
        setTodosView(savedTodosView);
    }

    initTodosKanbanDragAndDrop(antiForgeryToken);
    initTodoTagToggles(antiForgeryToken);

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
