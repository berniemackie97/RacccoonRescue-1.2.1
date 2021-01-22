using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Reflection;
using System.Net;
using UnityEditor;
using System;
using System.Text;

#if GAMESPARKS
using GameSparks.Core;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class GamesparksConfiguration : EditorWindow
{
	static string login = "";
	static string password = "";
	static string game_name = "";

	[MenuItem("GameSparks/Create game")]
	private static void CreateGameOption()
	{
		Init();
	}

	static void Init()
	{
		GamesparksConfiguration window = ScriptableObject.CreateInstance<GamesparksConfiguration>();
		window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 200);
		window.ShowPopup();
	}

	void OnGUI()
	{
		EditorGUILayout.LabelField("Creating new game in Gamesparks", EditorStyles.wordWrappedLabel);
		GUILayout.Space(30);
		game_name = EditorGUILayout.TextField("Game name", game_name);
		EditorGUILayout.LabelField("Authorizate to Gamesparks account", EditorStyles.wordWrappedLabel);
		login = EditorGUILayout.TextField("Login", login);
		password = EditorGUILayout.PasswordField("Password", password);
		if (GUILayout.Button("Create"))
		{
			//if (GameSparksSettings.ApiKey == "")
			//{
			CreateGame(login, password);
			//}
			this.Close();
		}
		if (GUILayout.Button("Cancel"))
		{
			this.Close();
		}
	}

	static void CreateGame(string dest_login, string dest_password)
	{
		string HOST = "https://config2.gamesparks.net/";
		string REST_URL = HOST + "restv2/game";

		var Json_config = LoadResourceTextfile("config.json");
		Json_config = Json_config.Replace("Jelly Garden", game_name);
		string url_put = REST_URL + "/config";
		WebClient wc = new WebClient();
		string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(dest_login + ":" + dest_password));
		wc.Headers[HttpRequestHeader.Authorization] = string.Format(
			"Basic {0}", credentials);

		string put = wc.UploadString(url_put, "Post", Json_config);

		var parsedJSON = GSJson.From(put) as IDictionary<string, object>;
		string apiKey = parsedJSON["apiKey"].ToString();
		Debug.Log("Game created " + apiKey);

		//		GameSparksSettings.ApiKey = apiKey;  
		//		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		Application.OpenURL("https://portal2.gamesparks.net");
	}



	public static string LoadResourceTextfile(string path)
	{
		string filePath = path.Replace(".json", "");

		TextAsset targetFile = Resources.Load<TextAsset>(filePath);

		return targetFile.text;
	}
}
#endif