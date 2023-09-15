/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  LogManager.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/18/2018 13:15:51

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace Hybriona
{
	public static class LogManager 
	{
		public enum LogType {INFORMATIONAL,WARNING,ERROR,SPECIAL}
		public enum LogLevel {ALWAYS = 0,COMMON=1,OFTEN=2,RARE = 3};
		public const string LogFormat = "[{0}] [{1}] : {2}\n--------------------------------------------------------------------\n";
		private static string WritePathTemplate;

		static LogManager()
		{
			WritePathTemplate = Application.persistentDataPath + "/Logs02142010/";
			if(Directory.Exists(WritePathTemplate))
			{
				Directory.Delete(WritePathTemplate,true);
			}
			Directory.CreateDirectory(WritePathTemplate);
			WritePathTemplate = WritePathTemplate + "{0}.log";
		}


		public static void Write(LogType type,string message,string fileTag = "Player")
		{
			File.AppendAllText(string.Format(WritePathTemplate,fileTag),string.Format(LogFormat,System.DateTime.Now.ToString(),type.ToString(),message));
		}

		public static string GetLogPath(string fileTag = "Player")
		{
			return string.Format(WritePathTemplate,fileTag);
		}

		public static string GetLog(string fileTag = "Player")
		{
			return File.ReadAllText(string.Format(WritePathTemplate,fileTag));
		}

	}
}
