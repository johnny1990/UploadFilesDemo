function validateData() {
    if ($('[type="checkbox"]').is(':checked')) {
        Alert("Congratulations!!", "Your file is downloaded", "success");
        return true;
    }

    else {
        Alert("Selection Was Empty!!", "Please select at least one", "error");
        return false;
    }
}  