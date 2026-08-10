var problemsViews = ['card', 'table', 'board'];

function setProblemsView(mode) {
    problemsViews.forEach(function (view) {
        var isActive = view === mode;
        $('#' + view + 'View').toggleClass('d-none', !isActive);
        $('#' + view + 'ViewBtn').toggleClass('active', isActive);
    });
    localStorage.setItem('problemsViewMode', mode);
}

function initKanbanDragAndDrop() {
    var $board = $('#boardView');
    if ($board.length === 0) {
        return;
    }

    var antiForgeryToken = $('#antiForgeryToken').val();
    var $draggedCard = null;
    var $draggedColumn = null;
    var $draggedColumnWrapper = null;

    $board.on('dragstart', '.kanban-card', function (e) {
        $draggedCard = $(this);
        $draggedCard.addClass('dragging');
        e.originalEvent.dataTransfer.effectAllowed = 'move';
        e.originalEvent.dataTransfer.setData('text/plain', $draggedCard.data('problem-id'));
    });

    $board.on('dragend', '.kanban-card', function () {
        $(this).removeClass('dragging');
        $draggedCard = null;
    });

    $board.on('dragstart', '.kanban-column-handle', function (e) {
        $draggedColumn = $(this).closest('.kanban-column');
        $draggedColumnWrapper = $draggedColumn.closest('.col');
        $draggedColumn.addClass('column-dragging');
        e.originalEvent.dataTransfer.effectAllowed = 'move';
        e.originalEvent.dataTransfer.setData('text/plain', 'column:' + $draggedColumn.data('status'));
    });

    $board.on('dragend', '.kanban-column-handle', function () {
        if ($draggedColumn) {
            $draggedColumn.removeClass('column-dragging');
        }
        $draggedColumn = null;
        $draggedColumnWrapper = null;
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

            if ($draggedColumnWrapper) {
                var $targetWrapper = $column.closest('.col');
                if ($targetWrapper.is($draggedColumnWrapper)) {
                    return;
                }

                var $previousSibling = $draggedColumnWrapper.prev();
                var $row = $draggedColumnWrapper.parent();

                if ($draggedColumnWrapper.index() < $targetWrapper.index()) {
                    $draggedColumnWrapper.insertAfter($targetWrapper);
                } else {
                    $draggedColumnWrapper.insertBefore($targetWrapper);
                }

                var orderedStatuses = $row.find('.kanban-column').map(function () {
                    return $(this).data('status');
                }).get();

                $.ajax({
                    url: '/Problems/ReorderLists',
                    method: 'POST',
                    contentType: 'application/json',
                    headers: { 'X-CSRF-TOKEN': antiForgeryToken },
                    data: JSON.stringify({ orderedStatuses: orderedStatuses })
                }).fail(function () {
                    if ($previousSibling.length) {
                        $draggedColumnWrapper.insertAfter($previousSibling);
                    } else {
                        $row.prepend($draggedColumnWrapper);
                    }
                    alert('Could not save the new list order. Please try again.');
                });

                return;
            }

            if (!$draggedCard) {
                return;
            }

            var $sourceColumn = $draggedCard.closest('.kanban-column');
            var newStatus = $column.data('status');

            if ($sourceColumn.is($column)) {
                return;
            }

            var problemId = $draggedCard.data('problem-id');
            $column.find('.kanban-cards').append($draggedCard);
            updateColumnCount($sourceColumn);
            updateColumnCount($column);

            $.ajax({
                url: '/Problems/UpdateStatus',
                method: 'POST',
                contentType: 'application/json',
                headers: { 'X-CSRF-TOKEN': antiForgeryToken },
                data: JSON.stringify({ id: problemId, status: newStatus })
            }).fail(function () {
                $sourceColumn.find('.kanban-cards').append($draggedCard);
                updateColumnCount($sourceColumn);
                updateColumnCount($column);
                alert('Could not update the problem status. Please try again.');
            });
        });
    });
}

function initProblemTagToggles() {
    var $antiForgeryInput = $('#antiForgeryToken');
    if ($antiForgeryInput.length === 0) {
        return;
    }
    var antiForgeryToken = $antiForgeryInput.val();

    $(document).on('click', '.tag-badge', function () {
        var $badge = $(this);
        var problemId = $badge.data('problem-id');
        var tag = $badge.data('tag');
        var url = tag === 'urgent' ? '/Problems/ToggleUrgent' : '/Problems/ToggleImportant';
        var $allBadgesForTag = $('.tag-badge[data-problem-id="' + problemId + '"][data-tag="' + tag + '"]');
        var wasActive = $badge.hasClass('tag-active');

        $allBadgesForTag.toggleClass('tag-active', !wasActive).toggleClass('tag-inactive', wasActive);

        $.ajax({
            url: url,
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: problemId })
        }).fail(function () {
            $allBadgesForTag.toggleClass('tag-active', wasActive).toggleClass('tag-inactive', !wasActive);
            alert('Could not update the tag. Please try again.');
        });
    });
}

function initEditProblemModal() {
    var $modalEl = $('#editProblemModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#antiForgeryToken').val();
    var $form = $('#editProblemForm');
    var $error = $('#editProblemError');

    $(document).on('click', '.edit-problem-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#editProblemId').val($btn.data('id'));
        $('#editProblemName').val($btn.data('name'));
        $('#editProblemDescription').val($btn.data('description'));
        $('#editProblemStatus').val($btn.data('status'));
        $('#editProblemIsUrgent').prop('checked', $btn.data('is-urgent') === true);
        $('#editProblemIsImportant').prop('checked', $btn.data('is-important') === true);
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var payload = {
            id: parseInt($('#editProblemId').val(), 10),
            name: $('#editProblemName').val(),
            description: $('#editProblemDescription').val(),
            status: $('#editProblemStatus').val(),
            isUrgent: $('#editProblemIsUrgent').is(':checked'),
            isImportant: $('#editProblemIsImportant').is(':checked')
        };

        $.ajax({
            url: '/Problems/Edit',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify(payload)
        }).done(function () {
            modal.hide();
            window.location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not save the problem. Please try again.');
        });
    });
}

function initDeleteProblemModal() {
    var $modalEl = $('#deleteProblemModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#antiForgeryToken').val();
    var $error = $('#deleteProblemError');

    $(document).on('click', '.delete-problem-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#deleteProblemId').val($btn.data('id'));
        $('#deleteProblemName').text($btn.data('name'));
        modal.show();
    });

    $('#confirmDeleteProblemBtn').on('click', function () {
        $error.addClass('d-none').text('');
        var problemId = parseInt($('#deleteProblemId').val(), 10);

        $.ajax({
            url: '/Problems/Delete',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: problemId })
        }).done(function () {
            modal.hide();
            window.location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not delete the problem. Please try again.');
        });
    });
}

function updateColumnCount($column) {
    var count = $column.find('.kanban-card').length;
    $column.find('.kanban-count-badge').text(count);
    $column.find('.kanban-empty-hint').toggle(count === 0);
}

$(function () {
    $('.view-toggle-btn').on('click', function () {
        setProblemsView($(this).data('view'));
    });

    var savedMode = localStorage.getItem('problemsViewMode');
    if (problemsViews.includes(savedMode)) {
        setProblemsView(savedMode);
    }

    initKanbanDragAndDrop();
    initProblemTagToggles();
    initEditProblemModal();
    initDeleteProblemModal();
});
