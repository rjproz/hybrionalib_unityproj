using System;
using Assets.SimpleZip;
using UnityEngine;
using UnityEngine.UI;

namespace Hybriona
{
    public class Example : MonoBehaviour
    {
       
        public void Start()
        {
            try
            {
                var sample = "El perro de San Roque no tiene rabo porque Ramón Rodríguez se lo ha robado.";

                sample = sample + sample + sample + sample + sample;
                
                var compressed = Zip.CompressToString(sample);
                var decompressed = Zip.Decompress(compressed);

                
            }
            catch (Exception e)
            {
                
            }
        }
    }
}