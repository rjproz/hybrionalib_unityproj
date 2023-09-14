﻿using System;
using System.Text;


namespace Hybriona
{
    public static class Zip
    {
        /// <summary>
        /// Compress plain text to byte array
        /// </summary>
        public static byte[] Compress(string text)
        {
           
            return Ionic.Zlib.ZlibStream.CompressString(text);
        }

        /// <summary>
        /// Compress plain text to compressed string
        /// </summary>
        public static string CompressToString(string text)
        {
            return Convert.ToBase64String(Compress(text));
        }

        /// <summary>
        /// Decompress byte array to plain text
        /// </summary>
        public static string Decompress(byte[] bytes)
        {
            return Encoding.UTF8.GetString(Ionic.Zlib.ZlibStream.UncompressBuffer(bytes));
        }

        /// <summary>
        /// Decompress compressed string to plain text
        /// </summary>
        public static string Decompress(string data)
        {
            return Decompress(Convert.FromBase64String(data));
        }
    }
}