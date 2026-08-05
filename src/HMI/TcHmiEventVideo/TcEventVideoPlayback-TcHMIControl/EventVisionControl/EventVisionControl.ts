/*
 * Generated 4/15/2024 12:59:44 PM
 * Copyright (C) 2024
 */

/* The bundled moment script is a plain resource without a type package. */
declare const moment: any;

/*
 * Since the 14.6 Controls package the event grid popups ship only as ES modules.
 * This project compiles as a classic script ("module": "none"), so the stock
 * DetailsPopup base class is loaded with a runtime dynamic import and the video
 * popup class is defined once that import resolves. The new Function wrapper
 * keeps the TypeScript compiler from rewriting import() for the "none" module
 * target. The class lands in a file scoped variable, not on window.
 *
 * Every failure of this mechanism (blocked eval, missing module, changed
 * export) is absorbed: the grid then falls back to the stock details popup
 * without the video button instead of losing the popup altogether.
 */
let eventVisionPopupClass: any;
let eventVisionPopupImportFailed = false;
let eventVisionVersionLogged = false;

try {
    new Function('path', 'return import(path)')('/Beckhoff.TwinCAT.HMI.Controls/dist/TcHmiEventGrid/DetailsPopup.js')
        .then((mod: any) => {
            eventVisionPopupClass = class DetailsVideoPopup extends mod.DetailsPopup {

                constructor(element: HTMLElement, control: any) {
                    super(element, control);

                    /*
                     * Description.json is the authority for these values; the
                     * setters below run before any poll starts. The literals
                     * only mirror the shipped defaults.
                     */
                    this.__virtualDirectory = '/EventVideos';
                    this.__videoCheckRetries = 60;
                    this.__videoCheckDelay = 100;

                    this.__videoCheckAttempts = 0;
                    this.__buttonState = 'waiting';

                    /*
                     * Poll bookkeeping. The token invalidates every scheduled
                     * or in flight check the moment a new event is shown, the
                     * popup hides, or the popup is destroyed, so a stale
                     * ListFiles result can never flip the button for the
                     * wrong event.
                     */
                    this.__pollToken = null;
                    this.__pollTimer = null;
                    this.__pollRequestId = null;

                    /*
                     * Button texts and the dialog title come from the language
                     * files under EventVisionControl/Lang. The English literals
                     * in __applyButtonState are the fallback if a key is
                     * missing.
                     */
                    this.__videoLocalization = new TcHmi.Locale.ControlLocalization('TcHmi.Controls.EventVision.EventVisionControl');

                    this.__createVideoDialog();
                    this.__createVideoButton();

                    /*
                     * Language files load asynchronously and the user can switch
                     * the locale at runtime, so re-apply the current button
                     * state text whenever the localization data changes. The
                     * base class calls every destroyer in __eventDestroyers on
                     * destroy.
                     */
                    this.__eventDestroyers.push(this.__videoLocalization.watch(() => this.__applyButtonState()));
                }

                update(event: any) {
                    super.update(event);
                    /*
                     * The grid calls update() directly when the selected row
                     * changes while the popup is open, so the button state
                     * machine must restart here, not only on popup open.
                     */
                    this.updateVideoButton(
                        this.__parentControl.getVirtualDrive(),
                        this.__parentControl.getVideoCheckRetries(),
                        this.__parentControl.getVideoCheckDelay());
                }

                hide() {
                    this.__cancelPoll();
                    super.hide();
                }

                destroy() {
                    this.__cancelPoll();
                    this.__videoButton?.destroy();
                    if (this.__dialog) {
                        this.__dialog.dialog('destroy');
                        this.__dialog.remove();
                    }
                    super.destroy();
                }

                __cancelPoll() {
                    this.__pollToken = null;
                    if (this.__pollTimer !== null) {
                        clearTimeout(this.__pollTimer);
                        this.__pollTimer = null;
                    }
                    if (this.__pollRequestId !== null) {
                        TcHmi.Server.releaseRequest(this.__pollRequestId);
                        this.__pollRequestId = null;
                    }
                }

                __createVideoDialog() {
                    /*
                     * The video element is created once and reused; only its
                     * source changes per open. The borrowed TcHmiVideo classes
                     * size it inside the dialog. No loop: an alarm clip plays
                     * once. Muted guarantees autoplay under Chromium policy;
                     * the service encodes no audio track anyway.
                     */
                    this.__videoElement = document.createElement('video');
                    this.__videoElement.className = 'TcHmi_Controls_Beckhoff_TcHmiVideo-template-content tchmi-video-template-content';
                    this.__videoElement.controls = true;
                    this.__videoElement.autoplay = true;
                    this.__videoElement.muted = true;
                    this.__videoElement.playsInline = true;

                    // jquery ui ships as a plain script resource without types, so the host stays untyped.
                    const dialogHost: any = $('<div>', {
                        'class': 'TcHmi_Controls_EventVision_EventVisionControl_video-dialog'
                    });
                    dialogHost.append(this.__videoElement);
                    this.__dialog = dialogHost.dialog({
                        autoOpen: false,
                        modal: true,
                        close: () => this.__stopVideo()
                    });
                }

                __stopVideo() {
                    /*
                     * jquery ui close() only hides the element; without this the
                     * video keeps decoding invisibly until the next open.
                     */
                    const video = this.__videoElement;
                    if (!video) {
                        return;
                    }
                    video.pause();
                    video.removeAttribute('src');
                    video.load();
                }

                __createVideoButton() {
                    /*
                     * The id is derived from the owning control like the stock popup
                     * buttons, so several EventVision instances on one page do not
                     * collide.
                     */
                    this.__videoButton = TcHmi.ControlFactory.createEx('TcHmi.Controls.Beckhoff.TcHmiButton',
                        `${this.__parentControl.getId()}.${this.__name}.videoButton`, {
                            'data-tchmi-width': 160,
                            'data-tchmi-height': 30
                        }, this.__parentControl);

                    this.__videoButton.getElement().addClass('TcHmi_Controls_EventVision_EventVisionControl_video-button');

                    /*
                     * First flex item, so the auto right margin in Style.css
                     * parks the button at the left edge while the stock confirm
                     * and close buttons keep their right alignment.
                     */
                    this.__elementFooter.insertBefore(this.__videoButton.getElement()[0], this.__elementFooter.firstChild);

                    this.__eventDestroyers.push(TcHmi.EventProvider.register(this.__videoButton.getId() + '.onPressed', () => {
                        this.__openVideoDialog();
                    }));

                    this.__applyButtonState();
                }

                __openVideoDialog() {
                    const src = this.__getVideoSource();
                    if (src === null) {
                        return;
                    }
                    this.__dialog.dialog('option', {
                        title: this.__text('Dialog_Title_VideoPlayback', 'Video Playback'),
                        width: this.__parentControl.getVideoWidth(),
                        height: this.__parentControl.getVideoHeight()
                    });
                    // Re-assert muted: a user unmute would otherwise block the next autoplay.
                    this.__videoElement.muted = true;
                    this.__videoElement.src = src;
                    this.__dialog.dialog('open');
                }

                updateVideoButton(virtualDirectory: string, videoCheckRetries: number, videoCheckDelay: number) {
                    // Whatever event was shown before, its poll is now moot.
                    this.__cancelPoll();

                    // Events raised without a CameraName attribute have no video. Hide the button.
                    if (this.__event?.params?.jsonAttribute?.includes('CameraName') !== true) {
                        this.__videoButton.getElement().addClass('TcHmi_Controls_EventVision_EventVisionControl_no-video');
                        return;
                    }

                    this.__virtualDirectory = virtualDirectory;
                    this.__videoCheckRetries = videoCheckRetries;
                    this.__videoCheckDelay = videoCheckDelay;

                    this.__videoButton.getElement().removeClass('TcHmi_Controls_EventVision_EventVisionControl_no-video');

                    this.__videoCheckAttempts = 0;
                    const token = {};
                    this.__pollToken = token;
                    this.__waitForVideo(token);
                }

                /*
                 * Button state machine. The service needs a few seconds after the
                 * event to assemble the video, so the button starts disabled and the
                 * popup polls the virtual directory until the file shows up or the
                 * retries run out.
                 */

                __waitForVideo(token: object) {
                    if (token !== this.__pollToken) {
                        return;
                    }

                    if (this.__videoCheckAttempts >= this.__videoCheckRetries) {
                        this.__setButtonState('unavailable');
                        return;
                    }

                    this.__setButtonState('waiting');

                    if (this.__videoCheckAttempts > 0) {
                        this.__pollTimer = setTimeout(() => {
                            this.__pollTimer = null;
                            this.__checkForVideo(token);
                        }, this.__videoCheckDelay);
                    } else {
                        this.__checkForVideo(token);
                    }
                }

                async __checkForVideo(token: object) {
                    if (token !== this.__pollToken) {
                        return;
                    }

                    try {
                        const cameraName = this.__getCameraName();
                        if (cameraName === null) {
                            this.__setButtonState('unavailable');
                            return;
                        }
                        const filePath = `${this.__virtualDirectory}/${cameraName}`;
                        const fileName = this.__getVideoFileName();

                        const found = await new Promise<boolean>(resolve => {
                            this.__pollRequestId = TcHmi.Server.writeSymbol('ListFiles', filePath, (data: any) => {
                                if (data.error || data.results === undefined || data.results[0]?.value === undefined) {
                                    /*
                                     * A server side error counts as a failed attempt, so a
                                     * missing virtual directory ends in VIDEO UNAVAILABLE
                                     * instead of polling forever.
                                     */
                                    resolve(false);
                                    return;
                                }
                                resolve(data.results[0].value[fileName] !== undefined);
                            });
                        });
                        this.__pollRequestId = null;

                        if (token !== this.__pollToken) {
                            return;
                        }

                        this.__videoCheckAttempts++;
                        if (found) {
                            this.__setButtonState('ready');
                        } else {
                            this.__waitForVideo(token);
                        }
                    } catch (error) {
                        /*
                         * Never leave the button stuck in WAITING FOR VIDEO: any
                         * unexpected throw ends the state machine visibly.
                         */
                        TcHmi.Log.error('EventVisionControl: video check failed: ' + error);
                        if (token === this.__pollToken) {
                            this.__setButtonState('unavailable');
                        }
                    }
                }

                __setButtonState(state: string) {
                    this.__buttonState = state;
                    this.__applyButtonState();
                }

                __applyButtonState() {
                    switch (this.__buttonState) {
                        case 'ready':
                            this.__videoButton.setText(this.__text('Button_Text_PlayVideo', 'PLAY VIDEO'));
                            this.__videoButton.setIsEnabled(true);
                            break;
                        case 'unavailable':
                            this.__videoButton.setText(this.__text('Button_Text_VideoUnavailable', 'VIDEO UNAVAILABLE'));
                            this.__videoButton.setIsEnabled(false);
                            break;
                        default:
                            this.__videoButton.setText(this.__text('Button_Text_WaitingForVideo', 'WAITING FOR VIDEO'));
                            this.__videoButton.setIsEnabled(false);
                            break;
                    }
                    this.__videoButton.setTooltip(this.__text('Button_Tooltip_PlayVideo', 'Play the video recorded for this event'));
                }

                __text(key: string, fallback: string) {
                    const text = this.__videoLocalization.getText(key);
                    return (typeof text === 'string' && text.length > 0 && text !== key) ? text : fallback;
                }

                /* Video location. */

                __getVideoSource(): string | null {
                    const cameraName = this.__getCameraName();
                    if (cameraName === null) {
                        return null;
                    }
                    // The cache buster keeps the browser from replaying a stale video for a reused file name.
                    return `${this.__virtualDirectory}/${cameraName}/${this.__getVideoFileName()}?_cb=${Date.now()}`;
                }

                __getCameraName(): string | null {
                    const raw = this.__event?.params?.jsonAttribute;
                    if (typeof raw !== 'string') {
                        return null;
                    }
                    /*
                     * Current PLC libraries write the attribute with unquoted
                     * keys, which JSON.parse rejects. Try strict JSON first so
                     * a fixed PLC needs no repair, then the key quoting regex.
                     * The regex corrupts values that contain colons, so a parse
                     * failure is logged instead of silently breaking the button.
                     */
                    try {
                        const attr = JSON.parse(raw);
                        if (typeof attr?.CameraName === 'string') {
                            return attr.CameraName;
                        }
                    } catch (error) { /* fall through to the repaired parse */ }
                    try {
                        const attr = JSON.parse(raw.replace(/(['"])?([a-z0-9A-Z_]+)(['"])?\s*:/g, '"$2": '));
                        if (typeof attr?.CameraName === 'string') {
                            return attr.CameraName;
                        }
                    } catch (error) { /* fall through to the error log */ }
                    TcHmi.Log.error('EventVisionControl: could not read CameraName from the event JSON attribute: ' + raw);
                    return null;
                }

                __getVideoFileName() {
                    const tz = this.__parentControl.getTimeZone();
                    if (!tz || typeof tz.bias !== 'number') {
                        /*
                         * No timezone info attribute is bound. The service names the
                         * file in server local time, so this fallback only lines up
                         * when browser and server share a timezone.
                         */
                        return moment(this.__event.timeRaised).format('YYYY-MM-DD-HH-mm-ss') + '.mp4';
                    }
                    return moment(this.__convertUTCToServerTime(this.__toUTC(this.__event.timeRaised), tz)).format('YYYY-MM-DD-HH-mm-ss') + '.mp4';
                }

                /*
                 * Timezone conversion. The PLC delivers a Windows TIME_ZONE_INFORMATION
                 * style structure (bias in minutes, DST transition rules), so the
                 * conversion follows the Windows rules instead of the browser locale.
                 *
                 * Known limits: the transition rules are evaluated against UTC
                 * instead of local time (up to one bias of error in a window
                 * around each transition) and southern hemisphere rule pairs
                 * (daylight start after standard start) are treated as never in
                 * DST. The planned contract change, where the PLC puts the exact
                 * file name into the event, retires this conversion entirely.
                 */

                __convertUTCToServerTime(utcDate: Date, tz: any) {
                    // Zones without DST publish empty transition rules (wMonth 0).
                    if (!tz.daylightDate?.wMonth || !tz.standardDate?.wMonth) {
                        return new Date(utcDate.getTime() - (tz.bias + tz.standardBias) * 60 * 1000);
                    }

                    // The year is needed to evaluate the DST transition rules.
                    const year = utcDate.getUTCFullYear();

                    const dstStart = this.__getTransitionDate(year, tz.daylightDate);
                    const stdStart = this.__getTransitionDate(year, tz.standardDate);

                    const isDST = utcDate >= dstStart && utcDate < stdStart;

                    /*
                     * Total offset in minutes is the base bias plus the daylight or
                     * standard bias. Example: 300 + (-60) = 240, which is UTC-4
                     * during DST. The bias is positive for zones west of UTC, so the
                     * offset is subtracted to get server local time.
                     */
                    const offsetMinutes = tz.bias + (isDST ? tz.daylightBias : tz.standardBias);

                    return new Date(utcDate.getTime() - offsetMinutes * 60 * 1000);
                }

                __getTransitionDate(year: number, rule: any) {
                    /*
                     * Windows transition rules encode:
                     * wMonth: month (1 = January), wDay: week of the month (1 = first,
                     * 5 = last), wDayOfWeek: 0 = Sunday, wHour/wMinute: time of day.
                     */
                    const month = rule.wMonth - 1; // Windows months are 1 based, JS months 0 based.
                    const hour = rule.wHour || 0;
                    const minute = rule.wMinute || 0;
                    const dayOfWeek = rule.wDayOfWeek;

                    // Day of week of the first of the target month.
                    const firstOfMonth = new Date(Date.UTC(year, month, 1));
                    const firstDay = firstOfMonth.getUTCDay();

                    // Days from the first of the month to the first matching weekday.
                    const dayOffset = (dayOfWeek - firstDay + 7) % 7;

                    let day;
                    if (rule.wDay < 5) {
                        // Weeks 1 to 4: jump to the matching weekday, then forward whole weeks.
                        day = 1 + dayOffset + 7 * (rule.wDay - 1);
                    } else {
                        // wDay 5 means the last occurrence: walk back from the end of the month.
                        const lastDayOfMonth = new Date(Date.UTC(year, month + 1, 0)).getUTCDate();
                        for (let d = lastDayOfMonth; d >= lastDayOfMonth - 6; d--) {
                            const temp = new Date(Date.UTC(year, month, d));
                            if (temp.getUTCDay() === dayOfWeek) {
                                day = d;
                                break;
                            }
                        }
                    }

                    return new Date(Date.UTC(year, month, day, hour, minute));
                }

                __toUTC(date: Date) {
                    // Converts a local date to a UTC equivalent Date object.
                    return new Date(date.getTime() + date.getTimezoneOffset() * 60000);
                }
            };
        })
        .catch((reason: any) => {
            eventVisionPopupImportFailed = true;
            TcHmi.Log.error('EventVisionControl: could not load the details popup base class from /Beckhoff.TwinCAT.HMI.Controls/dist/TcHmiEventGrid/DetailsPopup.js: ' + reason);
        });
} catch (error) {
    /*
     * A Content Security Policy without unsafe-eval blocks new Function. The
     * control still registers and uses the stock details popup.
     */
    eventVisionPopupImportFailed = true;
}

module TcHmi {
    export module Controls {
        export module EventVision {
            export class EventVisionControl extends TcHmi.Controls.Beckhoff.TcHmiEventGrid {

                /*
                Attribute philosophy
                --------------------
                - Local variables are not set while definition in class, so they have the value 'undefined'.
                - On compile the Framework sets the value from HTML or from theme (possibly 'null') via normal setters.
                - The "changed detection" in the setter will result in processing the value only once while compile.
                - Attention: If we have a Server Binding on an Attribute the setter will be called once with null to initialize and later with the correct value.
                */


                /* The literals mirror the Description.json defaults, which are authoritative. */
                protected __virtualDirectory = "/EventVideos";
                protected __videoCheckRetries = 60;
                protected __videoCheckDelay = 100;
                protected __TZInfo: object | undefined = undefined;

                protected __videoWidth = 1000;
                protected __videoHeight = 800;

                protected __popupClassRetries = 0;
                protected __popupFallbackLogged = false;
                protected __stockPopupFailLogged = false;

                /**
                 * Constructor of the control
                 * @param {JQuery} element Element from HTML (internal, do not use)
                 * @param {JQuery} pcElement precompiled Element (internal, do not use)
                 * @param {TcHmi.Controls.ControlAttributeList} attrs Attributes defined in HTML in a special format (internal, do not use)
                 * @returns {void}
                 */
                constructor(element: JQuery, pcElement: JQuery, attrs: TcHmi.Controls.ControlAttributeList) {
                    /** Call base class constructor */
                    super(element, pcElement, attrs);
                }

                /**
                 * Is called during control initialize phase after attribute setter have been called based on it's default or initial html dom values.
                 * @returns {void}
                 */
                public __init() {
                    super.__init();

                    /*
                     * One log line per page load with the loaded package
                     * versions, so a field report after a TwinCAT HMI update
                     * names the versions involved without any digging.
                     */
                    if (!eventVisionVersionLogged) {
                        eventVisionVersionLogged = true;
                        const ownVersion = TcHmi.Controls.getVersion('TcHmi.Controls.EventVision.EventVisionControl');
                        const gridVersion = TcHmi.Controls.getVersion('TcHmi.Controls.Beckhoff.TcHmiEventGrid');
                        TcHmi.Log.info('EventVisionControl ' + (ownVersion ? ownVersion.full : 'unknown')
                            + ' on Framework ' + TcHmi.version.full
                            + ', Controls package ' + (gridVersion ? gridVersion.full : 'unknown'));
                    }
                }

                /**
                * Is called by the system after the control instance gets part of the current DOM.
                * Is only allowed to be called from the framework itself!
                */
                public __attach() {
                    super.__attach();

                    /**
                     * Initialize everything which is only available while the control is part of the active dom.
                     */
                }



                /**
                * Is called by the system after the control instance is no longer part of the current DOM.
                * Is only allowed to be called from the framework itself!
                */
                public __detach() {
                    super.__detach();

                    /**
                     * Disable everything which is not needed while the control is not part of the active dom.
                     * No need to listen to events for example!
                     */
                }

                /**
                * Destroy the current control instance.
                * Will be called automatically if system destroys control!
                */
                public destroy() {
                    /**
                    * While keep alive is set the control must not be destroyed.
                    */
                    if (this.getKeepAlive()) {
                        return;
                    }

                    super.destroy();

                    /**
                    * Free resources like child controls etc.
                    */
                }


                public __showDetails() {
                    // Retry timers and late framework calls must not touch a destroyed control.
                    if (this.getIsDestroyed()) {
                        return;
                    }

                    if (!this.getAllowDetailsPopup())
                        return;

                    if (!this.__detailsPopup) {
                        if (!eventVisionPopupClass) {
                            if (!eventVisionPopupImportFailed && this.__popupClassRetries++ < 100) {
                                /*
                                 * The popup base class import can still be in flight on
                                 * the first open after a page load. Retry briefly
                                 * instead of losing the click.
                                 */
                                setTimeout(() => this.__showDetails(), 50);
                                return;
                            }
                            // Import blocked or never resolved: stock popup, no video button.
                            this.__logPopupFallback('the details popup base class from the Controls package did not load');
                            this.__showStockDetailsPopup();
                            return;
                        }
                        this.__popupClassRetries = 0;
                        try {
                            this.__detailsPopup = new eventVisionPopupClass(this.__elementDetailsPopup[0], this);
                        } catch (error) {
                            this.__logPopupFallback('the video popup could not be created, the grid internals may have changed: ' + error);
                            this.__discardHalfBuiltPopup();
                            this.__showStockDetailsPopup();
                            return;
                        }
                    }

                    /*
                     * The popup's update() override restarts the video button
                     * state machine; after a fallback it is a stock popup and
                     * update() is plain stock behavior.
                     */
                    const selectedEvent = this.getSelectedEvent();
                    if (selectedEvent) {
                        this.__detailsPopup?.update(selectedEvent);
                    }
                    this.__detailsPopup?.show();
                }

                /*
                 * Degraded path: the full stock details popup, only the video
                 * button is missing. Guarded so a failing fallback costs one
                 * click instead of an uncaught exception in the grid handler.
                 */
                protected __showStockDetailsPopup() {
                    try {
                        super.__showDetails();
                    } catch (error) {
                        if (!this.__stockPopupFailLogged) {
                            this.__stockPopupFailLogged = true;
                            TcHmi.Log.error('EventVisionControl: the stock details popup fallback also failed: ' + error);
                        }
                    }
                }

                /*
                 * A popup constructor throw after its super() call leaves the
                 * stock confirm and close buttons registered and parented in
                 * the popup footer. Destroy and remove them so the stock
                 * fallback can register its own under the same ids.
                 */
                protected __discardHalfBuiltPopup() {
                    try {
                        for (const name of ['confirmButton', 'closeButton', 'videoButton']) {
                            TcHmi.Controls.get(this.getId() + '.detailsPopup.' + name)?.destroy();
                        }
                        const popupElement = this.__elementDetailsPopup;
                        popupElement?.find('.TcHmi_Controls_Beckhoff_TcHmiEventGrid-template-popup-footer').empty();
                        popupElement?.find('.TcHmi_Controls_Beckhoff_TcHmiEventGrid-template-popup-content').empty();
                    } catch (error) {
                        // Cleanup is best effort; the fallback still runs.
                    }
                }

                protected __logPopupFallback(reason: string) {
                    if (this.__popupFallbackLogged) {
                        return;
                    }
                    this.__popupFallbackLogged = true;
                    TcHmi.Log.warn('EventVisionControl: falling back to the stock details popup without the video button because ' + reason);
                }


                public getVirtualDrive() { return this.__virtualDirectory; }
                public getTimeZone() { return this.__TZInfo; }
                public getVideoCheckRetries() { return this.__videoCheckRetries; }
                public getVideoCheckDelay() { return this.__videoCheckDelay; }
                public getVideoWidth() { return this.__videoWidth; }
                public getVideoHeight() { return this.__videoHeight; }

                public setVirtualDrive(value: string) { this.__virtualDirectory = value;}
                public setTimeZone(value: object | undefined) {this.__TZInfo = value;}
                public setVideoCheckRetries(value: number) { this.__videoCheckRetries = value;}
                public setVideoCheckDelay(value: number) { this.__videoCheckDelay = value;}
                public setVideoWidth(value: number) { this.__videoWidth = value;}
                public setVideoHeight(value: number) { this.__videoHeight = value;}
            }
        }
    }
}
/**
* Register Control
*/
TcHmi.Controls.registerEx('EventVisionControl', 'TcHmi.Controls.EventVision', TcHmi.Controls.EventVision.EventVisionControl);
