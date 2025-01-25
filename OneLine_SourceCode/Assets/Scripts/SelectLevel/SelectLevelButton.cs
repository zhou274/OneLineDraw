using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectLevelButton : MonoBehaviour
{
	int _level;
	public DOTweenAnimation aniShow, aniHide;
	public Image iSurface, iDark;
	public Color surfaceColor, darkColor;

	public int level {
		get { return _level; }
		set {
			_level = value;
			tLevel.text = value.ToString ();
		}
	}

	bool _passed = false;

	public bool passed {
		get { return _passed; }
		set {
			_passed = value;
			if (value) {
				iSurface.color = GW_LEVEL_SELECT.instance.passedColor;
				iDark.color = GameManager.GetDarkColor (iSurface.color);
			} else {
				iSurface.color = GW_LEVEL_SELECT.instance.normalColor;
				iDark.color = GameManager.GetDarkColor (iSurface.color);
			}
		}
	}

	public Text tLevel;

	void OnEnable ()
	{
		if (GW_LEVEL_SELECT.instance != null) {
			passed = _passed;
		}
	}

	public void OnClick ()
	{
		AudioManager.instance.PlaySound (AudioClipType.LevelStart);
		GameManager.currentLevel = level;
		GW_LEVEL_SELECT.instance.HideAllLevelTween ();
		WindowManager.OpenWindow (WindowName.GW_GAME_PLAY);
	}
}
