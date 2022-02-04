module TcHmi {
    export module Functions {
        export module TcHmiProject1 {
            export async function CheckforVideo( FileName: any): Promise<boolean> {
                let exists = false;
                //declare promise to call the List Files function to list the files in the /Videos virtual directory
                //if thre is an error, return rejected. else return resolved.
                var check_Promise = new Promise<boolean>(function (resolve, reject) {
                    TcHmi.Server.writeSymbol("ListFiles", "/Videos", function (data) {
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

                        if (files.value[FileName] === undefined) {
                            console.log(FileName, " has not been found")
                            resolve(false);
                            return;
                        }
                        console.log(FileName, " has been found")
                        resolve(true);
                    });//writeSymbol
                });//check_Promise declaration

                let promise = await check_Promise.then(
                    (value) => { exists = value; },
                    (error) => { console.log(error); });
                //console.log("exists is being returned as:",exists);

                return exists;

            }
        }
        registerFunctionEx('CheckforVideo','TcHmi.Functions.TcHmiProject1',TcHmiProject1.CheckforVideo);
    }
}