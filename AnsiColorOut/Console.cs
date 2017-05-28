using System;
using System.Runtime.InteropServices;

namespace Bozho.PowerShell {
	/// <summary>
	/// A Console helper class.
	/// </summary>
	public static class Console {
		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

		[DllImport("kernel32.dll")]
		public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetStdHandle(int handle);

		public const int STD_INPUT_HANDLE = -10;
		public const int STD_OUTPUT_HANDLE = -11;
		public const int STD_ERROR_HANDLE = -12;

		[Flags]
		public enum ConsoleInputModes : uint {
			ENABLE_PROCESSED_INPUT = 0x0001,
			ENABLE_LINE_INPUT = 0x0002,
			ENABLE_ECHO_INPUT = 0x0004,
			ENABLE_WINDOW_INPUT = 0x0008,
			ENABLE_MOUSE_INPUT = 0x0010,
			ENABLE_INSERT_MODE = 0x0020,
			ENABLE_QUICK_EDIT_MODE = 0x0040,
			ENABLE_EXTENDED_FLAGS = 0x0080,
			ENABLE_AUTO_POSITION = 0x0100,
		}

		[Flags]
		public enum ConsoleOutputModes : uint {
			ENABLE_PROCESSED_OUTPUT = 0x0001,
			ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
			ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
			DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
			ENABLE_LVB_GRID_WORLDWIDE = 0x0010,
		}

		/// <summary>
		/// Cygwin utilities (at the time of this writing) seem to be stripping 
		/// ENABLE_VIRTUAL_TERMINAL_PROCESSING output handle flag, which disables ANSI 
		/// sequence processing in PowerShell.
		/// 
		/// The best workaround I came up with was this method that re-adds the flag.
		/// 
		/// You can execute in your custom prompt function, or add it to something like 
		/// a custom cls function.
		/// </summary>
		public static void EnableVirtualTerminalProcessing() {
			int mode;
			IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
			GetConsoleMode(handle, out mode);
			mode |= (int)ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
			SetConsoleMode(handle, mode);
		}
	}
}
