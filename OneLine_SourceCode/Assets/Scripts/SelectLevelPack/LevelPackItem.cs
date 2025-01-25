using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPackItem : MonoBehaviour
{
	public Text tPercentCompleted;
	public TextMeshProUGUI tPackName;

    public Image iProgress;
	LevelPackModule _levelPack;
	public int id;

	public LevelPackModule levelPack {
		get { return _levelPack; }
		set { 
			_levelPack = value;
			if(value.packType==PackType.Beginner)
			{
                tPackName.text = "初级难度";
            }
            else if(value.packType==PackType.Medium)
            {
                tPackName.text = "中等难度";
            }
            else if (value.packType == PackType.Expert)
            {
                tPackName.text = "专家难度";
            }
            else if (value.packType == PackType.Master)
            {
                tPackName.text = "大师难度";
            }
            //tPackName.text = value.packType.ToString ();
			int levelPassed = 0;
			switch (value.packType) {
			case PackType.Beginner:
				levelPassed = GameManager.dataSaveDict [GameManager.currentGameName].beginner.Count;
				break;
			case PackType.Medium:
				levelPassed = GameManager.dataSaveDict [GameManager.currentGameName].medium.Count;
				break;
			case PackType.Expert:
				levelPassed = GameManager.dataSaveDict [GameManager.currentGameName].expert.Count;
				break;
			case PackType.Master:
				levelPassed = GameManager.dataSaveDict [GameManager.currentGameName].master.Count;
				break;
			}
			float progress = (float)levelPassed / value.LevelsCount;
			iProgress.fillAmount = progress;
			tPercentCompleted.text = Mathf.RoundToInt (progress * 100).ToString () + "%";
		}
	}

	void OnEnable ()
	{
		
	}

	public void GotoSelectLevel ()
	{
		GameManager.currentPackType = levelPack.packType;
		//PackType type;
		if(tPackName.text=="初级难度")
		{
            GameManager.currentPackName = PackType.Beginner.ToString();
        }
		else if(tPackName.text == "中等难度")
		{
            GameManager.currentPackName = PackType.Medium.ToString();
        }
        else if (tPackName.text == "专家难度")
        {
            GameManager.currentPackName = PackType.Expert.ToString();
        }
		else if(tPackName.text == "大师难度")
		{
            GameManager.currentPackName = PackType.Master.ToString();
        }
        
		GameManager.currentPackModule = levelPack;
		GameManager.gameState = GameState.SELECT_LEVEL;
		WindowManager.OpenWindow (WindowName.GW_LEVEL_SELECT);
	}


}
