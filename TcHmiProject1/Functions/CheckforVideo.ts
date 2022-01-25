module TcHmi {
    export module Functions {
        export module TcHmiProject1 {
            export function CheckforVideo(FileName:string): boolean {
                let files;
                let exists = false;
                //call the List Files function to list the files in the /Videos virtual directory
                TcHmi.Server.writeSymbol("ListFiles","/Videos",function(data) {
                    //If there is an error log it, and leave the function
                    if (data.error) {
                        console.log("error getting file list");
                        return;
                    }
                    //No error, make sure the results exist.
                    if (data.results != undefined) {
                        files = data.results[0];
                        if (files.value != undefined) {
                            if (files.value[FileName] != undefined) {
                                exists = true;
                            }
                        }
                    }
                    
                });
                return exists;
            }
            
        }
        registerFunctionEx('CheckforVideo','TcHmi.Functions.TcHmiProject1',TcHmiProject1.CheckforVideo);
    }
}