function loadTreeChildren($children) {
    $children.html('<p class="text-muted small tree-loading mb-0">Loading…</p>');

    $.get($children.data('url'))
        .done(function (html) {
            $children.html(html);
            $children.data('loaded', true);
        })
        .fail(function () {
            $children.html('<p class="tree-load-error mb-0" role="button">Could not load. Click to retry.</p>');
        });
}

$(function () {
    $(document).on('click', '.tree-node-header', function (e) {
        if ($(e.target).closest('.tree-manage-link').length) {
            return;
        }

        var $header = $(this);
        var $node = $header.closest('.tree-node');
        var $children = $node.children('.tree-children');
        if ($children.length === 0) {
            return;
        }

        var isExpanding = $children.hasClass('d-none');

        if (!isExpanding) {
            $children.addClass('d-none');
            $header.removeClass('expanded').attr('aria-expanded', 'false');
            return;
        }

        $header.addClass('expanded').attr('aria-expanded', 'true');
        $children.removeClass('d-none');

        if (!$children.data('loaded')) {
            loadTreeChildren($children);
        }
    });

    $(document).on('click', '.tree-load-error', function () {
        loadTreeChildren($(this).closest('.tree-children'));
    });
});
