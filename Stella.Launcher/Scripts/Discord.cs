using DiscordRPC;
using DiscordRPC.Logging;
using StellaModLauncher.Properties;
using Button = DiscordRPC.Button;

namespace StellaModLauncher.Scripts;

internal abstract class Discord
{
	public const string? Invitation = "https://discord.com/invite/SVcbaRc7gH";
	public const string? FeedbackChannel = "https://discord.gg/X8bt6mkbu7";
	public static string? Username = "";
	public static bool IsReady;
	public static DiscordRpcClient? Client;

	private static readonly RichPresence Presence = new()
	{
		Details = $"{Resources.Discord_InTheMainWindow} 🏠",
		State = string.Format(Resources.Discord_Version_, Program.AppFileVersion),
		Assets = new Assets
		{
			LargeImageKey = "main",
			LargeImageText = Resources.Discord_Desc
		},
		Timestamps = Timestamps.Now,
		Buttons =
		[
			new Button { Label = Resources.Discord_OfficialWebsite, Url = Program.AppWebsiteFull },
			new Button { Label = Resources.Discord_DiscordServer, Url = Invitation }
		]
	};

	public static void InitRpc()
	{
		int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
		if (data == 0) return;

		Client = new DiscordRpcClient("1057407191704940575") { Logger = new ConsoleLogger { Level = LogLevel.Warning } };

		Client.OnReady += (_, msg) =>
		{
			Username = msg.User.Username;
			Program.Logger.Info($"Discord RPC: Connected to Discord with user {Username}");

			IsReady = true;
		};
		Client.OnPresenceUpdate += (_, _) => Program.Logger.Info("Discord RPC: Presence has been updated");
		Client.OnClose += (_, _) =>
		{
			Program.Logger.Info("Discord RPC: Closed");
			IsReady = false;
		};
		Client.OnUnsubscribe += (_, _) =>
		{
			Program.Logger.Warn("Discord RPC: Unsubscribed");
			IsReady = false;
		};
		Client.OnConnectionEstablished += (_, _) => Program.Logger.Info("Discord RPC: Connection successfully");
		Client.OnError += (_, _) =>
		{
			Program.Logger.Error("Discord RPC: An error occurred during the transmission of a message");
			IsReady = false;
		};

		Client.Initialize();
		Client.SetPresence(Presence);
	}

	public static void Home()
	{
		int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
		if (data == 0) return;

		Presence.Details = $"{Resources.Discord_InTheMainWindow} 🐈";

		try
		{
			Client!.SetPresence(Presence);
		}
		catch (Exception ex)
		{
			Log.ThrowError(ex);
		}
	}

	public static void SetStatus(string status)
	{
		int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
		if (data == 1 && IsReady)
		{
			Presence.Details = status;
			try
			{
				Client!.SetPresence(Presence);
			}
			catch (Exception ex)
			{
				Log.ThrowError(ex);
			}
		}
		else
		{
			Program.Logger.Info($"Discord Rich Presence was not updated. Data: {data}; IsReady: {IsReady}");
		}
	}
}
