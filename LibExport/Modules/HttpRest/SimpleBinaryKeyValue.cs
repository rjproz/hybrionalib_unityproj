#if HYBRIONA_LIB_ENABLE_HTTP_CLIENT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

namespace Hybriona
{
	public class SimpleBinaryKeyValue 
	{
		public string headerMeta {get; private set;}
		private string _keyval_Seperater;
		private string _element_Seperater;


#region Setter
		public SimpleBinaryKeyValue()
		{
			_keyval_Seperater = RandomString();
			_element_Seperater = RandomString();
			headerMeta = meta +" "+_keyval_Seperater+" "+_element_Seperater;
		}

		private string meta = "application/hyb_sbkv";
		private Dictionary<string,byte[]> _data = new Dictionary<string,byte[]>();

		public void Add(string key,byte [] raw)
		{
			if(_data.ContainsKey(key) )
			{
				_data[key] =  raw ;
			}
			else
			{
				_data.Add( key, raw );
			}
		}

		public byte [] Encode()
		{
			
			List<byte> builder = new List<byte>();
			int counter = 0;
			foreach(KeyValuePair<string,byte[]> entry in _data)
			{
				builder.AddRange(String2ByteArray(entry.Key));
				builder.AddRange(String2ByteArray(_keyval_Seperater));

				builder.AddRange(entry.Value);

				if(counter < _data.Count - 1)
				{
					builder.AddRange(String2ByteArray(_element_Seperater));
				}

				counter++;

			}
			byte [] output = builder.ToArray();
			builder.Clear();
			System.GC.Collect();
			return output;
		}

#endregion

#region Getter

		public SimpleBinaryKeyValue(string _contentType,byte [] raw)
		{
			string [] headers = _contentType.Split(' ');
			_keyval_Seperater = headers[1];
			_element_Seperater = headers[2];

			string content = Encoding.ASCII.GetString(raw);

			int startIndex = 0;
			do
			{
				int keyValIndex = content.IndexOf(_keyval_Seperater,startIndex);
				byte [] subset = new byte[keyValIndex - startIndex];
				Array.Copy(raw,startIndex,subset,0,subset.Length);
				string key = Encoding.ASCII.GetString(subset);


				startIndex = keyValIndex +  _keyval_Seperater.Length;

				int elementIndex = content.IndexOf(_element_Seperater,startIndex);
				if(elementIndex == -1)
				{
					elementIndex = raw.Length - 1;
				}
				subset = new byte[elementIndex - startIndex];
				Array.Copy(raw,startIndex,subset,0,subset.Length);
			

				_data.Add(key,subset);
				startIndex =  elementIndex + _element_Seperater.Length;
			}
			while(startIndex < raw.Length);

			System.GC.Collect();
		}
		public byte [] GetEntry(string key)
		{
			return  _data[key];
		}
		public Dictionary<string,byte []> GetFiles()
		{
			return _data;
		}
#endregion


		private byte [] String2ByteArray(string str)
		{
			char [] charArray = str.ToCharArray();
			byte [] bytes = new byte[charArray.Length];

			for(int i=0;i<bytes.Length;i++)
			{
				bytes[i] = System.Convert.ToByte(charArray[i]);
			}
			return bytes;
		}

		private string RandomString(int length = 50)
		{
			StringBuilder builder = new StringBuilder();
			for(int i=0;i<length;i++)
			{
				int mode = UnityEngine.Random.Range(0,1000)%3;

				//int num = Random.Range(48,57+1);
				//int ABZ = Random.Range(65,90+1);
				//int abz = Random.Range(97,122+1);

				int val = 0;
				if(mode == 0)
				{
					val = UnityEngine.Random.Range(48,57+1);
				}
				else if(mode == 1)
				{
					val = UnityEngine.Random.Range(65,90+1);
				}
				else if(mode == 2)
				{
					val = UnityEngine.Random.Range(97,122+1);
				}


				builder.Append( (char)val);
			}
			return builder.ToString();

		}
	}
}
#endif