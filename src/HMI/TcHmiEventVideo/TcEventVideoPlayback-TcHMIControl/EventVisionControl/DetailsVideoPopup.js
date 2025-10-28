// Keep these lines for a best effort IntelliSense of Visual Studio 2017 and higher.
/// <reference path="./../../TcHmiProject1/Packages/Beckhoff.TwinCAT.HMI.Framework.12.760.59/runtimes/native1.12-tchmi/TcHmi.d.ts" />


class DetailsVideoPopup extends TcHmi.Controls.Beckhoff.TcHmiEventGridPopups.DetailsPopup {
    constructor(element, control) {
        super(element, control);

        this.__virtualDirectory = "/Videos";
        this.__videoCheckRetries = 10;
        this.__videoCheckDelay = 500;

        this.__videoVideoCheckAttempts = 0;


        this.__videoButton;


        this.dialog;
        this.createVideoPopup();
        this.createVideoButton();
    }

    createVideoPopup() {
        this.dialog = $("<div>", {
            id: "dialog-form",
            title: "Video Playback"
        }).dialog({
            autoOpen: false,
            height: this.__parentControl.__videoHeight,
            width: this.__parentControl.__videoWidth,
            modal: true,

            close: function () {
            }
        });


    }

    createVideoButton() {
        this.__videoButton = TcHmi.ControlFactory.createEx("TcHmi.Controls.Beckhoff.TcHmiButton", "newButtonId", {
            "data-tchmi-width": 160,
            "data-tchmi-height": 30,
            "data-tchmi-text": "PLAY VIDEO",
            "data-tchmi-tooltip": "Show Me The Video"
        }, this.__parentControl);

        this.__videoButton.getElement().addClass("TcHmi_Controls_EventVision_EventVisionControl_video-button");

        this.__elementFooter.appendChild(this.__videoButton.getElement()[0]);

        this.__eventDestroyers.push(TcHmi.EventProvider.register(this.__videoButton.getId() + ".onPressed", () => {            
            this.setVideoPopupHTML(this.getVideoSource());
            this.dialog.dialog("open");
        }));
    }

    updateVideoButton(virtualDirectory, videoCheckRetries, videoCheckDelay) {
        if (this.__event?.params?.jsonAttribute?.includes("CameraName") != true) {
            this.__videoButton.getElement().addClass("TcHmi_Controls_EventVision_EventVisionControl_no-video");
            return;
        }

        this.__virtualDirectory = virtualDirectory;
        this.__videoCheckRetries = videoCheckRetries;
        this.__videoCheckDelay = videoCheckDelay;


        this.__videoButton.getElement().removeClass("TcHmi_Controls_EventVision_EventVisionControl_no-video");

        this.__videoVideoCheckAttempts = 0;

        this.WaitingForVideo();
    }



    async CheckForVideo() {
        let exists = false;
        let filePath = this.getVideoPath();
        let fileName = this.getVideoFileName();
        //declare promise to call the List Files function to list the files in the /Videos virtual directory
        //if thre is an error, return rejected. else return resolved.

        var check_Promise = new Promise (async function (resolve, reject) {

            TcHmi.Server.writeSymbol("ListFiles", filePath, function (data) {
                //If there is an error log it, and leave the function
                if (data.error) {
                    reject("error getting file list");
                    return;
                }
                //No error, make sure the results exist.
                if (data.results === undefined) {
                    reject("error getting results from File List");
                    return;
                }
                let files = data.results[0];
                if (files.value === undefined) {
                    reject("What we got back from the ListFiles function isn't what was expected.");
                    return;
                }

                if (files.value[fileName] === undefined) {
                    console.log(fileName, " has not been found")
                    resolve(false);
                    return;
                }
                console.log(fileName, " has been found")
                resolve(true);
            });//writeSymbol
        });//check_Promise declaration

        await check_Promise.then(
            (value) => {
                this.__videoVideoCheckAttempts++;
                if (value)
                    this.VideoReady();
                else
                    this.WaitingForVideo();
            },
            (error) => { console.log(error); });
        //console.log("exists is being returned as:",exists);

        return exists;
    }


    //Update button to match video status//

    WaitingForVideo() {
        if (this.__videoVideoCheckAttempts >= this.__videoCheckRetries) {
            this.VideoUnavailable();
            return;
        }

        this.__videoButton.setText("WAITING FOR VIDEO");
        this.__videoButton.setIsEnabled(false);


        let _this = this;
        if (this.__videoVideoCheckAttempts > 0)
            setTimeout(function () { _this.CheckForVideo(); }, this.__videoCheckDelay);
        else
            this.CheckForVideo();        
    }

    VideoReady() {        
        this.__videoButton.setText("PLAY VIDEO");
        this.__videoButton.setIsEnabled(true);
    }
    VideoUnavailable() {
        this.__videoButton.setText("VIDEO UNAVAILABLE");
        this.__videoButton.setIsEnabled(false);
    }


