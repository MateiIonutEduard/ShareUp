var count = 0;
var buf = new Array();

function RemoveItem(id) {
    var i = id - 1;
    buf.splice(i, 1);
    $('#data').empty();
    count--;

    for (var j = 0; j < buf.length; j++) {
        var email = `<td>${buf[j]}</td>`
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
            buf.push(list[0]);
            
            $('#data').append(node);
            $('#to').val('');

            e.preventDefault();
            return false;
        }

        return true;
    });

    $(document).on('submit', '#sender', e => {
        e.preventDefault();
        var modal = $(".modal");
        modal.css("display", "block");

        $('#cancel').on('click', () => {
            modal.css("display", "none");
        });

        $('#close').on('click', () => {
            modal.css("display", "none");
        });

        var buffer = new FormData();
        buffer.append('to', buf[0]);

        for (var k = 1; k < buf.length; k++)
            buffer.append('to', buf[k]);

        buffer.append('files', $('#files').get(0).files[0]);
        var len = $('#files').get(0).files.length;

        for (var i = 1; i < len; i++)
            buffer.append('files', $('#files').get(0).files[i]);

        console.log(buffer);

        $.ajax({
            url: '/Index',
            type: 'post',
            success: () => {
                setTimeout(() => {
                    $('.progress-bar').width('0%');
                    $('.progress-bar').html('0%');
                    location.reload(true);
                }, 500);
            },
            xhr: function () {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener('progress', function (e) {
                    if (e.lengthComputable) {
                        var percent = parseInt((e.loaded / e.total) * 100);
                        $('.progress-bar').width(`${percent}%`);
                        $('.progress-bar').html(`${percent}%`);
                    }
                }, false);

                return xhr;
            },
            beforeSend: function () {
                $('.progress-bar').width("0%");
                $('.progress-bar').html("0%");
            },
            cache: false,
            data: buffer,
            processData: false,
            contentType: false,
            async: true
        });
    });

    $(dcoument).on('submit', '#signup', e => {
        var key = $('#password').val();
        var conf = $('confirm').val();

        if (key === conf && key) {
            var buffer = {
                'username': $('#username').val(),
                'password': $('#password').val(),
                'address': $('#address').val()
            };

            $.ajax({
                url: '/Account/?handler=Signup',
                type: 'post',
                data: buffer,
                success: () => {
                    setTimeout(() => {
                        location.href = '/Index';
                    }, 500);
                },
                cache: false,
                async: true
            });
        }
    });

    $(document).on('submit', '#upkey', e => {
        e.preventDefault();
        var key = $('#password').val();
        var conf = $('#confirm').val();

        if (key === conf && key) {
            $.ajax({
                url: '/Account/?handler=Change',
                type: 'post',
                data: {
                    'password': key
                },
                success: data => {
                    setTimeout(() => {
                        location.href = '/Index';
                    }, 500);
                },
                cache: false,
                async: true
            });
        }
    });
});