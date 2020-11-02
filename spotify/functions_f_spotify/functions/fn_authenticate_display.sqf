params
[
        ["_display", displayNull, [displayNull]]
];

private _colorHighlight = (["GUI", "BCG_RGB"] call BIS_fnc_displayColorGet) call BIS_fnc_colorRGBtoHTML;
private _colorWarning = (["IGUI", "WARNING_RGB"] call BIS_fnc_displayColorGet) call BIS_fnc_colorRGBtoHTML;
private _separator = [" - ", ": "] select (language == "spanish");
private _bullet = "<t size='0.5' color='" + _colorHighlight + "'><img image='A3\Ui_f\data\IGUI\RscIngameUI\RscHint\indent_square.paa' /></t>";

private _control_group = _display displayCtrl 2300;
private _text_control = _control_group controlsGroupCtrl 1100;
private _message = [];

// Message start
_message pushBack ["<t align='center'><img image='\spotify\ui_f-spotify\data\spotify\logo_green_ca.paa' />"];
_message pushBack [""];
_message pushBack [""];
_message pushBack ["<t size='2'>Asaayu's Arma Spotify Player</t>"];
_message pushBack [""];
_message pushBack ["This is an example"];
// Message end


_text_control ctrlSetStructuredText parseText _message joinString "<br/>";
[_text_control] call BIS_fnc_ctrlFitToTextHeight;

private _ctrl_pos = ctrlPosition _control_group;
_ctrl_pos set [3, ctrlPosition _control_group#3];
_control_group ctrlSetPosition _ctrl_pos;
