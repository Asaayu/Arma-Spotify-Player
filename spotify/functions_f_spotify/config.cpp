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
		class spotify_setup_functions
		{
		        file = "\spotify\functions_f_spotify\functions";
			class preinit
			{
				preInit = 1;
			};
			class setup_onload {};
			class menu_onload {};
			class verify_extension {};
			class authenticate_display {};
			class authenticate_action {};

		};
	};
};
