class options_control_group: ctrlControlsGroupNoHScrollbars
{
	idc = 55000;
	show = 0;
	x = X(0.1) + W(0.8/8);
	y = Y(0.1) + HI(0.0125);
	w = W(0.8) - W(0.8/8);
	h = H(0.8) - H(0.8/10) - HI(0.0125);
	class controls
	{
		class options_background: ctrlStaticBackground
		{
			x = 0;
			y = 0;
			w = W(0.8) - W(0.8/8);
			h = H(0.8) - H(0.8/10) - HI(0.0125);
			colorBackground[] = {0.14,0.14,0.14,1};
		};
		class devices_title: ctrlStatic
		{
			text = "Settings";
			x = W(0.1);
			y = 0;
			w = W(0.45);
			h = H(0.05);
			sizeEx = H(0.05);
			color[] = {1,1,1,1};
			shadow = 0;
			font = "RobotoCondensedBold";
		};
		class github_button: ctrlActivePicture
		{
			text = "\spotify\ui_f_spotify\data\icons\github_ca.paa";
			x = W(0.55) - W(0.05);
			y = H(0.02);
			w = W(0.05);
			h = HI(0.0125);
			tooltip = "Click to open the mod's GitHub page in your browser.";
			onButtonClick = "'ArmaSpotifyController' callExtension 'github';";
		};
		class spacer_01: ctrlStatic
		{
			x = W(0.1);
			y = H(0.06);
			w = W(0.45);
			h = H(0.0025);
			colorBackground[] = {0.2,0.2,0.2,1};
		};
		class data_location_text: ctrlStatic
		{
			style = 1;
			text = "Cache location:";
			tooltip = "This is the location where images will be downloaded to.\nIn the future you may be able to change this file location.";
			x = W(0.1);
			y = LHX(1);
			w = W(0.2);
			h = LH;
			sizeEx = H(0.02);
			font = "RobotoCondensedBold";
		};
		class data_location_edit: ctrlEdit
		{
			style = 512;
			text = "";
			canModify = 0;
			x = W(0.1) + W(0.2);
			y = LHX(1);
			w = W(0.25);
			h = LH;
			sizeEx = H(0.02);
			onLoad = "(_this#0) ctrlSetText ('ArmaSpotifyController' callExtension 'data');";
		};
		class clear_cache: ctrlButton
		{
			text = "Clear Cache";
			tooltip = "Delete all files in the cache directory.";
			x = W(0.6/2) - W(0.08);
			y = LHX(3);
			w = W(0.075);
			h = LH;
			sizeEx = LH;
			font = "RobotoCondensedBold";
			onButtonClick = "'ArmaSpotifyController' callExtension 'clear_cache';";
		};

