using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HybSimpleExtensions  {

	public static string ToBase64(this byte [] bytes)
	{
		return System.Convert.ToBase64String(bytes);
	}
}
