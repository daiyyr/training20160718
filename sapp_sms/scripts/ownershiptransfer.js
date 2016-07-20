
function ImageButtonSubmit_ClientClick() {
    var elements = $("#ComboBoxDebtor input");
    if (elements[0].value == "Null") {
        alert("Please select new proprietor.");
        return false;
    }

    if ($("#TextBoxStart").val() == "") {
        alert("Please select active date.");
        return false;
    }

    __doPostBack('__Page', 'ImageButtonSubmit');
    return false;
}

