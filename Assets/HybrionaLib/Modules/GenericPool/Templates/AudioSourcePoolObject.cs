/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  AudioSourcePoolObject.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-04-2025 10:10:50

*************************************************************************/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Hybriona
{
	public class AudioSourcePoolObject : GOPoolObject
	{
       

        [field:SerializeField]
        public AudioSource audioSource { get; private set; }
        

      
        public override void Activate()
        {
            if (audioSource && audioSource.clip)
            {
                autoDestroy = true;
                lifeIfAutoDestroy = audioSource.clip.length + .5f;
            }
            base.Activate();
           
        }

        public void SetClip(AudioClip clip)
        {
            if (audioSource)
            {
                audioSource.clip = clip;
            }
        }

        public void Play()
        {
            if (!audioSource.playOnAwake)
            {
                audioSource.Stop();
                audioSource.Play();
            }
        }

    }
    #if UNITY_EDITOR


    [CustomEditor(typeof(AudioSourcePoolObject))]
    public class AudioSourcePoolObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            bool expanded = true;
            while (property.NextVisible(expanded))
            {
                // Skip the script field and inherited 'hideThis' field
                if (property.name == "lifeIfAutoDestroy" || property.name == "autoDestroy")
                    continue;

                EditorGUILayout.PropertyField(property, true);
                expanded = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
