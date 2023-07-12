using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Data;
using UnityEditor;

public class ScenesManager : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField] private SceneAsset menuScene;
	[SerializeField] private SceneAsset astarScene;
	[SerializeField] private SceneAsset geneticScene;
	[SerializeField] private SceneAsset boidsScene;
	[SerializeField] private SceneAsset dijkstraScene;

	private void OnValidate()
	{
		if (menuScene != null) startSceneName = menuScene.name;
		if (astarScene != null) astarSceneName = astarScene.name;
		if(geneticScene != null) geneticSceneName = geneticScene.name;
		if(boidsScene != null) boidsSceneName = boidsScene.name;
		if(dijkstraScene != null) dijkstraSceneName = dijkstraScene.name;

	}
#endif

	[SerializeField] private string startSceneName;
	[SerializeField] private string astarSceneName;
	[SerializeField] private string geneticSceneName;
	[SerializeField] private string boidsSceneName;
	[SerializeField] private string dijkstraSceneName;

	public void LoadStartScene()
	{
		SceneManager.LoadScene(startSceneName);
	}
	public void LoadAstarScene()
	{
		SceneManager.LoadScene(astarSceneName);
	}
	public void LoadGeneticScene()
	{
		SceneManager.LoadScene(geneticSceneName);
	}
	public void LoadBoidsScene()
	{
		SceneManager.LoadScene(boidsSceneName);
	}
	public void LoadDijktraScene()
	{
		SceneManager.LoadScene(dijkstraSceneName);
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Scene scene = SceneManager.GetActiveScene();

			if (scene.name == "Menu")
			{
				Debug.Log("Quitting");
				Application.Quit();
			}
			else
			{
				LoadStartScene();
			}
		}
	}

}
