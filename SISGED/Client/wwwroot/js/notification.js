function getTimeFromNow(notificationDate) {

    var date = moment(notificationDate, 'DD/MM/YYYY');

    var timeFromNow = date.locale('es').fromNow();

    return timeFromNow;
}