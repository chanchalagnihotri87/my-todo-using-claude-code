var solutionsViews = ['table', 'board', 'focus'];

function setSolutionsView(mode) {
    solutionsViews.forEach(function (view) {
        var isActive = view === mode;
        $('#' + view + 'View').toggleClass('d-none', !isActive);
        $('#' + view + 'ViewBtn').toggleClass('active', isActive);
    });
    localStorage.setItem('solutionsViewMode', mode);
}

function updateTwentyPercentBadges(solutionId, isTwentyPercent) {
    $('[data-solution-id="' + solutionId + '"] .twenty-percent-badge').toggleClass('d-none', !isTwentyPercent);
}

function updateColumnCount($column) {
    var count = $column.find('.kanban-card').length;
    $column.find('.kanban-count-badge').text(count);
    $column.find('.kanban-empty-hint').toggle(count === 0);
}

function initSolutionsKanbanDragAndDrop() {
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
        e.originalEvent.dataTransfer.setData('text/plain', $draggedCard.data('solution-id'));
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

            var solutionId = $draggedCard.data('solution-id');
            var newStatus = $column.data('status');
            var orderedIds = $column.find('.kanban-card').map(function () {
                return $(this).data('solution-id');
            }).get();

            var $failedColumn = $originalColumn;
            var $failedNextSibling = $originalNextSibling;
            var $failedCard = $draggedCard;

            $.ajax({
                url: '/Solutions/Reorder',
                method: 'POST',
                contentType: 'application/json',
                headers: { 'X-CSRF-TOKEN': antiForgeryToken },
                data: JSON.stringify({ id: solutionId, status: newStatus, orderedIds: orderedIds })
            }).fail(function () {
                if ($failedNextSibling && $failedNextSibling.length) {
                    $failedNextSibling.before($failedCard);
                } else {
                    $failedColumn.find('.kanban-cards').append($failedCard);
                }
                updateColumnCount($failedColumn);
                updateColumnCount($column);
                alert('Could not reorder the solution. Please try again.');
            });

            $draggedCard = null;
            $originalColumn = null;
            $originalNextSibling = null;
        });
    });
}

function initFocusBoardDragAndDrop() {
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
        e.originalEvent.dataTransfer.setData('text/plain', $draggedCard.data('solution-id'));
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

            var solutionId = $draggedCard.data('solution-id');
            var previousIsTwentyPercent = $originalColumn.data('twenty-percent') === true || $originalColumn.data('twenty-percent') === 'true';
            var isTwentyPercent = $column.data('twenty-percent') === true || $column.data('twenty-percent') === 'true';
            var orderedIds = $column.find('.kanban-card').map(function () {
                return $(this).data('solution-id');
            }).get();

            var $failedColumn = $originalColumn;
            var $failedNextSibling = $originalNextSibling;
            var $failedCard = $draggedCard;

            updateTwentyPercentBadges(solutionId, isTwentyPercent);

            $.ajax({
                url: '/Solutions/ReorderFocus',
                method: 'POST',
                contentType: 'application/json',
                headers: { 'X-CSRF-TOKEN': antiForgeryToken },
                data: JSON.stringify({ id: solutionId, isTwentyPercent: isTwentyPercent, orderedIds: orderedIds })
            }).fail(function () {
                if ($failedNextSibling && $failedNextSibling.length) {
                    $failedNextSibling.before($failedCard);
                } else {
                    $failedColumn.find('.kanban-cards').append($failedCard);
                }
                updateColumnCount($failedColumn);
                updateColumnCount($column);
                updateTwentyPercentBadges(solutionId, previousIsTwentyPercent);
                alert('Could not move the solution. Please try again.');
            });

            $draggedCard = null;
            $originalColumn = null;
            $originalNextSibling = null;
        });
    });
}

function applyFocusColumnOrder(isSwapped) {
    $('#focusView .col[data-group="twentyPercent"]').css('order', isSwapped ? 2 : 1);
    $('#focusView .col[data-group="normal"]').css('order', isSwapped ? 1 : 2);
}

function initFocusColumnSwap() {
    var $btn = $('#focusSwapColumnsBtn');
    if ($btn.length === 0) {
        return;
    }

    var isSwapped = localStorage.getItem('focusColumnOrderSwapped') === 'true';
    applyFocusColumnOrder(isSwapped);

    $btn.on('click', function () {
        isSwapped = !isSwapped;
        applyFocusColumnOrder(isSwapped);
        localStorage.setItem('focusColumnOrderSwapped', isSwapped);
    });
}

$(function () {
    $('.view-toggle-btn').on('click', function () {
        setSolutionsView($(this).data('view'));
    });

    var savedMode = localStorage.getItem('solutionsViewMode');
    if (solutionsViews.includes(savedMode)) {
        setSolutionsView(savedMode);
    }

    initSolutionsKanbanDragAndDrop();
    initFocusBoardDragAndDrop();
    initFocusColumnSwap();
});
