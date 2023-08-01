using System;
using System.Reflection;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts
{
    internal static class Telemetry
    {
        private static readonly string Info = $"[{Resources.Telemetry_ThisFeatureDoesntDoAnything}]:";

        public static void Opened()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Output($"{Info} {string.Format(Resources.Telemetry_DeliveredTelemetryData, m?.ReflectedType?.Name, 1)}");
        }

        public static void SendLogFiles()
        {
            Console.ForegroundColor = ConsoleColor.Blue;

            try
            {
                // Send


                // Last logs
                Log.Output($"{Info} {Resources.Telemetry_LogFilesWasSentToDeveloper}");
                MethodBase m = MethodBase.GetCurrentMethod();
                Log.Output($"{Info} {string.Format(Resources.Telemetry_DeliveredTelemetryData, m?.ReflectedType?.Name, 2)}");
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex);
            }
        }

        public static void SupportMe_AnswYes()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Output($"{Info} {string.Format(Resources.Telemetry_DeliveredTelemetryData, m?.ReflectedType?.Name, 3)}");
        }

        public static void SupportMe_AnswNo()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Output($"{Info} {string.Format(Resources.Telemetry_DeliveredTelemetryData, m?.ReflectedType?.Name, 4)}");
        }

        public static void Error(Exception ex)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Output($"{Info} {string.Format(Resources.Telemetry_DeliveredTelemetryData, m?.ReflectedType?.Name, 1)}");
        }
    }
}
