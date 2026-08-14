var objectivesViews = ['table', 'board', 'focus'];

function setObjectivesView(mode) {
    objectivesViews.forEach(function (view) {
        var isActive = view === mode;
        $('#' + view + 'View').toggleClass('d-none', !isActive);
        $('#' + view + 'ViewBtn').toggleClass('active', isActive);
    });
    localStorage.setItem('objectivesViewMode', mode);
}

function updateColumnCount($column) {
    var count = $column.find('.kanban-card').length;
    $column.find('.kanban-count-badge').text(count);
    $column.find('.kanban-empty-hint').toggle(count === 0);
}

function updateTwentyPercentBadges(objectiveId, isTwentyPercent) {
    $('[data-objective-id="' + objectiveId + '"] .twenty-percent-badge').toggleClass('d-none', !isTwentyPercent);
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

function initObjectivesFocusDragAndDrop() {
    var $board = $('#focusView');
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
            var previousIsTwentyPercent = $originalColumn.data('twenty-percent') === true || $originalColumn.data('twenty-percent') === 'true';
            var isTwentyPercent = $column.data('twenty-percent') === true || $column.data('twenty-percent') === 'true';
            var orderedIds = $column.find('.kanban-card').map(function () {
                return $(this).data('objective-id');
            }).get();

            var $failedColumn = $originalColumn;
            var $failedNextSibling = $originalNextSibling;
            var $failedCard = $draggedCard;

            updateTwentyPercentBadges(objectiveId, isTwentyPercent);

            $.ajax({
                url: '/Objectives/ReorderFocus',
                method: 'POST',
                contentType: 'application/json',
                headers: { 'X-CSRF-TOKEN': antiForgeryToken },
                data: JSON.stringify({ id: objectiveId, isTwentyPercent: isTwentyPercent, orderedIds: orderedIds })
            }).fail(function () {
                if ($failedNextSibling && $failedNextSibling.length) {
                    $failedNextSibling.before($failedCard);
                } else {
                    $failedColumn.find('.kanban-cards').append($failedCard);
                }
                updateColumnCount($failedColumn);
                updateColumnCount($column);
                updateTwentyPercentBadges(objectiveId, previousIsTwentyPercent);
                alert('Could not move the objective. Please try again.');
            });

            $draggedCard = null;
            $originalColumn = null;
            $originalNextSibling = null;
        });
    });
}

function applyObjectivesFocusColumnOrder(isSwapped) {
    $('#focusView .col[data-group="twentyPercent"]').css('order', isSwapped ? 2 : 1);
    $('#focusView .col[data-group="normal"]').css('order', isSwapped ? 1 : 2);
}

function initObjectivesFocusColumnSwap() {
    var $btn = $('#focusSwapColumnsBtn');
    if ($btn.length === 0) {
        return;
    }

    var isSwapped = localStorage.getItem('objectivesFocusColumnOrderSwapped') === 'true';
    applyObjectivesFocusColumnOrder(isSwapped);

    $btn.on('click', function () {
        isSwapped = !isSwapped;
        applyObjectivesFocusColumnOrder(isSwapped);
        localStorage.setItem('objectivesFocusColumnOrderSwapped', isSwapped);
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

function initEditObjectiveModal() {
    var $modalEl = $('#editObjectiveModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#solutionAntiForgeryToken').val();
    var $form = $('#editObjectiveForm');
    var $error = $('#editObjectiveError');

    $(document).on('click', '.edit-objective-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#editObjectiveId').val($btn.data('id'));
        $('#editObjectiveText').val($btn.data('text'));
        $('#editObjectiveStatus').val($btn.data('status'));
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var payload = {
            id: parseInt($('#editObjectiveId').val(), 10),
            text: $('#editObjectiveText').val(),
            status: $('#editObjectiveStatus').val()
        };

        $.ajax({
            url: '/Objectives/Edit',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify(payload)
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not save the objective. Please try again.');
        });
    });
}

function initDeleteObjectiveModal() {
    var $modalEl = $('#deleteObjectiveModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#solutionAntiForgeryToken').val();
    var $error = $('#deleteObjectiveError');

    $(document).on('click', '.objective-delete', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#deleteObjectiveId').val($btn.data('objective-id'));
        $('#deleteObjectiveText').text($btn.data('text'));
        modal.show();
    });

    $('#confirmDeleteObjectiveBtn').on('click', function () {
        $error.addClass('d-none').text('');
        var objectiveId = parseInt($('#deleteObjectiveId').val(), 10);

        $.ajax({
            url: '/Objectives/Delete',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: objectiveId })
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not delete the objective. Please try again.');
        });
    });
}

$(function () {
    $('.view-toggle-btn').on('click', function () {
        setObjectivesView($(this).data('view'));
    });

    var savedMode = localStorage.getItem('objectivesViewMode');
    if (objectivesViews.includes(savedMode)) {
        setObjectivesView(savedMode);
    }

    initObjectivesKanbanDragAndDrop();
    initObjectivesFocusDragAndDrop();
    initObjectivesFocusColumnSwap();
    initAddObjectiveModal();
    initEditObjectiveModal();
    initDeleteObjectiveModal();
});
