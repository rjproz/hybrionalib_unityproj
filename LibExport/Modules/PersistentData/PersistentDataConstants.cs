/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  BaseGameSave.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/19/2018 12:22:07

*************************************************************************/
#if HYBRIONA_LIB_ENABLE_PERSISTENT_DATA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class PersistentDataConstants
	{
		public const string saveFileFormat = "data_{0}.db";
		public static string saveFileDirectory;
	}


}
#endif