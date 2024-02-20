using System.Runtime.InteropServices;

namespace _8_2._Class_Library
{
	public static class TaskbarProgress
	{
		public enum Flags
		{
			NoProgress = 0,
			Indeterminate = 1,
			Normal = 2,
			Error = 4,
			Paused = 8
		}

		private static readonly ITaskbarList3 taskbarList = (ITaskbarList3)new TaskbarList();

		static TaskbarProgress()
		{
			taskbarList.HrInit();
		}

		public static IntPtr MainWinHandle { get; set; }

		public static void SetProgressState(Flags tbpFlags)
		{
			taskbarList.SetProgressState(MainWinHandle, tbpFlags);
		}

		public static void SetProgressValue(ulong completed)
		{
			taskbarList.SetProgressValue(MainWinHandle, completed, 100);
		}

		[ComImport]
		[Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface ITaskbarList3
		{
			void HrInit();
			void AddTab(IntPtr hwnd);
			void DeleteTab(IntPtr hwnd);
			void ActivateTab(IntPtr hwnd);
			void SetActiveAlt(IntPtr hwnd);
			void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
			void SetProgressValue(IntPtr hwnd, ulong completed, ulong total);
			void SetProgressState(IntPtr hwnd, Flags tbpFlags);
		}

		[Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
		[ClassInterface(ClassInterfaceType.None)]
		[ComImport]
		private class TaskbarList
		{
		}
	}
}
