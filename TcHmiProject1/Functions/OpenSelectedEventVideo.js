var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var TcHmi;
(function (TcHmi) {
    let Functions;
    (function (Functions) {
        let TcHmiProject1;
        (function (TcHmiProject1) {
            function OpenSelectedEventVideo(par1) {
                return __awaiter(this, void 0, void 0, function* () {
                    console.log("OpenSelectedVideoStarted");
                    let EventGrid = TcHmi.Controls.get("TcHmiEventGrid");
                    if (EventGrid === undefined) {
                        console.log("EventGrid has not  been found");
                        return;
                    }
                    let SelectedEvent = EventGrid.getSelectedEvent();
                    if (SelectedEvent === null) {
                        console.log("SelectedEvent has been found");
                        return;
                    }
                    //construct fileName using the timestamp from the event.
                    //The timestamp uses the UTC timezone, so we need to make sure we use the same here,
                    //the EventGrid displays the local time zone, so the hours will be different.
                    let temp = '';
                    let timeStamp = SelectedEvent.timeRaised;
                    let tempYear = timeStamp.getUTCFullYear().toString();
                    let tempMonth = number2String((timeStamp.getUTCMonth() + 1));
                    let tempDate = number2String(timeStamp.getUTCDate());
                    let tempHour = number2String(timeStamp.getUTCHours());
                    let tempMinutes = number2String(timeStamp.getUTCMinutes());
                    let tempSeconds = number2String(timeStamp.getUTCSeconds());
                    let tempMil = timeStamp.getUTCMilliseconds().toString();
                    //example filename (YYYY-MM-DD-HH-mm-SS-ms): 2022-01-25-21-16-59-506.mp4
                    let FileName = temp.concat(tempYear, "-", tempMonth, "-", tempDate, "-", tempHour, "-", tempMinutes, "-", tempSeconds, "-", tempMil, ".mp4");
                    //check if the file exists in the virtual directory
                    console.log("FileName: ", FileName);
                    let FileExists = false;
                    yield TcHmiProject1.CheckforVideo(par1, FileName).then((value) => { FileExists = value; }, (error) => { console.log(error); });
                    if (!FileExists) {
                        return;
                    }
                    //since the file exists, load the video player and set its source to our found video
                    //need to change the region first, as the video player control has not been attached, yet.                       
                    let region = TcHmi.Controls.get("TcHmiRegion");
                    if (region === undefined) {
                        console.log("tcHmiRegion is not found");
                        return;
                    }
                    region.setTargetContent("VideoPlayback/VideoPlayer.content");
                    yield sleep(200);
                    let VideoPlayer = TcHmi.Controls.get("TcHmiVideo");
                    if (VideoPlayer === undefined) {
                        console.log("Unable to find TcHmiVideo");
                        return;
                    }
                    temp = "/Videos/";
                    FileName = temp.concat(FileName);
                    console.log("virtual file path: ", FileName);
                    VideoPlayer.setSrcList([{ source: FileName, type: "video/mp4" }]);
                });
            } //function
            TcHmiProject1.OpenSelectedEventVideo = OpenSelectedEventVideo;
            function sleep(milliseconds) {
                return new Promise(resolve => setTimeout(resolve, milliseconds));
            }
            //timestamp format is (YYYY-MM-DD-HH-mm-SS-ms) so we need to make sure if any of them are single digits, that 2 digits are still added
            function number2String(num) {
                let str = '';
                if (num < 10) {
                    str = '0';
                }
                return str = str.concat(num.toString());
            } //number2String()
        })(TcHmiProject1 = Functions.TcHmiProject1 || (Functions.TcHmiProject1 = {}));
        Functions.registerFunctionEx('OpenSelectedEventVideo', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.OpenSelectedEventVideo);
    })(Functions = TcHmi.Functions || (TcHmi.Functions = {}));
})(TcHmi || (TcHmi = {}));
//# sourceMappingURL=OpenSelectedEventVideo.js.map