var objectivesViews = ['table', 'board'];

function setObjectivesView(mode) {
    objectivesViews.forEach(function (view) {
        var isActive = view === mode;
        $('#' + view + 'View').toggleClass('d-none', !isActive);
        $('#' + view + 'ViewBtn').toggleClass('active', isActive);
    });
    localStorage.setItem('objectivesViewMode', mode);
}

function updateObjectivesSummary() {
    var total = $('#tableView tbody tr[data-objective-id]').length;
    var completed = $('#tableView .objective-status-select').filter(function () {
        return $(this).val() === 'Completed';
    }).length;
    $('#objectivesSummary').text(completed + '/' + total + ' completed');
}

function updateColumnCount($column) {
    var count = $column.find('.kanban-card').length;
    $column.find('.kanban-count-badge').text(count);
    $column.find('.kanban-empty-hint').toggle(count === 0);
}

function initObjectivesKanbanDragAndDrop() {
    var $board = $('#boardView');
    if ($board.length === 0) {
        return;
    }

    var antiForgeryToken = $('#solutionAntiForgeryToken').val();
    var $draggedCard = null;
    var $originalColumn = null;
    var $originalNextSibling = null;

    $board.on('dragstart', '.kanban-card', function (e) {
        $draggedCard = $(this);
        $originalColumn = $draggedCard.closest('.kanban-column');
        $originalNextSibling = $draggedCard.next();
        $draggedCard.addClass('dragging');
        e.originalEvent.dataTransfer.effectAllowed = 'move';
        e.originalEvent.dataTransfer.setData('text/plain', $draggedCard.data('objective-id'));
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

            updateColumnCount($originalColumn);
            updateColumnCount($column);

            var objectiveId = $draggedCard.data('objective-id');
            var newStatus = $column.data('status');
            var orderedIds = $column.find('.kanban-card').map(function () {
                return $(this).data('objective-id');
            }).get();

            var $failedColumn = $originalColumn;
            var $failedNextSibling = $originalNextSibling;
            var $failedCard = $draggedCard;

            $.ajax({
                url: '/Objectives/Reorder',
                method: 'POST',
                contentType: 'application/json',
                headers: { 'X-CSRF-TOKEN': antiForgeryToken },
                data: JSON.stringify({ id: objectiveId, status: newStatus, orderedIds: orderedIds })
            }).fail(function () {
                if ($failedNextSibling && $failedNextSibling.length) {
                    $failedNextSibling.before($failedCard);
                } else {
                    $failedColumn.find('.kanban-cards').append($failedCard);
                }
                updateColumnCount($failedColumn);
                updateColumnCount($column);
                alert('Could not reorder the objective. Please try again.');
            });

            $draggedCard = null;
            $originalColumn = null;
            $originalNextSibling = null;
        });
    });
}

function initAddObjectiveModal() {
    var $modalEl = $('#addObjectiveModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#solutionAntiForgeryToken').val();
    var $form = $('#addObjectiveForm');
    var $error = $('#addObjectiveError');

    $(document).on('click', '.add-objective-btn', function () {
        $error.addClass('d-none').text('');
        $('#newObjectiveText').val('');
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var solutionId = $('#objectivesSolutionId').val();
        var text = $('#newObjectiveText').val().trim();
        if (!text) {
            return;
        }

        $.ajax({
            url: '/Objectives/Create',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ solutionId: solutionId, text: text })
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not add the objective. Please try again.');
        });
    });
}

$(function () {
    var antiForgeryToken = $('#solutionAntiForgeryToken').val();

    $('.view-toggle-btn').on('click', function () {
        setObjectivesView($(this).data('view'));
    });

    var savedMode = localStorage.getItem('objectivesViewMode');
    if (objectivesViews.includes(savedMode)) {
        setObjectivesView(savedMode);
    }

    initObjectivesKanbanDragAndDrop();
    initAddObjectiveModal();

    $(document).on('change', '.objective-status-select', function () {
        var $select = $(this);
        var objectiveId = $select.data('objective-id');
        var status = $select.val();
        var previousStatus = $select.data('previous-status') || 'NotStarted';
        var $text = $select.closest('tr').find('span');

        $text.toggleClass('text-muted', status === 'Completed');
        updateObjectivesSummary();

        $.ajax({
            url: '/Objectives/UpdateStatus',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: objectiveId, status: status })
        }).done(function () {
            $select.data('previous-status', status);
        }).fail(function () {
            $select.val(previousStatus);
            $text.toggleClass('text-muted', previousStatus === 'Completed');
            updateObjectivesSummary();
            alert('Could not update the objective. Please try again.');
        });
    });

    $(document).on('click', '.objective-delete', function () {
        var objectiveId = $(this).data('objective-id');

        $.ajax({
            url: '/Objectives/Delete',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: objectiveId })
        }).done(function () {
            location.reload();
        }).fail(function () {
            alert('Could not delete the objective. Please try again.');
        });
    });
});
