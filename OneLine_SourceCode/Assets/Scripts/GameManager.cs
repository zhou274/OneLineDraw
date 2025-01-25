using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static int currentGameColorModeID = 0;
	// ten(id) cua game dang chon
	public static string currentGameName = "OneLine";
	// data cac man choi
	public static Dictionary <string,GameDataModule> GameData;
	// data cua man choi trong ganemName,packs dang chon
	public static GameDataModule currentGameData;
	// packname dang chon
	public static string currentPackName;
	// pack dang chon (tem, so level)
	public static PackType currentPackType;
	// level hien tai dang choi
	public static int currentLevel;
	// data luu tru , key la ten(id) game
	public static Dictionary<string,DataSaveModule> dataSaveDict;
	// gametYPE dang chon , ben trong chua ten game , danh sach pack ,ten game hien thi
	public static GameTypeModule currentGameType;
	public static LevelPackModule currentPackModule;
	public static GameState gameState;

	public static void LoadGameData (string gameName)
	{
		if (GameData == null) {
			GameData = new Dictionary<string, GameDataModule> ();
		}
		if (GameData.ContainsKey (gameName)) {
			currentGameData = GameData [gameName];
			return;
		}
		GameDataModule data = new GameDataModule ();
		data.gameName = gameName;
		data.Data = new Dictionary<string, Dictionary<int,string>> ();
		GameData.Add (gameName, data);

		currentGameData = data;
	}

	public static string LoadLevel (string packName, int level)
	{
		if (currentGameData.Data.ContainsKey (packName)) {
			if (currentGameData.Data [packName].ContainsKey (level)) {
				return currentGameData.Data [packName] [level];
			}
		} else {
			Dictionary<int,string> data = new Dictionary<int, string> ();
			currentGameData.Data.Add (packName, data);
		}

		TextAsset ta = Resources.Load ("Data/" + currentGameData.gameName + "/" + packName + "/Level_" + level)as TextAsset;
		if (ta == null) {
			return "";
		}
		currentGameData.Data [packName].Add (level, ta.text);
		string dataStr = ta.text;
		Resources.UnloadAsset (ta);
		return dataStr;
	}


	public static void LoadDataSave ()
	{
		if (dataSaveDict == null) {
			dataSaveDict = new Dictionary<string, DataSaveModule> ();
		}
		DataSaveModule data;
		string json = PlayerPrefs.GetString (currentGameName, "");
		#if UNITY_EDITOR
		//json = "";
		#endif
		if (json == "") {
			data = new DataSaveModule ();
			data.beginner = new List<int> ();
			data.medium = new List<int> ();
			data.expert = new List<int> ();
			data.master = new List<int> ();
			if (data != null && !dataSaveDict.ContainsKey (currentGameName)) {
				dataSaveDict.Add (currentGameName, data);
			}
			SaveData ();
		} else {
			data = JsonUtility.FromJson<DataSaveModule> (json);
		}

		if (data != null && !dataSaveDict.ContainsKey (currentGameName)) {
			dataSaveDict.Add (currentGameName, data);
		}
	}

	public static void SaveData ()
	{
		DataSaveModule data = dataSaveDict [currentGameName];
		string json = JsonUtility.ToJson (data);
		PlayerPrefs.SetString (GameManager.currentGameName, json);
		PlayerPrefs.Save ();
	}


	public static GameTypeModule GetGameConfigByName (string gameName)
	{
		for (int i = 0; i < GameDefine.instance.gameTypeConfig.listGames.Count; i++) {
			GameTypeModule gCf = GameDefine.instance.gameTypeConfig.listGames [i];
			if (gCf.gameName == gameName) {
				return gCf;
			}
		}
		return null;
	}
	// lay mau toi hon cua mau hien tai
	public static Color GetDarkColor (Color col)
	{
		float r = col.r * 255 > 20 ? ((col.r * 255) - 20) / 255 : 0;
		float g = col.g * 255 > 20 ? ((col.g * 255) - 20) / 255 : 0;
		float b = col.b * 255 > 20 ? ((col.b * 255) - 20) / 255 : 0;
		return new Color (r, g, b, col.a);
	}



	public static void GetListLevelPassed (PackType type, ref List<int> list)
	{
		switch (GameManager.currentPackType) {
		case PackType.Beginner:
			list = GameManager.dataSaveDict [GameManager.currentGameName].beginner;
			break;
		case PackType.Medium:
			list = GameManager.dataSaveDict [GameManager.currentGameName].medium;
			break;
		case PackType.Expert:
			list = GameManager.dataSaveDict [GameManager.currentGameName].expert;
			break;
		case PackType.Master:
			list = GameManager.dataSaveDict [GameManager.currentGameName].master;
			break;
		}
	}
}

[System.Serializable]
public class PackInfoModule
{
	public List<LevelPackModule> Packs;
}

[System.Serializable]
public class LevelPackModule
{
	public PackType packType;
	public int LevelsCount;
}

[System.Serializable]
public enum PackType
{
	Beginner,
	Medium,
	Expert,
	Master
}

[System.Serializable]
public class GameDataModule
{
	public string gameName;
	public PackInfoModule packInfo;
	// data cua man choi
	public Dictionary<string,Dictionary<int,string>> Data;
}

[System.Serializable]
public class DataSaveModule
{
	// nhung level nao da pass thi cho vao list nay .
	public List<int> beginner, medium, expert, master;
}

[System.Serializable]
public enum GameState
{
	SELECT_GAME,
	SELECT_PACK,
	SELECT_LEVEL,
	PLAYING,
	WIN,
	PAUSING

}