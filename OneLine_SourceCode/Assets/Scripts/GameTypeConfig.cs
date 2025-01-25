using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "GameTypeConfig", menuName = "GameConfiguration/GameTypeConfig", order = 1)]
public class GameTypeConfig : ScriptableObject
{
	public List<GameTypeModule> listGames;

}

[System.Serializable]
public class GameTypeModule
{
	public Sprite gameLogo;
	public string gameName;
	public string gameNameShow;
	public LevelPackModule[] levelPacks;
}