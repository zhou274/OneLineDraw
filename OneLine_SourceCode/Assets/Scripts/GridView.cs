using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridView : MonoBehaviour
{
	public Vector2 cellSize;
	public Vector2 Spacing;
	public GridLayoutGroup.Axis GridViewaxis;
	public GameObject contentPrefap;
	public List<GameObject> listContentObject;
	GameObject contentPanel;
	RectTransform contentRect, panelRect;
	public bool isTargetToCenter = false;
	EventTrigger e;
	Vector2 worldItemSize;
	public int selectedId;
	ScrollRect scrollRect;
	GameObject buttonPools;

	void Awake ()
	{
		buttonPools = new GameObject ();
		buttonPools.name = "Pools";
		buttonPools.transform.SetParent (transform);
		SimplePool.Preload (contentPrefap, 200, buttonPools.transform);
		gameObject.AddComponent<Mask> ();
		panelRect = gameObject.GetComponent<RectTransform> ();
		GameObject scrollObj = new GameObject ();
		scrollObj.transform.parent = transform;
		RectTransform scrollRectTransform = scrollObj.AddComponent<RectTransform> ();
		scrollObj.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
		scrollObj.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		scrollRectTransform.sizeDelta = new Vector2 (gameObject.GetComponent<RectTransform> ().rect.width, panelRect.rect.height);

		scrollObj.name = "Scroll";
		scrollRect = scrollObj.AddComponent<ScrollRect> ();

		// event 
		EventTrigger trigger = scrollObj.AddComponent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.EndDrag;
		entry.callback.AddListener ((data) => {
			EndDrag ();
		});
		trigger.triggers.Add (entry);
		EventTrigger.Entry entry2 = new EventTrigger.Entry ();
		entry2.eventID = EventTriggerType.Drag;
		entry2.callback.AddListener ((data) => {
			OnDrag ();
		});
		trigger.triggers.Add (entry2);


		// scroll setting 
		scrollRect.horizontal = false;
		scrollRect.vertical = true;
		scrollRect.movementType = ScrollRect.MovementType.Elastic;
		scrollRect.elasticity = 0.1F;

		// create content 

		contentPanel = new GameObject ();
		contentPanel.transform.parent = scrollObj.transform;
		contentPanel.name = "ContentPanel";
		contentRect = contentPanel.AddComponent<RectTransform> ();
		contentRect.pivot = new Vector2 (0.5F, 1);
		contentRect.anchoredPosition3D = new Vector3 (0, 0, 0);

		Image img = contentPanel.AddComponent<Image> ();
		img.color = new Color (1, 1, 1, 0);
		scrollRect.content = contentRect;

		contentRect.localScale = new Vector3 (1, 1, 1);

		GridLayoutGroup gridLayoutGroup = contentPanel.AddComponent<GridLayoutGroup> ();
		gridLayoutGroup.cellSize = cellSize;
		gridLayoutGroup.spacing = Spacing;
		gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
		gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;

		ContentSizeFitter contentSizeFitter = contentPanel.AddComponent<ContentSizeFitter> ();
		switch (GridViewaxis) {
		case GridLayoutGroup.Axis.Vertical: 
			contentRect.pivot = new Vector2 (0.5F, 1);
			gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
			gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
			float width = gameObject.GetComponent<RectTransform> ().rect.width;
			int a = Mathf.RoundToInt (width / (cellSize.x + Spacing.x));
			contentRect.sizeDelta = new Vector2 (a * (cellSize.x + Spacing.x) - Spacing.x, panelRect.rect.height);
			contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
			scrollRect.horizontal = false;
			scrollRect.vertical = true;
			break;
		case GridLayoutGroup.Axis.Horizontal:
			contentRect.pivot = new Vector2 (0, 0.5F);
			float height = gameObject.GetComponent<RectTransform> ().rect.height;
			int b = Mathf.RoundToInt (height / (cellSize.y + Spacing.y));
			contentRect.anchoredPosition = new Vector2 (0, 0);
			contentRect.sizeDelta = new Vector2 (b * (cellSize.y + Spacing.y) - Spacing.y, panelRect.rect.height);
			gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
			gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
			gridLayoutGroup.constraintCount = 1;
			gridLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
			gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
			contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
			contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
			scrollRect.horizontal = true;
			scrollRect.vertical = false;
			break;
		}
	}

	Vector2 top = new Vector2 (0, 1);

	void OnEnable ()
	{
		if (scrollRect != null) {
			scrollRect.normalizedPosition = top;
		}
	}

	public GameObject AddContent ()
	{
		//GameObject contentObj = (GameObject)Instantiate (contentPrefap, contentPanel.transform.position, Quaternion.Euler (0, 0, 0));
		//contentObj.transform.parent = contentPanel.transform;
		GameObject contentObj = SimplePool.Spawn (contentPrefap, contentPanel.transform.position, Quaternion.Euler (0, 0, 0));
		contentObj.transform.SetParent (contentPanel.transform);
		contentObj.transform.localScale = new Vector3 (1, 1, 1);
		listContentObject.Add (contentObj);
		return contentObj;
	}

	public void RemoveContent (GameObject contentObj)
	{
		contentObj.transform.SetParent (buttonPools.transform);
		SimplePool.Despawn (contentObj);
		listContentObject.Remove (contentObj);

	}

	public void ClearAllContent ()
	{
		GameObject[] temp = listContentObject.ToArray ();
		foreach (GameObject o in temp) {
			RemoveContent (o);
		}
	}

	void Update ()
	{
		if (isTargetToCenter) {
			float size = cellSize.x + Spacing.x;
			float maxSize = size * listContentObject.Count;
			float minDis = size;
			int c = 0;
			for (int i = 0; i < listContentObject.Count; i++) {
				float dis = Vector3.Distance (listContentObject [i].transform.position, transform.position);

				float scale = dis < 1F ? 1.8F - (dis * 0.8F) : 1;
				listContentObject [i].transform.localScale = new Vector3 (scale, scale, 1);
				if (dis < minDis) {
					minDis = dis;
					c = i;
					selectedId = c;
				}
			}

			if (isDraging == false) {
				//scrollRect.normalizedPosition = new Vector2 (0.333F, 0);
				scrollRect.normalizedPosition = Vector2.Lerp (scrollRect.normalizedPosition, new Vector2 ((1 / ((float)listContentObject.Count - 1)) * (float)c, 0), Time.deltaTime * 5);

			}
		}
	}

	bool isDraging = false;

	public void OnDrag ()
	{
		isDraging = true;
		//Debug.Log ("Scroll");
	}

	public void EndDrag ()
	{
		Invoke ("SetFalseDrag", 0.5F);

	}

	void SetFalseDrag ()
	{
		isDraging = false;
	}
}
