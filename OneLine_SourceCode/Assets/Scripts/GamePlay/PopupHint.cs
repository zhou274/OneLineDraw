﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using UnityEngine.Analytics;

public class PopupHint : MonoBehaviour
{
	public Text tHintAmount;

    public GameObject popupRewardVideo;

    public string clickid;
    private StarkAdManager starkAdManager;
    void OnEnable ()
	{
		//tHintAmount.text = GameDefine.hintCount.ToString ();
		GameManager.gameState = GameState.PAUSING;
	}

	void Start ()
	{
		GameDefine.instance.OnHintCountChange += this.OnHintCountChange;
	}

	void OnHintCountChange ()
	{
		//tHintAmount.text = GameDefine.hintCount.ToString ();
	}

	public void Cancel ()
	{
		gameObject.SetActive (false);
		GameManager.gameState = GameState.PLAYING;
	}

	public void UseHint ()
	{
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    GW_GAME_PLAY.instance.Hint();
                    gameObject.SetActive(false);
                    GameManager.gameState = GameState.PLAYING;


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
        //if (GameDefine.hintCount > 0) {
        //	GameDefine.hintCount--;
        //	GW_GAME_PLAY.instance.Hint ();
        //	gameObject.SetActive (false);
        //	GameManager.gameState = GameState.PLAYING;
        //}
        //      else
        //      {
        //          gameObject.SetActive(false);
        //          popupRewardVideo.gameObject.SetActive(true);
        //      }
    }
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
}
