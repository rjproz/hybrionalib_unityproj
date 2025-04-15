/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ParticleSystemPoolObject.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  18-09-2023 23:19:27

*************************************************************************/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Hybriona
{
	public class ParticleSystemPoolObject : GOPoolObject 
	{
        [HideInInspector] public bool autoDestroy;

        [field: SerializeField]
        public new ParticleSystem particleSystem { get; private set; }

        [field: SerializeField]
        public ParticleSystemRenderer particleSystemRenderer { get; private set; }

        public override void Activate()
        {
            if (particleSystem)
            {
                autoDestroy = true;
                lifeIfAutoDestroy = particleSystem.main.duration + .5f;

                particleSystem.Clear(true);
            }
            base.Activate();

          
        }

        public void SetParticle(ParticleSystem particleSystem)
        {
            this.particleSystem = particleSystem;
        }
    }

#if UNITY_EDITOR


    [CustomEditor(typeof(ParticleSystemPoolObject))]
    public class ParticleSystemPoolObjectEditor : Editor
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
