private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];

{
	private _ctrl = _display displayCtrl _x;
	_ctrl ctrlShow false;
} foreach [50000,69000,55000,75000,85000,86000];

// Delete any playlist control
ctrlDelete (_display displayCtrl 87000);

// Delete any album control
ctrlDelete (_display displayCtrl 88000);

// Delete any device control
ctrlDelete (_display displayCtrl 50000);
