function IsDecimal(value, colname) {
    var regex = /^(\+|-)?([0-9]*\.?[0-9]*)$/;
    if (regex.test(value))
        return true;
    else
        return false;
}
function DecimalNull(value, colname) {
    if (value == "") {
        return [true, ''];
    }
    else {
        if (IsDecimal(value, colname)) {
            return [true, ''];
        }
        else {
            return [false, colname + ' should be decimal'];
        }
    }
}
function DecimalNotNull(value, colname) {
    if (value == "") {
        return [false, colname + ' Required'];
    }
    if (IsDecimal(value, colname)) {
        return [true, ''];
    }
    else {
        return [false, colname + ' should be decimal'];
    }
}
function NotNull(value, colname) {
    if (value == "") {
        return [false, colname + ' Required'];
    }
    else {
        return [true, ''];
    }
}
function StrNotNull(value, colname) {
    if (value == "") {
        return [false, colname + ' Required'];
    }
    else if (value.indexOf("\"") != -1) {
        return [false, colname + ' No \" Allowed!'];
    }
    else {
        return [true, ''];
    }
}
function StrNoDQuote(value, colname) {
    if (value == "") {
        return [true, ''];
    }
    else if (value.indexOf("\"") != -1) {
        return [false, colname + ' No \" Allowed!'];
    }
    else {
        return [true, ''];
    }

}

function ReplaceDQuote(e) {
    e.value = e.value.replace("\"", "'");
}