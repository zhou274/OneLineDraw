using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GameColorManager : MonoBehaviour
{
	public List<GameElementColorModeDefine> listColorModeDefine;
	GameElementColorModeDefine currentColorModeDefine;
	public int colorModeID;
	public static GameColorManager instance;

	void Awake ()
	{
		instance = this;
		currentColorModeDefine = listColorModeDefine [colorModeID];
	}

	public Action onColorChange;

	public void ChangeColor ()
	{
		colorModeID++;
		if (colorModeID >= listColorModeDefine.Count) {
			colorModeID = 0;
		}
		currentColorModeDefine = listColorModeDefine [colorModeID];
		if (onColorChange != null) {
			onColorChange ();
		}
		Camera.main.backgroundColor = GetColor (GameElementColorType.BACKGROUND);
	}

	public static Color GetColor (GameElementColorType type)
	{
		for (int i = 0; i < instance.currentColorModeDefine.listColorModule.Count; i++) {
			GameElementColorModule md = instance.currentColorModeDefine.listColorModule [i];
			if (md.type == type) {
				return md.color;
			}
		}
		return Color.white;
	}
}

[System.Serializable]
public class GameElementColorModeDefine
{
	public string colorModeName;
	public List<GameElementColorModule> listColorModule;
}

[System.Serializable]
public class GameElementColorModule
{
	public GameElementColorType type;
	public Color color;
}

[System.Serializable]
public enum GameElementColorType
{
	TOP_BAR,
	BOTTOM_BAR,
	BACKGROUND,
	MENU_USER_LEVEL_BG,
	MENU_USER_LEVEL_VALUE,
	TITLE_TEXT,
	SELECT_PACK_GAME_NAME,
	BEGINNER_COLOR,
	MEDIUM_COLOR,
	EXPERT_COLOR,
	MASTER_COLOR,
	LEVEL_BUTTON,
	GP_USER_LEVEL_BG,
	GP_USER_LEVEL_VALUE,
	GP_BACK_BUTTON,
	GP_GAME_NAME,
	GP_GAME_PACK_LEVEL,
	GP_CELL,
	GP_BOTTOM_BUTTON_1,
	GP_BOTTOM_BUTTON_2
}