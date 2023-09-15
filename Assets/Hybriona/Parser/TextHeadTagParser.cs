using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hybriona
{
	public class TextHeadTagParser  {

		private static string END_TAG;
		public static void Prepare(string data,string endTag = "[END]")
		{
			END_TAG = endTag;
			m_Data = data;
		}

		private static string m_Data;
		public static string Find()
		{
			try
			{
				return m_Data.Substring(0, m_Data.IndexOf(END_TAG,0)).TrimEnd('\n').TrimStart('\n');
			}
			catch
			{
				return m_Data;
			}
		}

		public static string Find(string tag)
		{
			try
			{
				int indexStart = m_Data.IndexOf(tag);
				int indexEnd = m_Data.IndexOf(END_TAG,indexStart)-1;
				return m_Data.Substring(indexStart + tag.Length,indexEnd - indexStart - tag.Length).TrimEnd('\n').TrimStart('\n');
			}
			catch
			{
				return null;
			}
		}
	}
}