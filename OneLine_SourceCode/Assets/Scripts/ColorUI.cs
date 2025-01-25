using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorUI : MonoBehaviour
{

	SpriteRenderer sprRender;
	Image imageRender;
	Text textRender;
	public GameElementColorType type;

	void Awake ()
	{
		sprRender = GetComponent<SpriteRenderer> ();
		textRender = GetComponent<Text> ();
		imageRender = GetComponent<Image> ();
	}

	void Start ()
	{
		// event thay UI color
		GameColorManager.instance.onColorChange += new System.Action (OnChangeColor);
		OnChangeColor ();
	}

	void OnChangeColor ()
	{
		Color col = GameColorManager.GetColor (type);
		if (sprRender != null) {
			sprRender.color = col;
		}
		if (imageRender != null) {
			imageRender.color = col;
		}
		if (textRender != null) {
			textRender.color = col;
		}
	}
}
