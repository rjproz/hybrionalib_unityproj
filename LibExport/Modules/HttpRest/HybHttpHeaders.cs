using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class HttpHeaders {

		private Dictionary<string,string> _headers = null;
		public Dictionary<string,string> headers()
		{
			return _headers;
		}
#region Setters
		public HttpHeaders()
		{
			_headers = new Dictionary<string, string>();
		}

		/// <summary>
		/// Set HTTP Content type
		/// </summary>
		/// <param name="type">Type.</param>
		public void ContentType(string type)
		{
			if(_headers.ContainsKey("Content-Type"))
			{
				_headers["Content-Type"] = type;
			}
			else
			{
				_headers.Add("Content-Type",type);
			}
		}
		/// <summary>
		/// Set Http Content type to Json
		/// </summary>
		public void ContentTypeJson()
		{
			ContentType("application/json");
		}

		public void Add(string key,string value)
		{
			if(_headers.ContainsKey(key))
			{
				_headers[key] = value;
			}
			else
			{
				_headers.Add(key,value);
			}
		}
        public bool Has(string key)
        {
            return _headers.ContainsKey(key);
        }
#endregion
    }
}
