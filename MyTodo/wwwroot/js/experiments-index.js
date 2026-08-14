var experimentsViews = ['table', 'board'];

function setExperimentsView(mode) {
    experimentsViews.forEach(function (view) {
        var isActive = view === mode;
        $('#' + view + 'View').toggleClass('d-none', !isActive);
        $('#' + view + 'ViewBtn').toggleClass('active', isActive);
    });
    localStorage.setItem('experimentsViewMode', mode);
}

function updateColumnCount($column) {
    var count = $column.find('.kanban-card').length;
    $column.find('.kanban-count-badge').text(count);
    $column.find('.kanban-empty-hint').toggle(count === 0);
}

function initExperimentsKanbanDragAndDrop() {
    var $board = $('#boardView');
    if ($board.length === 0) {
        return;
    }

    var antiForgeryToken = $('#experimentAntiForgeryToken').val();
    var $draggedCard = null;
    var $originalColumn = null;
    var $originalNextSibling = null;

    $board.on('dragstart', '.kanban-card', function (e) {
        $draggedCard = $(this);
        $originalColumn = $draggedCard.closest('.kanban-column');
        $originalNextSibling = $draggedCard.next();
        $draggedCard.addClass('dragging');
        e.originalEvent.dataTransfer.effectAllowed = 'move';
        e.originalEvent.dataTransfer.setData('text/plain', $draggedCard.data('experiment-id'));
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

            var experimentId = $draggedCard.data('experiment-id');
            var newStatus = $column.data('status');
            var orderedIds = $column.find('.kanban-card').map(function () {
                return $(this).data('experiment-id');
            }).get();

            var $failedColumn = $originalColumn;
            var $failedNextSibling = $originalNextSibling;
            var $failedCard = $draggedCard;

            $.ajax({
                url: '/Experiments/Reorder',
                method: 'POST',
                contentType: 'application/json',
                headers: { 'X-CSRF-TOKEN': antiForgeryToken },
                data: JSON.stringify({ id: experimentId, status: newStatus, orderedIds: orderedIds })
            }).fail(function () {
                if ($failedNextSibling && $failedNextSibling.length) {
                    $failedNextSibling.before($failedCard);
                } else {
                    $failedColumn.find('.kanban-cards').append($failedCard);
                }
                updateColumnCount($failedColumn);
                updateColumnCount($column);
                alert('Could not reorder the experiment. Please try again.');
            });

            $draggedCard = null;
            $originalColumn = null;
            $originalNextSibling = null;
        });
    });
}

function initAddExperimentModal() {
    var $modalEl = $('#addExperimentModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#experimentAntiForgeryToken').val();
    var $form = $('#addExperimentForm');
    var $error = $('#addExperimentError');

    $(document).on('click', '.add-experiment-btn', function () {
        $error.addClass('d-none').text('');
        $('#newExperimentName').val('');
        $('#newExperimentDescription').val('');
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var solutionId = $('#experimentsSolutionId').val();
        var name = $('#newExperimentName').val().trim();
        var description = $('#newExperimentDescription').val().trim();
        if (!name) {
            return;
        }

        $.ajax({
            url: '/Experiments/Create',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ solutionId: solutionId, name: name, description: description })
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not add the experiment. Please try again.');
        });
    });
}

function initEditExperimentModal() {
    var $modalEl = $('#editExperimentModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#experimentAntiForgeryToken').val();
    var $form = $('#editExperimentForm');
    var $error = $('#editExperimentError');

    $(document).on('click', '.edit-experiment-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#editExperimentId').val($btn.data('id'));
        $('#editExperimentName').val($btn.data('name'));
        $('#editExperimentDescription').val($btn.data('description'));
        $('#editExperimentStatus').val($btn.data('status'));
        modal.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        $error.addClass('d-none').text('');

        var payload = {
            id: parseInt($('#editExperimentId').val(), 10),
            name: $('#editExperimentName').val(),
            description: $('#editExperimentDescription').val(),
            status: $('#editExperimentStatus').val()
        };

        $.ajax({
            url: '/Experiments/Edit',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify(payload)
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not save the experiment. Please try again.');
        });
    });
}

function initDeleteExperimentModal() {
    var $modalEl = $('#deleteExperimentModal');
    if ($modalEl.length === 0) {
        return;
    }

    var modal = new bootstrap.Modal($modalEl[0]);
    var antiForgeryToken = $('#experimentAntiForgeryToken').val();
    var $error = $('#deleteExperimentError');

    $(document).on('click', '.delete-experiment-btn', function () {
        var $btn = $(this);
        $error.addClass('d-none').text('');
        $('#deleteExperimentId').val($btn.data('id'));
        $('#deleteExperimentName').text($btn.data('name'));
        modal.show();
    });

    $('#confirmDeleteExperimentBtn').on('click', function () {
        $error.addClass('d-none').text('');
        var experimentId = parseInt($('#deleteExperimentId').val(), 10);

        $.ajax({
            url: '/Experiments/Delete',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'X-CSRF-TOKEN': antiForgeryToken },
            data: JSON.stringify({ id: experimentId })
        }).done(function () {
            modal.hide();
            location.reload();
        }).fail(function () {
            $error.removeClass('d-none').text('Could not delete the experiment. Please try again.');
        });
    });
}

$(function () {
    $('.view-toggle-btn').on('click', function () {
        setExperimentsView($(this).data('view'));
    });

    var savedMode = localStorage.getItem('experimentsViewMode');
    if (experimentsViews.includes(savedMode)) {
        setExperimentsView(savedMode);
    }

    initExperimentsKanbanDragAndDrop();
    initAddExperimentModal();
    initEditExperimentModal();
    initDeleteExperimentModal();
});
