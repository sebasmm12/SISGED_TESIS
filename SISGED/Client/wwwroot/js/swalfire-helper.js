async function showLockSwalFire(swalFireInfo) {
    var isChanged = false;
    
    await Swal.fire({
        title: swalFireInfo.title,
        icon:  swalFireInfo.icon,
        html:  swalFireInfo.html,
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, ' + swalFireInfo.action + '!',
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.value) {
            isChanged = true;
        }
        else {
            isChanged = false;
        }
    });

    return isChanged;
}

function showSwalFire(swalFireInfo) {
    Swal.fire({
        title: swalFireInfo.title,
        icon: swalFireInfo.icon,
        html: swalFireInfo.html,
        showConfirmButton: false,
    });
}