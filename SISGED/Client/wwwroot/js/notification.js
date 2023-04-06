function getTimeFromNow(notificationDate) {

    var date = moment(notificationDate, 'DD/MM/YYYY h:mm:ss a');

    var timeFromNow = date.locale('es').fromNow();

    return timeFromNow;
}