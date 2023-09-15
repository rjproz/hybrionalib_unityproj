using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hybriona
{
	public static class TimeExtension 
	{
		public static string ToHumanSpan(this TimeSpan span)
		{
			if(span.TotalSeconds < 60)
			{
				return "just now";
			}
			else if(span.TotalMinutes < 30)
			{
				return "few minutes ago";
			}
			else if(span.TotalHours < 24)
			{
				return "few hours ago";
			}
			else if(span.TotalDays < 2)
			{
				return "yesterday";
			}
			else if(span.TotalDays < 7)
			{
				return "few days ago";
			}
			else if(span.TotalDays < 8)
			{
				return "last week";
			}
			else if(span.TotalDays < 18)
			{
				return "two weeks ago";
			}
			else if(span.TotalDays < 30)
			{
				return "few weeks ago";
			}
			else if(span.TotalDays < 360)
			{
				return "few months ago";
			}
			else if(span.TotalDays < 730)
			{
				return "last year";
			}
			else
			{
				return (span.TotalDays%365)+" years ago";
			}
		}

	}
}
