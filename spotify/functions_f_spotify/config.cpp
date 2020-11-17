class CfgPatches
{
	class functions_f_spotify
	{
		author = "Asaayu";
		name = "functions_f_spotify";
		url = "https://www.arma3.com";
		units[] = {};
		weapons[] = {};
		requiredVersion = 0.1;
		requiredAddons[] = { "A3_Data_F"};
	};
};

class CfgFunctions
{
	class functions_f_spotify
	{
		tag = "spotify";
		class spotify_main_functions
		{
		        file = "\spotify\functions_f_spotify\functions";
			class preinit
			{
				preInit = 1;
			};
		};
		class spotify_api_functions
		{
		        file = "\spotify\functions_f_spotify\functions\spotify";
			class get_devices {};
			class play_button {};
			class skip_button {};
			class set_playback {};
			class update_display {};
			class volume {};
		};
		class spotify_gui_functions
		{
		        file = "\spotify\functions_f_spotify\functions\gui";
			class mouse_over {};
			class menu_onload {};
			class setup_onload {};
		};
	};
};
