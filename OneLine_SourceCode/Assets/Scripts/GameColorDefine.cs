using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// khong dung scrip nay nua
[CreateAssetMenu (fileName = "GameColorDefine", menuName = "GameConfiguration/GameColorDefine", order = 1)]
public class GameColorDefine : ScriptableObject
{
	public List<GameColorModule> ListGameColors;


}

[System.Serializable]
public class GameColorModule
{
	// ten cua bo color
	public string name;
	// id cua bo color
	public int id;
	// color background
	public Color backgroundColor;
	public Color topBarColor;
	public Color bottomBarColor;
	public Color adsAreaColor;

	// color button
	public Color levelButtonNormal, levelButtonPassed;

	// color gameplay
	[Space (10)]
	[Header ("GamePlay Color")]
	public Color userLevelBgColor;
	public Color userLevelValueColor;
	public Color backButtonColor, gameNameColor, gameLevelPackColor, cellColor;
	public List<Color> listBlockColor;
	public Color gameplayMenuButtonColor, gameplayMenuButtonColor2;

}