		class open_cache: clear_cache
		{
			text = "Open Cache";
			tooltip = "Open the cache directory in the file browser.";
			x = W(0.6/2) + W(0.005);
			onButtonClick = "'ArmaSpotifyController' callExtension 'open_cache';";
		};
		class spacer_02: spacer_01
		{
			y = LHX(5);
		};
		class log_location_text: data_location_text
		{
			text = "Log location:";
			tooltip = "This is the location where log files will be created.\nIn the future you may be able to change this file location.";
			y = LHX(6);
		};
		class log_location_edit: data_location_edit
		{
			y = LHX(6);
			onLoad = "(_this#0) ctrlSetText ('ArmaSpotifyController' callExtension 'log');";
		};
		class clear_logs: clear_cache
		{
			text = "Clear Logs";
			tooltip = "Delete all files in the log directory.\nDoes not delete the current log file.";
			y = LHX(8);
			onButtonClick = "'ArmaSpotifyController' callExtension 'clear_logs';";
		};
		class open_logs: clear_logs
		{
			text = "Open Logs";
			tooltip = "Open the log direcotry in the file browser.";
			x = W(0.6/2) + W(0.005);
			onButtonClick = "'ArmaSpotifyController' callExtension 'open_logs';";
		};
		class spacer_03: spacer_01
		{
			y = LHX(10);
		};
		class info_delay: log_location_text
		{
			text = "Sync Request Delay:";
			tooltip = "How often your game client sends a request to resync with your Spotify client.\nThis is also how often your client sends information to users who are listening along to your music.";
			y = LHX(11);
		};
		class info_delay_combo: ctrlCombo
		{
			x = W(0.1) + W(0.2);
			y = LHX(11);
			w = W(0.25);
			h = H(0.02);
			sizeEx = H(0.02);
			onLoad = "(_this#0) lbSetCurSel ((profilenamespace getVariable ['aasp_info_delay', 3])-1);";
			onLBSelChanged = "profilenamespace setVariable ['aasp_info_delay',(_this#0) lbValue (_this#1)]; saveProfileNamespace;";
			class items
			{
				class 1
				{
					text = "1s";
					value = 1;
				};
				class 2
				{
					text = "2s";
					value = 2;
				};
				class 3
				{
					text = "3s";
					value = 3;
				};
				class 4
				{
					text = "4s";
					value = 4;
				};
				class 5
				{
					text = "5s";
					value = 5;
				};
			};
		};
		class spacer_04: spacer_01
		{
			y = LHX(13);
		};
		class notification_location: data_location_text
		{
			text = "Notification Location:";
			tooltip = "The location where notifications will appear.";
			y = LHX(14);
		};
		class notification_location_edit: info_delay_combo
		{
			y = LHX(14);
			onLoad = "(_this#0) lbSetCurSel (profilenamespace getVariable ['aasp_notification_location', 0])";
			onLBSelChanged = "profilenamespace setVariable ['aasp_notification_location',(_this#0) lbValue (_this#1)]; saveProfileNamespace;";
			class items
			{
				class none
				{
					text = "None";
					value = 0;
				};
				class top_left
				{
					text = "Top Left";
					value = 1;
				};
				class top_right
				{
					text = "Top Right";
					value = 2;
				};
				class bottom_left
				{
					text = "Bottom Left";
					value = 3;
				};
				class bottom_right
				{
					text = "Bottom Right";
					value = 4;
				};
			};
		};
		class notification_announce: data_location_text
		{
			text = "Announce Song Changes:";
			tooltip = "When a new song starts playing display a notification using the notification location setting above. This will include when in a listening party.";
			y = LHX(16);
		};
		class notification_announce_edit: info_delay_combo
		{
			y = LHX(16);
			onLoad = "(_this#0) lbSetCurSel (profilenamespace getVariable ['aasp_notification_announce', 0])";
			onLBSelChanged = "profilenamespace setVariable ['aasp_notification_announce',(_this#0) lbValue (_this#1)]; saveProfileNamespace;";
			class items
			{
				class false
				{
					text = "Disabled";
					value = 0;
				};
				class true
				{
					text = "Enabled";
					value = 1;
				};
			};
		};
		class spacer_05: spacer_01
		{
			y = LHX(18);
		};
		class listen_along: data_location_text
		{
			text = "Listening Along:";
			tooltip = "When enabled, other users with this mod loaded will be able to select you to listen along to your music.\nPlaying, pausing, and skipping songs will be broadcasted to users listening along with you.\nLocal files, player stats (shuffle mode, repeat mode, volume, etc.), personal information and profile information is not broadcasted to users listening along.";
			y = LHX(19);
		};
		class listen_along_combo: info_delay_combo
		{
			y = LHX(19);
			onLoad = "(_this#0) lbSetCurSel (profilenamespace getVariable ['aasp_listen_along', 1])";
			onLBSelChanged = "profilenamespace setVariable ['aasp_listen_along',(_this#0) lbValue (_this#1)]; saveProfileNamespace;";
			class items
			{
				class false
				{
					text = "Disabled";
					value = 0;
				};
				class true
				{
					text = "Enabled";
					value = 1;
				};
			};
		};
		class spacer_06: spacer_01
		{
			y = LHX(21);
		};
		class legal_01: ctrlStatic
		{
			style = 2;
			text = "Asaayu's Arma Spotify Player (AASP) is not affiliated, created by, nor endorsed by Spotify, Spotify AB, and Spotify Technology S.A.";
			x = W(0.1);
			y = LHX(22);
			w = W(0.45);
			h = LH;
			sizeEx = LH;
			font = "RobotoCondensedBold";
			shadow = 0;
		};
		class legal_02: legal_01
		{
			text = "All Spotify trademarks, service marks, trade names, logos, domain names, and any other features of the Spotify brand";
			y = LHX(23);
		};
		class legal_03: legal_01
		{
			text = "(“Spotify Brand Features”) are the sole property of Spotify or its licensors.";
			y = LHX(24);
		};
		class legal_04: legal_01
		{
			text = "All cover art and song metadata is supplied and made avalaible by Spotify.";
			y = LHX(25);
		};
		class revoke_access: ctrlButton
		{
			text = "Revoke Spotify Access";
			tooltip = "Opens the webpage to remove this mod's access to your Spotify account.\nThis will revoke future token requests, meaning actions done under the current token will still work until the automated token refresh fails and the mod will require authorization again.";
			x = W(0.6/2) - W(0.105);
			y = LHX(27);
			w = W(0.1);
			h = LH;
			sizeEx = LH;
			font = "RobotoCondensedBold";
			onButtonClick = "'ArmaSpotifyController' callExtension 'revoke';";
		};
		class deauthorise_button: revoke_access
		{
			x = W(0.6/2) + W(0.005);
			y = LHX(27);
			w = W(0.1);
			text = "Revoke Authorization";
			tooltip = "Forces the mod to revoke the current users authorisation and deletes any token files saved.";
			onButtonClick = "'ArmaSpotifyController' callExtension 'deauthorise'; (ctrlParent (_this#0)) closeDisplay 0; [] spawn {createDialog 'AASP_spotify';}";
		};
		class legal_05: legal_01
		{
			text = "By using this mod you agree to the EULA and Privacy Policy, click the buttons below to view them in full.";
			y = LHX(29);
		};
		class eula_button: revoke_access
		{
			x = W(0.6/2) - W(0.08);
			y = LHX(30);
			w = W(0.075);
			text = "EULA";
			tooltip = "Opens the end-user license agreement.";
			onButtonClick = "'ArmaSpotifyController' callExtension 'eula';";
		};
		class privacy_button: revoke_access
		{
			x = W(0.6/2) + W(0.005);
			y = LHX(30);
			w = W(0.075);
			text = "Privacy Policy";
			tooltip = "Opens the privacy policy.";
			onButtonClick = "'ArmaSpotifyController' callExtension 'privacy';";
		};
	};
};
