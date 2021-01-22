using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Xml;


public class LevelData
{
	public static LevelData Instance;

	public static int[] map = new int[11 * 70];

	//List of mission in this map
	private static float limitAmount = 40;

	public static float LimitAmount {
		get {
			return LevelData.limitAmount;
		}
		set {
			LevelData.limitAmount = value;
			if (value < 0)
				LevelData.limitAmount = 0;
		}
	}

	static TargetManager targetManager;
	private static bool startReadData;
	public static Dictionary<int, ItemColor> colorsDict = new Dictionary<int, ItemColor>();
	static int key;
	public static int colors;
	public static int star1;
	public static int star2;
	public static int star3;

	private static int maxCols;
	private static int maxRows;
	public static int[] powerups = new int[4];


	#region Target

	static void SetTarget(TargetType targetType)
	{
		targetManager = new TargetManager(targetType);
	}

	public static bool IsTargetCubs()
	{
		if (targetManager == null)
			return false;
		return targetManager.GetTarget() == TargetType.RescuePets;
	}

	public static int GetTargetCount()
	{
		return targetManager.GetTargetCount();
	}

	public static int GetTotalTargetCount()
	{
		return targetManager.GetTotalTargetCount();
	}

	public static TargetType GetTargetOnLevel(int levelNumber)
	{
		LoadLevel(levelNumber);
		return GetTarget();
	}

	public static TargetType GetTarget()
	{
		return targetManager.GetTarget();
	}

	public static void AddTargetCount(int inc)
	{
		targetManager.AddTargetCount(inc);
	}

	public static void SetTotalTargetCount(int inc)
	{
		targetManager.SetTotalTargetCount(inc);
	}

	public static bool CheckTargetComplete()
	{
		return targetManager.CheckTargetComplete() && mainscript.Instance.stars > 0;
	}

	#endregion


	#region Level

	public static bool LoadLevel(int currentLevel)
	{
		//Read data from text file
		TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
		if (mapText == null) {
			mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
		}
		ProcesDataFromString(mapText.text);
		return true;
	}

	static void ProcesDataFromString(string mapText)
	{
		string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
		LevelData.colorsDict.Clear();
		foreach (string line in lines) {
			if (line.StartsWith("MODE ")) {
				string modeString = line.Replace("MODE", string.Empty).Trim();
				SetTarget((TargetType)int.Parse(modeString));
			}
		}
	}

	public static void LoadLevel(Action<int, int> Callback)
	{
		mainscript.Instance.currentLevel = PlayerPrefs.GetInt("OpenLevel");// TargetHolder.level;
		if (mainscript.Instance.currentLevel == 0)
			mainscript.Instance.currentLevel = 1;
		LoadDataFromLocal(mainscript.Instance.currentLevel);
		Callback(maxRows, maxCols);
	}


	public static bool LoadDataFromLocal(int currentLevel)
	{
		//Read data from text file
		TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
		if (mapText == null) {
			mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
		}
		ProcessGameDataFromString(mapText.text);
		return true;
	}

	static void ProcessGameDataFromString(string mapText)
	{
		string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
		LevelData.colorsDict.Clear();
		int mapLine = 0;
		int key = 0;
		int pets = 0;
		foreach (string line in lines) {
			if (line.StartsWith("MODE ")) {
				string modeString = line.Replace("MODE", string.Empty).Trim();
				SetTarget((TargetType)int.Parse(modeString));
			} else if (line.StartsWith("SIZE ")) {
				string blocksString = line.Replace("SIZE", string.Empty).Trim();
				string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				maxCols = int.Parse(sizes[0]);
				maxRows = int.Parse(sizes[1]);
			} else if (line.StartsWith("LIMIT ")) {
				string blocksString = line.Replace("LIMIT", string.Empty).Trim();
				string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				LevelData.LimitAmount = int.Parse(sizes[1]);

			} else if (line.StartsWith("COLOR LIMIT ")) {
				string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
				LevelData.colors = int.Parse(blocksString);
			} else if (line.StartsWith("STARS ")) {
				string blocksString = line.Replace("STARS", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				LevelData.star1 = int.Parse(blocksNumbers[0]);
				LevelData.star2 = int.Parse(blocksNumbers[1]);
				LevelData.star3 = int.Parse(blocksNumbers[2]);
			} else if (line.StartsWith("POWERUPS ")) {
				string blocksString = line.Replace("POWERUPS", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < 4; i++) {
					powerups[i] = int.Parse(blocksNumbers[i]);
				}
			} else { //Maps
					 //Split lines again to get map numbers
				string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < st.Length; i++) {
					int value = int.Parse(st[i].ToString());
					ItemKind itemKind = LevelEditorBase.THIS.items[value];
					if (!LevelData.colorsDict.ContainsValue(itemKind.color) && (itemKind.itemType == ItemTypes.Simple || itemKind.itemType == ItemTypes.Cub) && itemKind.color != ItemColor.random && itemKind.color != ItemColor.Unbreakable && value > 0) {
						LevelData.colorsDict.Add(key, itemKind.color);
						key++;
					}
					int item = int.Parse(st[i].ToString());
					if (item > 0 && item < LevelEditorBase.THIS.items.Count) {
						if (LevelEditorBase.THIS.items[item].itemType == ItemTypes.Cub)
							pets++;
					}
					LevelData.map[mapLine * maxCols + i] = item;
				}
				mapLine++;
			}
		}
		//random colors
		if (LevelData.colorsDict.Count == 0) {
			//add constant colors 
			LevelData.colorsDict.Add(0, ItemColor.YELLOW);
			LevelData.colorsDict.Add(1, ItemColor.RED);

			//add random colors
			List<ItemColor> randomList = new List<ItemColor>();
			randomList.Add(ItemColor.BLUE);
			randomList.Add(ItemColor.GREEN);
			//if (LevelData.GetTarget() != TargetType.Round)
			randomList.Add(ItemColor.VIOLET);
			for (int i = 0; i < LevelData.colors - 2; i++) {
				ItemColor randCol = ItemColor.YELLOW;
				while (LevelData.colorsDict.ContainsValue(randCol)) {
					randCol = randomList[UnityEngine.Random.RandomRange(0, randomList.Count)];
				}
				LevelData.colorsDict.Add(2 + i, randCol);

			}

		}

		if (GetTarget() == TargetType.Top)
			SetTotalTargetCount(6);
		else if (GetTarget() == TargetType.Round)
			SetTotalTargetCount(1);
		else {
			SetTotalTargetCount(pets);
		}
	}

	#endregion

	//
}
