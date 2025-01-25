using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WindowManager : MonoBehaviour
{
	public static WindowManager instance;
	public List<WindowModule> listWindows;
	public WindowModule openingWindow = null;
	float timeTween = 0.5F;

	void Awake ()
	{
		instance = this;
		openingWindow = null;
	}

	WindowModule GetWindowByName (WindowName name)
	{
		for (int i = 0; i < listWindows.Count; i++) {
			WindowModule m = listWindows [i];
			if (listWindows [i].name == name) {
				return m;
			}
		}
		return null;
	}

	public static void OpenWindow (WindowName name)
	{
		//Debug.Log ("Openingwindow la : " + instance.openingWindow.name);
		if (instance.openingWindow != null) {
			instance.StartCoroutine (instance.CloseWindowCrt ());
			instance.StartCoroutine (instance.OpenWindowCrt (name, instance.maxDuration));
		} else {
			instance.StartCoroutine (instance.OpenWindowCrt (name, 0));
		}
	}

	public IEnumerator OpenWindowCrt (WindowName name, float time)
	{
		yield return new WaitForSeconds (time);
		WindowModule m = GetWindowByName (name);
		m.obj.SetActive (true);
		DOTweenAnimation[] listTween = m.obj.transform.GetComponentsInChildren<DOTweenAnimation> ();
		for (int i = 0; i < listTween.Length; i++) {
			if (listTween [i].autoPlay) {
				listTween [i].tween.PlayForward ();
			}
		}
		instance.openingWindow = m;
	}

	float maxDuration = 0;

	public IEnumerator CloseWindowCrt ()
	{
		GameObject obj = instance.openingWindow.obj;
		maxDuration = 0;
		DOTweenAnimation[] listTween = instance.openingWindow.obj.transform.GetComponentsInChildren<DOTweenAnimation> ();
		for (int i = 0; i < listTween.Length; i++) {
			
			if (listTween [i].autoPlay) {
				if (listTween [i].duration > maxDuration) {
					maxDuration = listTween [i].duration;
				}
				listTween [i].tween.SmoothRewind ();
			}
		}
		//Debug.Log ("MaxDuration la : " + maxDuration);
		yield return new WaitForSeconds (maxDuration);
		obj.SetActive (false);
	}

}

[System.Serializable]
public class WindowModule
{
	public WindowName name;
	public GameObject obj;
}

[System.Serializable]
public enum WindowName
{
	GW_GAME_SELECT,
	GW_LEVEL_PACK,
	GW_LEVEL_SELECT,
	GW_GAME_PLAY
}