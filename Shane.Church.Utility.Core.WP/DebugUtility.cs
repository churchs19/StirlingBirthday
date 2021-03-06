﻿using Microsoft.Phone.Info;
using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;

namespace Shane.Church.StirlingBirthday.Core.WP
{
	public static class DebugUtility
	{
		private const long MaximumMilliseconds = 25000L;
		private static readonly Stopwatch _stopwatch = new Stopwatch();
		private static object lockObj = new object();

		public static void DebugOutputMemoryUsage(string label = null)
		{
#if DEBUG
			var limit = DeviceStatus.ApplicationMemoryUsageLimit;
			var current = DeviceStatus.ApplicationCurrentMemoryUsage;
			var remaining = limit - current;
			var peak = DeviceStatus.ApplicationPeakMemoryUsage;
			var safetyMargin = limit - peak;

			var message = string.Format("{0}\r\nMemory limit (bytes): {1}\r\nCurrent memory usage: {2} bytes ({3} bytes remaining)\r\nPeak memory usage: {4} bytes ({5} bytes safety margin)", label, limit, current, remaining, peak, safetyMargin);
			Debug.WriteLine(message);
			SaveDiagnosticMessage(message);
#endif
		}

		public static void DebugStartStopwatch()
		{
#if DEBUG
			_stopwatch.Start();
#endif
		}
		public static void DebugOutputElapsedTime(string label = null)
		{
#if DEBUG
			var milliSeconds = _stopwatch.ElapsedMilliseconds;
			var remaining = MaximumMilliseconds - milliSeconds;

			var message = string.Format("{0}\r\nRunning time: {1} milliseconds\r\nRemaining time (max): {2} milliseconds", label, milliSeconds, remaining);
			Debug.WriteLine(message);
			SaveDiagnosticMessage(message);
#endif
		}

		//Note: to get a result requires ID_CAP_IDENTITY_DEVICE  
		// to be added to the capabilities of the WMAppManifest  
		// this will then warn users in marketplace  
		public static string GetDeviceUniqueID()
		{
			byte[] result = null;
			object uniqueId;
			if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId))
				result = (byte[])uniqueId;

			return Convert.ToBase64String(result);
		}

		public static void SaveDiagnostics(Diagnostics diagInfo)
		{
			lock (lockObj)
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var fileStream = store.OpenFile("Diagnostics.txt", System.IO.FileMode.Append))
					{
						using (var writer = new System.IO.StreamWriter(fileStream))
						{
							writer.WriteLine(DateTime.Now.ToString("O"));
							writer.WriteLine("-------------------------------------------------");
							writer.WriteLine(String.Format("Device ID: {0}", diagInfo.DeviceUniqueId));
							writer.WriteLine(String.Format("Agent Exit Reason: {0}", diagInfo.AgentExitReason));
							writer.WriteLine("-------------------------------------------------");
						}
					}
				}
			}
		}

		public static void SaveDiagnosticMessage(string message)
		{
			lock (lockObj)
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var fileStream = store.OpenFile("Diagnostics.txt", System.IO.FileMode.Append))
					{
						using (var writer = new System.IO.StreamWriter(fileStream))
						{
							writer.WriteLine(DateTime.Now.ToString("O"));
							writer.WriteLine("-------------------------------------------------");
							writer.WriteLine(message);
							writer.WriteLine("-------------------------------------------------");
						}
					}
				}
			}
		}

		public static void SaveDiagnosticException(Exception ex)
		{
			lock (lockObj)
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var fileStream = store.OpenFile("Diagnostics.txt", System.IO.FileMode.Append))
					{
						using (var writer = new System.IO.StreamWriter(fileStream))
						{
							writer.WriteLine(DateTime.Now.ToString("O"));
							writer.WriteLine("-------------------------------------------------");
							writer.WriteLine("Exception");
							writer.WriteLine("-------------------------------------------------");
							writer.WriteLine(String.Format("Exception Type: {0}", ex.GetType().ToString()));
							writer.WriteLine(String.Format("Exception Message: {0}", ex.Message));
							writer.WriteLine("Stack Trace:");
							writer.WriteLine(ex.StackTrace);
							writer.WriteLine("-------------------------------------------------");
						}
					}
				}
			}
		}
	}

	public class Diagnostics
	{
		public long Id { get; set; }
		public string DeviceUniqueId { get; set; }
		public string AgentExitReason { get; set; }
	}
}