    //Getting Video Path
    getVideoPath() {
        return `${this.__virtualDirectory}/${this.getCameraName()}`;
    }
    getVideoSource() {
        return `${this.getVideoPath()}/${this.getVideoFileName()}?_cb=${Date.now()}`;

        //return "Videos/Camera1/2024-04-30-13-51-14.mp4";
    }
    getCameraName() {
        return JSON.parse(this.__event.params.jsonAttribute.replace(/(['"])?([a-z0-9A-Z_]+)(['"])?\s*:/g, '"$2": ')).CameraName;
    }

    getVideoFileName() {
         if (!this.__parentControl.__TZInfo || typeof this.__parentControl.__TZInfo.bias !== "number") {
             console.log("No timezone information was collected from %s%ADS.PLC1.MAIN.Playback.TimeZoneInfo%/s% using the none TZ converted method")
             return moment(this.__event.timeRaised).format("YYYY-MM-DD-HH-mm-ss").concat(".mp4");
         }
        return moment(this.convertUTCToServerTime(this.toUTC(this.__event.timeRaised), this.__parentControl.__TZInfo)).format("YYYY-MM-DD-HH-mm-ss").concat(".mp4");
    }


    
    setVideoPopupHTML(src) {
        console.log("Setting Source");

        this.dialog.html('');
        //TcHmi.Controls.get("EventVisionControl_1").__detailsPopup.dialog.html('');
        this.dialog.html(`<video class="TcHmi_Controls_Beckhoff_TcHmiVideo-template-content tchmi-video-template-content" controls="" playsinline="" poster="" autoplay="" loop=""><source src=${src} type="video/mp4">HTML5 Video support is missing...</video>`);
    }

    //Time zone functions//

    convertUTCToServerTime(utcDate, tz) {
        // Extract the year from the UTC date (needed to evaluate DST transition rules)
        const year = utcDate.getUTCFullYear();

        // Calculate the actual DST start and end dates based on server's timezone rules
        const dstStart = this.getTransitionDate(year, tz.daylightDate);
        const stdStart = this.getTransitionDate(year, tz.standardDate);

        // Determine whether the current UTC date falls in the daylight saving time range
        const isDST = utcDate >= dstStart && utcDate < stdStart;

        // Compute the total offset in minutes (base bias + daylight or standard bias)
        // Example: 300 + (-60) = 240 → UTC-4 during DST
        const offsetMinutes = tz.bias + (isDST ? tz.daylightBias : tz.standardBias);

        // Convert UTC time to server-local time by subtracting the offset (bias is POSITIVE for UTC-5)
        const serverTime = new Date(utcDate.getTime() - offsetMinutes * 60 * 1000);

        return serverTime;
    }
    getTransitionDate(year, rule) {
        // Windows rules use:
        // - wMonth: the month (1 = Jan, 11 = Nov)
        // - wDay: week of the month (1 = first, 2 = second, ..., 5 = last)
        // - wDayOfWeek: 0 = Sunday, 1 = Monday, ...
        // - wHour/wMinute: time of day for the transition

        const month = rule.wMonth - 1; // Convert from 1-based (Windows) to 0-based (JS)
        const hour = rule.wHour || 0;
        const minute = rule.wMinute || 0;
        const dayOfWeek = rule.wDayOfWeek;

        // Step 1: Start from the first of the target month (e.g., March 1st)
        const firstOfMonth = new Date(Date.UTC(year, month, 1));
        const firstDay = firstOfMonth.getUTCDay(); // Day of week of the 1st

        // Step 2: Calculate how many days from the 1st to reach the desired weekday
        let dayOffset = (dayOfWeek - firstDay + 7) % 7;

        let day;
        if (rule.wDay < 5) {
            // For week 1–4 (first, second, third, fourth):
            // Add 0–6 days to get to desired weekday, then jump forward N weeks
            day = 1 + dayOffset + 7 * (rule.wDay - 1);
        } else {
            // For wDay == 5 → last occurrence of that weekday in the month
            // Count backward from the end of the month to find the last correct weekday
            const lastDayOfMonth = new Date(Date.UTC(year, month + 1, 0)).getUTCDate(); // Day number (28–31)
            for (let d = lastDayOfMonth; d >= lastDayOfMonth - 6; d--) {
                const temp = new Date(Date.UTC(year, month, d));
                if (temp.getUTCDay() === dayOfWeek) {
                    day = d;
                    break;
                }
            }
        }

        // Step 3: Build final Date using calculated day and provided hour/minute
        return new Date(Date.UTC(year, month, day, hour, minute));
    }

    toUTC(date) {
        // Converts a local date to a UTC-equivalent Date object
        return new Date(date.getTime() + date.getTimezoneOffset() * 60000);
    }
}