if (!is3DEN && {!dialog}) then
{
	// Stop people from spamming the API when quickly opening and closing the display
	private _last_open = missionNamespace getVariable ["aasp_last_open", 0];
	if (_last_open <= diag_tickTime) then
	{
		createDialog "AASP_spotify";
		missionNamespace setVariable ["aasp_last_open", diag_tickTime + 2];
	}
	else
	{
		systemChat "Stop trying to open the AASP menu too fast!";
	};
};
