using System;
using System.IO.IsolatedStorage;
using System.IO;

namespace Shane.Church.Utility
{
	public class Cleaner
	{
		public static void CleanTempFiles()
		{
			using (var store = IsolatedStorageFile.GetUserStoreForApplication())
			{
				try
				{
					foreach (string fileName in store.GetFileNames("/Contacts/*"))
					{
						store.DeleteFile(fileName);
					}
				}
				catch { }
				try
				{
					foreach (string fileName in store.GetFileNames("*.tmp"))
					{
						store.DeleteFile(fileName);
					}
				}
				catch { }
			}
		}
	}
}
