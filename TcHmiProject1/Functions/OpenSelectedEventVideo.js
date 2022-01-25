var TcHmi;
(function (TcHmi) {
    let Functions;
    (function (Functions) {
        let TcHmiProject1;
        (function (TcHmiProject1) {
            function OpenSelectedEventVideo(par1) {
                let EventGrid = TcHmi.Controls.get("tchmiEventGrid");
                if (EventGrid != undefined) {
                    let SelectedEvent = EventGrid.getSelectedEvent();
                    if (SelectedEvent != null) {
                        //construct fileName using the timestamp from the event.
                        //The timestamp uses the UTC timezone, so we need to make sure we use the same here,
                        //the EventGrid displays the local time zone, so the hours will be different.
                        let temp = '';
                        let timeStamp = SelectedEvent.timeRaised;
                        let tempYear = timeStamp.getUTCFullYear().toString();
                        let tempMonth = number2String(timeStamp.getUTCMonth() + 1);
                        let tempDate = number2String(timeStamp.getUTCDate());
                        let tempHour = number2String(timeStamp.getUTCHours());
                        let tempMinutes = number2String(timeStamp.getUTCMinutes());
                        let tempSeconds = number2String(timeStamp.getUTCSeconds());
                        let tempMil = timeStamp.getUTCMilliseconds().toString();
                        //example filename (YYYY-MM-DD-HH-mm-SS-ms): 2022-01-25-21-16-59-506.mp4
                        let FileName = temp.concat(tempYear, '-', tempMonth, '-', tempDate, '-', tempHour, '-', tempMinutes, '-', tempSeconds, '-', tempMil, '.mp4');
                        //check if the file exists in the virtual directory
                        let FileExists = TcHmiProject1.CheckforVideo(FileName);
                        if (!FileExists) {
                            return;
                        }
                        console.log(FileName);
                    } //if event is not null
                } //ifEvent Grid is found
            } //function
            TcHmiProject1.OpenSelectedEventVideo = OpenSelectedEventVideo;
            //timestamp format is (YYYY-MM-DD-HH-mm-SS-ms) so we need to make sure if any of them are single digits, that 2 digits are still added
            function number2String(num) {
                let str = '';
                if (num < 10) {
                    str = '0';
                }
                return str = str.concat(str, num.toString());
            } //number2String()
        })(TcHmiProject1 = Functions.TcHmiProject1 || (Functions.TcHmiProject1 = {}));
        Functions.registerFunctionEx('OpenSelectedEventVideo', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.OpenSelectedEventVideo);
    })(Functions = TcHmi.Functions || (TcHmi.Functions = {}));
})(TcHmi || (TcHmi = {}));
//# sourceMappingURL=OpenSelectedEventVideo.js.map