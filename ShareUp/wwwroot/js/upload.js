var count = 0;
var buf = new Array();

function RemoveItem(id) {
    var i = id - 1;
    buf.splice(i, 1);
    $('#data').empty();
    count--;

    for (var j = 0; j < buf.length; j++) {
        var email = buf[j];
        var close = `<td><button type="button" class="close" onclick='RemoveItem(${j + 1})' aria-label="Close"><span class='text-danger' aria-hidden="true">&times;</span></button></td>`;

        var index = `<th scope='row'>${j + 1}</th>`;
        var node = `<tr>${index}${email}${close}</tr>`;
        $('#data').append(node);
    }

}

$(document).ready(() => {
    $(document).on('keydown', '#to', e => {
        if (e.keyCode === 13) {
            var list = $('#to').val().split('\n');

            var email = `<td>${list[0]}</td>`;
            var close = `<td><button type="button" class="close" onclick='RemoveItem(${++count})' aria-label="Close"><span class='text-danger' aria-hidden="true">&times;</span></button></td>`;

            var index = `<th scope='row'>${count}</th>`;
            var node = `<tr>${index}${email}${close}</tr>`;
            buf.push(email);
            
            $('#data').append(node);
            $('#to').val('');

            e.preventDefault();
            return false;
        }

        return true;
    });
});