using DiscordRPC;
using DiscordRPC.Logging;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts
{
    internal abstract class Discord
    {
        public const string Invitation = "https://discord.com/invite/SVcbaRc7gH";
        public const string FeedbackChannel = "https://discord.gg/X8bt6mkbu7";
        public static string Username = "";
        public static bool IsReady;
        public static DiscordRpcClient Client;

        private static readonly RichPresence Presence = new RichPresence
        {
            Details = $"{Resources.Discord_InTheMainWindow} 🏠",
            State = string.Format(Resources.Discord_Version_, Program.AppVersion),
            Assets = new Assets
            {
                LargeImageKey = "main",
                LargeImageText = Resources.Discord_Desc
            },
            Timestamps = Timestamps.Now,
            Buttons = new[]
            {
                new Button { Label = Resources.Discord_OfficialWebsite, Url = Program.AppWebsiteFull },
                new Button { Label = Resources.Discord_DiscordServer, Url = Invitation }
            }
        };

        public static void InitRpc()
        {
            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 0) return;

            Client = new DiscordRpcClient("1057407191704940575") { Logger = new ConsoleLogger { Level = LogLevel.Warning } };

            Client.OnReady += (sender, msg) =>
            {
                Username = msg.User.Username;
                Log.Output($"Discord RPC: Connected to Discord with user {Username}.");

                IsReady = true;
            };
            Client.OnPresenceUpdate += (sender, msg) => Log.Output("Discord RPC: Presence has been updated.");
            Client.OnClose += (sender, msg) =>
            {
                Log.Output("Discord RPC: Closed.");
                IsReady = false;
            };
            Client.OnUnsubscribe += (sender, msg) =>
            {
                Log.Output("Discord RPC: Unsubscribed.");
                IsReady = false;
            };
            Client.OnConnectionEstablished += (sender, msg) => Log.Output("Discord RPC: Connection successfully.");
            Client.OnError += (sender, msg) =>
            {
                Log.SaveError("Discord RPC: An error occurred during the transmission of a message.");
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
            Client.SetPresence(Presence);
        }

        public static void SetStatus(string status)
        {
            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 1 && IsReady)
            {
                Presence.Details = status;
                Client.SetPresence(Presence);
            }
            else
            {
                Log.Output($"Discord Rich Presence was not updated. Data: {data}; IsReady: {IsReady}");
            }
        }
    }
}
