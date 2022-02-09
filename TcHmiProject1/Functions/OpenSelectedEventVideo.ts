module TcHmi {
	export module Functions {
		export module TcHmiProject1 {
            export async function OpenSelectedEventVideo(par1: number) {
                console.log("OpenSelectedVideoStarted");
                let EventGrid = TcHmi.Controls.get<TcHmi.Controls.Beckhoff.TcHmiEventGrid>("TcHmiEventGrid");
                if (EventGrid === undefined) {
                    console.log("EventGrid has not  been found");
                    return;
                }
                let SelectedEvent = <Server.Events.Event> EventGrid.getSelectedEvent();
                if (SelectedEvent === null) {
                    console.log("SelectedEvent has been found");
                    return;
                }
                
                //construct fileName using the timestamp from the event.
                //The timestamp uses the UTC timezone, so we need to make sure we use the same here,
                //the EventGrid displays the local time zone, so the hours will be different.
                let temp = '';
                let timeStamp = SelectedEvent.timeReceived;

                let tempYear    =   timeStamp.getUTCFullYear().toString();
                let tempMonth   =   number2String((timeStamp.getUTCMonth() + 1));
                let tempDate    =   number2String(timeStamp.getUTCDate());
                let tempHour    =   number2String(timeStamp.getUTCHours());
                let tempMinutes =   number2String(timeStamp.getUTCMinutes());
                let tempSeconds = number2String(timeStamp.getUTCSeconds());
                let tempSecondsRollOver = number2String(timeStamp.getUTCSeconds());
                //let tempMils = timeStamp.getUTCMilliseconds().toString();
                

                
                //example filename (YYYY-MM-DD-HH-mm-SS-ms): 2022-01-25-21-16-59.mp4           
                let FileName = temp.concat(tempYear, "-", tempMonth, "-", tempDate, "-", tempHour, "-", tempMinutes, "-", tempSeconds, ".mp4");
                //check if the file exists in the virtual directory
                console.log("FileName: ", FileName);
                let FileExists = false;
                await CheckforVideo(par1, FileName).then((value) => { FileExists = value; }, (error) => { console.log(error); });
                //if the first check doesn't succeed, make sure that it is not from the time between raising and receiving the event causing the seconds to roll over to the next.
                if (!FileExists) {  
                    let FileName = temp.concat(tempYear, "-", tempMonth, "-", tempDate, "-", tempHour, "-", tempMinutes, "-", tempSecondsRollOver, ".mp4");
                    await CheckforVideo(par1, FileName).then((value) => { FileExists = value; }, (error) => { console.log(error); });
                    if (!FileExists) {
                        window.alert("No accompanying video was found for that event!");
                        return;
                    }
                }
                //since the file exists, load the video player and set its source to our found video
                //need to change the region first, as the video player control has not been attached, yet.                       
                let region = TcHmi.Controls.get<TcHmi.Controls.System.TcHmiRegion>("TcHmiRegion");
                if (region === undefined) {
                    console.log("tcHmiRegion is not found");
                    return;
                }
                region.setTargetContent("VideoPlayback/VideoPlayer.content");
                await sleep(200);
                let VideoPlayer = TcHmi.Controls.get<TcHmi.Controls.Beckhoff.TcHmiVideo>("TcHmiVideo");
                if (VideoPlayer === undefined) {
                    console.log("Unable to find TcHmiVideo");
                    return;
                }


                temp = "/Videos/"
                FileName = temp.concat(FileName);
                console.log("virtual file path: ", FileName);
                
                VideoPlayer.setSrcList([{source:FileName,type:"video/mp4"}]);
               

            }//function
            function sleep(milliseconds: number) {
                return new Promise(resolve => setTimeout(resolve, milliseconds));
            }
            //timestamp format is (YYYY-MM-DD-HH-mm-SS-ms) so we need to make sure if any of them are single digits, that 2 digits are still added
            function number2String(num: number) {
                let str = '';
                if (num < 10) {
                    str = '0';
                }
                return str = str.concat(num.toString());

            }//number2String()
            
		}
		registerFunctionEx('OpenSelectedEventVideo', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.OpenSelectedEventVideo);
	}
}