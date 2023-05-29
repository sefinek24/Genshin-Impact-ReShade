using DiscordRPC;
using DiscordRPC.Logging;

namespace StellaLauncher.Scripts
{
    internal abstract class Discord
    {
        public const string Invitation = "https://discord.com/invite/SVcbaRc7gH";
        public const string FeedbackChannel = "https://discord.gg/X8bt6mkbu7";
        public static string Username = "";
        public static bool isReady;
        public static DiscordRpcClient Client;

        public static readonly RichPresence Presence = new RichPresence
        {
            Details = "In the main window 🏠",
            State = $"Version: v{Program.AppVersion}",
            Assets = new Assets
            {
                LargeImageKey = "main",
                LargeImageText = "The best Genshin Impact mod pack with ReShade, custom graphics presets, FPS Unlocker, own launcher and more!"
            },
            Timestamps = Timestamps.Now,
            Buttons = new[]
            {
                new Button { Label = "Official website", Url = Program.AppWebsiteFull },
                new Button { Label = "Discord server", Url = Invitation }
            }
        };

        public static void InitRpc()
        {
            Client = new DiscordRpcClient("1057407191704940575") { Logger = new ConsoleLogger { Level = LogLevel.Warning } };
            Client.OnError += (sender, msg) => Log.Output("Discord RPC: An error occurred during the transmission of a message.");
            Client.OnReady += (sender, msg) =>
            {
                Username = msg.User.Username;
                Log.Output($"Discord RPC: Connected to Discord with user {Username}.");

                isReady = true;
            };
            Client.OnPresenceUpdate += (sender, msg) => Log.Output("Discord RPC: Presence has been updated.");
            Client.OnClose += (sender, msg) => Log.Output("Discord RPC: Closed.");
            Client.OnUnsubscribe += (sender, msg) => Log.Output("Discord RPC: Unsubscribed.");
            Client.OnConnectionEstablished += (sender, msg) => Log.Output("Discord RPC: Connection successfully.");

            Client.Initialize();
            Client.SetPresence(Presence);
        }

        public static void Home()
        {
            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 0) return;

            Presence.Details = "In the main window 🐈";
            Client.SetPresence(Presence);
        }

        public static void SetStatus(string status)
        {
            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 1 && isReady)
            {
                Presence.Details = status;
                Client.SetPresence(Presence);
            }
            else
            {
                Log.Output($"Discord Rich Presence was not updated. Data: {data}; isReady: {isReady}");
            }
        }
    }
}
