using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class HybPoolDataManager : MonoBehaviour {

	// Use this for initialization
	public HybPoolDecentralized [] registerThem;
	void Start () {
	
		for(int i=0;i<registerThem.Length;i++)
		{
			HybPoolManager.Instance.RegisterPool(registerThem[i].name,registerThem[i].gameObject);
		}
	}
	

}
#if UNITY_EDITOR
[CustomEditor(typeof(HybPoolDataManager))]
public class HybPoolDataManagerEditor : Editor {
	
	
	public HybPoolDataManager script
	{
		get
		{
			return (HybPoolDataManager) target;
		}
	}
	
	
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		if(GUILayout.Button("Create Constants File"))
		{
			CreateConstantsFile();
		}
		
		
	}
	private void CreateConstantsFile()
	{

		string fileData = "// Automated!! Created at "+System.DateTime.Now.ToString();
		fileData += "\n\nusing UnityEngine;\nusing System.Collections;\npublic class HybPoolConstants\n{\n\n";

		for(int i=0;i<script.registerThem.Length;i++)
		{
			string poolID = script.registerThem[i].name;
			fileData += "\t"+"public const string "+poolID.Replace(" ","_")+" = "+"\""+poolID+"\";\n";
		}

		fileData += "\n\n}";
		if(!System.IO.Directory.Exists("Assets/GeneratedCode"))
		{
			System.IO.Directory.CreateDirectory("Assets/GeneratedCode");
		}
#if !UNITY_WEBPLAYER
		System.IO.File.WriteAllText("Assets/GeneratedCode/HybPoolConstants.cs",fileData);
#endif
		AssetDatabase.Refresh();

	}
}
#endif