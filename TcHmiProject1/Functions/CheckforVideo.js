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
            function CheckforVideo(ctx, FileName) {
                return __awaiter(this, void 0, void 0, function* () {
                    let exists = false;
                    //declare promise to call the List Files function to list the files in the /Videos virtual directory
                    //if thre is an error, return rejected. else return resolved.
                    var check_Promise = new Promise(function (resolve, reject) {
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
                                console.log(FileName, " has been found");
                                resolve(false);
                                return;
                            }
                            console.log(FileName, " has been found");
                            resolve(true);
                        }); //writeSymbol
                    }); //check_Promise declaration
                    let promise = yield check_Promise.then((value) => { exists = value; }, (error) => { console.log(error); });
                    console.log("exists is being returned as:", exists);
                    return exists;
                });
            }
            TcHmiProject1.CheckforVideo = CheckforVideo;
        })(TcHmiProject1 = Functions.TcHmiProject1 || (Functions.TcHmiProject1 = {}));
        Functions.registerFunctionEx('CheckforVideo', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.CheckforVideo);
    })(Functions = TcHmi.Functions || (TcHmi.Functions = {}));
})(TcHmi || (TcHmi = {}));
//# sourceMappingURL=CheckforVideo.js.map