using System;
using Assets.SimpleZip;
using UnityEngine;
using UnityEngine.UI;

namespace Hybriona
{
    public class Example : MonoBehaviour
    {
        public Text Text;

        /// <summary>
        /// Usage example
        /// </summary>
        public void Start()
        {
            try
            {
                var sample = "El perro de San Roque no tiene rabo porque Ramón Rodríguez se lo ha robado.";

                sample = sample + sample + sample + sample + sample;
                
                var compressed = Zip.CompressToString(sample);
                var decompressed = Zip.Decompress(compressed);

                Text.text = string.Format("Plain text: {0}\n\nCompressed: {1}\n\nDecompressed: {2}",
                    sample,
                    compressed,
                    decompressed);
            }
            catch (Exception e)
            {
                Text.text = e.ToString();
            }
        }
    }
}