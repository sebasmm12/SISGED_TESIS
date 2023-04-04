function getTimeFromNow(notificationDate) {

    var date = moment(notificationDate, 'DD/MM/YYYY h:mm:ss');

    var timeFromNow = date.locale('es').fromNow();

    return timeFromNow;
}