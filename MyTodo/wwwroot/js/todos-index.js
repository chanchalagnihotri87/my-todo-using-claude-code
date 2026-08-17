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

function initTodosTableRowReorder(antiForgeryToken) {
    var $tbody = $('#todosTableBody');
    if ($tbody.length === 0) {
        return;
    }

    var $draggedRow = null;
    var originalOrder = [];

    function getRowOrder() {
        return $tbody.find('.todo-row').map(function () {
            return $(this).data('todo-id');
        }).get();
    }

    $tbody.on('dragstart', '.todo-row', function (e) {
        $draggedRow = $(this);
        originalOrder = getRowOrder();
        $draggedRow.addClass('dragging');
        e.originalEvent.dataTransfer.effectAllowed = 'move';
        e.originalEvent.dataTransfer.setData('text/plain', $draggedRow.data('todo-id'));
    });

    $tbody.on('dragend', '.todo-row', function () {
        $(this).removeClass('dragging');
        $tbody.find('.todo-row').removeClass('drag-over-top drag-over-bottom');
    });

    $tbody.on('dragover', '.todo-row', function (e) {
        if (!$draggedRow) {
            return;
        }

        var $target = $(this);
        if ($target.is($draggedRow)) {
            return;
        }

        e.preventDefault();
        e.originalEvent.dataTransfer.dropEffect = 'move';

        var rect = this.getBoundingClientRect();
        var isAfter = (e.originalEvent.clientY - rect.top) > rect.height / 2;

        $tbody.find('.todo-row').removeClass('drag-over-top drag-over-bottom');
        $target.addClass(isAfter ? 'drag-over-bottom' : 'drag-over-top');
    });

    $tbody.on('drop', '.todo-row', function (e) {
        e.preventDefault();

        var $target = $(this);
        $tbody.find('.todo-row').removeClass('drag-over-top drag-over-bottom');

        if (!$draggedRow || $target.is($draggedRow)) {
            return;
        }

        var rect = this.getBoundingClientRect();
        var isAfter = (e.originalEvent.clientY - rect.top) > rect.height / 2;

        if (isAfter) {
            $target.after($draggedRow);
        } else {
            $target.before($draggedRow);
        }

        var newOrder = getRowOrder();
        var $failedRow = $draggedRow;

        $.ajax({
            url: '/Todos/Reorder',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ orderedIds: newOrder })
        }).fail(function () {
            var $rows = {};
            $tbody.find('.todo-row').each(function () {
                $rows[$(this).data('todo-id')] = $(this);
            });
            originalOrder.forEach(function (id) {
                $tbody.append($rows[id]);
            });
            alert('Could not save the new order. Please try again.');
        });

        $draggedRow = null;
        originalOrder = [];
    });
}

function initTodosHistoryRowReorder(antiForgeryToken) {
    var $tbody = $('#todosHistoryTableBody');
    if ($tbody.length === 0) {
        return;
    }

    var $draggedRow = null;
    var draggedDate = null;
    var originalOrder = [];

    function getRowOrderForDate(date) {
        return $tbody.find('.todo-history-row[data-todo-date="' + date + '"]').map(function () {
            return $(this).data('todo-id');
        }).get();
    }

    $tbody.on('dragstart', '.todo-history-row', function (e) {
        $draggedRow = $(this);
        draggedDate = $draggedRow.data('todo-date');
        originalOrder = getRowOrderForDate(draggedDate);
        $draggedRow.addClass('dragging');
        e.originalEvent.dataTransfer.effectAllowed = 'move';
        e.originalEvent.dataTransfer.setData('text/plain', $draggedRow.data('todo-id'));
    });

    $tbody.on('dragend', '.todo-history-row', function () {
        $(this).removeClass('dragging');
        $tbody.find('.todo-history-row').removeClass('drag-over-top drag-over-bottom');
    });

    $tbody.on('dragover', '.todo-history-row', function (e) {
        if (!$draggedRow) {
            return;
        }

        var $target = $(this);
        if ($target.is($draggedRow) || $target.data('todo-date') !== draggedDate) {
            return;
        }

        e.preventDefault();
        e.originalEvent.dataTransfer.dropEffect = 'move';

        var rect = this.getBoundingClientRect();
        var isAfter = (e.originalEvent.clientY - rect.top) > rect.height / 2;

        $tbody.find('.todo-history-row').removeClass('drag-over-top drag-over-bottom');
        $target.addClass(isAfter ? 'drag-over-bottom' : 'drag-over-top');
    });

    $tbody.on('drop', '.todo-history-row', function (e) {
        var $target = $(this);
        $tbody.find('.todo-history-row').removeClass('drag-over-top drag-over-bottom');

        if (!$draggedRow || $target.is($draggedRow) || $target.data('todo-date') !== draggedDate) {
            return;
        }

        e.preventDefault();

        var rect = this.getBoundingClientRect();
        var isAfter = (e.originalEvent.clientY - rect.top) > rect.height / 2;

        if (isAfter) {
            $target.after($draggedRow);
        } else {
            $target.before($draggedRow);
        }

        var newOrder = getRowOrderForDate(draggedDate);

        $.ajax({
            url: '/Todos/Reorder',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ orderedIds: newOrder })
        }).fail(function () {
            var $rows = {};
            var $group = $tbody.find('.todo-history-row[data-todo-date="' + draggedDate + '"]');
            $group.each(function () {
                $rows[$(this).data('todo-id')] = $(this);
            });
            var $groupEnd = $group.last().next();

            originalOrder.forEach(function (id) {
                if ($groupEnd.length) {
                    $groupEnd.before($rows[id]);
                } else {
                    $tbody.append($rows[id]);
                }
            });
            alert('Could not save the new order. Please try again.');
        });

        $draggedRow = null;
        draggedDate = null;
        originalOrder = [];
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
    initTodosTableRowReorder(antiForgeryToken);
    initTodosHistoryRowReorder(antiForgeryToken);
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
