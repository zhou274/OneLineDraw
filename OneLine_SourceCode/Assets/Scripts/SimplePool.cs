
using System;
using System.Collections.Generic;
using UnityEngine;

public static class SimplePool
{
	private static void Init (GameObject prefab = null, int qty = 3)
	{
		if (SimplePool._pools == null) {
			SimplePool._pools = new Dictionary<int, SimplePool.Pool> ();
		}
		if (prefab != null) {
			int instanceID = prefab.GetInstanceID ();
			if (!SimplePool._pools.ContainsKey (instanceID)) {
				SimplePool._pools [instanceID] = new SimplePool.Pool (prefab, qty);
			}
		}
	}

	public static GameObject[] Preload (GameObject prefab, int qty = 1, Transform newParent = null)
	{
		SimplePool.Init (prefab, qty);
		GameObject[] array = new GameObject[qty];
		for (int i = 0; i < qty; i++) {
			array [i] = SimplePool.Spawn (prefab, Vector3.zero, Quaternion.identity);
			if (newParent != null) {
				array [i].transform.SetParent (newParent);
			}
		}
		for (int j = 0; j < qty; j++) {
			SimplePool.Despawn (array [j]);
		}
		return array;
	}

	public static GameObject Spawn (GameObject prefab, Vector3 pos, Quaternion rot)
	{
		SimplePool.Init (prefab, 3);
		return SimplePool._pools [prefab.GetInstanceID ()].Spawn (pos, rot);
	}

	public static GameObject Spawn (GameObject prefab)
	{
		return SimplePool.Spawn (prefab, Vector3.zero, Quaternion.identity);
	}

	public static T Spawn<T> (T prefab) where T : Component
	{
		return SimplePool.Spawn<T> (prefab, Vector3.zero, Quaternion.identity);
	}

	public static T Spawn<T> (T prefab, Vector3 pos, Quaternion rot) where T : Component
	{
		SimplePool.Init (prefab.gameObject, 3);
		return SimplePool._pools [prefab.gameObject.GetInstanceID ()].Spawn<T> (pos, rot);
	}

	public static void Despawn (GameObject obj)
	{
		SimplePool.Pool pool = null;
		foreach (SimplePool.Pool pool2 in SimplePool._pools.Values) {
			if (pool2.MemberIDs.Contains (obj.GetInstanceID ())) {
				pool = pool2;
				break;
			}
		}
		if (pool == null) {
			UnityEngine.Debug.LogFormat ("Object '{0}' wasn't spawned from a pool. Destroying it instead.", new object[] {
				obj.name
			});
			UnityEngine.Object.Destroy (obj);
		} else {
			pool.Despawn (obj);
		}
	}

	public static int GetStackCount (GameObject prefab)
	{
		if (SimplePool._pools == null) {
			SimplePool._pools = new Dictionary<int, SimplePool.Pool> ();
		}
		if (prefab == null) {
			return 0;
		}
		return (!SimplePool._pools.ContainsKey (prefab.GetInstanceID ())) ? 0 : SimplePool._pools [prefab.GetInstanceID ()].StackCount;
	}

	public static void ClearPool ()
	{
		if (_pools != null && _pools.Keys.Count > 0) {
			foreach (KeyValuePair<int,Pool> keyvalue in _pools) {
				Pool _pool = keyvalue.Value;
				foreach (GameObject o in _pool._inactive) {
					UnityEngine.Object.Destroy (o);
				}
				foreach (GameObject o in _pool._active) {
					UnityEngine.Object.Destroy (o);
				}
			}
		}
		
		if (SimplePool._pools != null) {
			SimplePool._pools.Clear ();
		}
	}

	private const int DEFAULT_POOL_SIZE = 3;

	public static Dictionary<int, SimplePool.Pool> _pools;

	public class Pool
	{
		public Pool (GameObject prefab, int initialQty)
		{
			this._prefab = prefab;
			this._inactive = new Queue<GameObject> (initialQty);
			this._active = new List<GameObject> ();
			this.MemberIDs = new HashSet<int> ();
		}

		public int StackCount {
			get {
				return this._inactive.Count;
			}
		}

		public GameObject Spawn (Vector3 pos, Quaternion rot)
		{
			GameObject gameObject;
			//ABS:
			while (this._inactive.Count != 0) {
				gameObject = this._inactive.Dequeue ();
				if (!(gameObject == null)) {
					//
					this._active.Add (gameObject);
					gameObject.transform.position = pos;
					gameObject.transform.rotation = rot;
					gameObject.SetActive (true);
					return gameObject;
				}
			}
			gameObject = UnityEngine.Object.Instantiate<GameObject> (this._prefab, pos, rot);
			gameObject.name = string.Format ("{0} ({1})", this._prefab.name, this._nextId++);
			this.MemberIDs.Add (gameObject.GetInstanceID ());
			return gameObject;
			//goto ABS;
		}

		public T Spawn<T> (Vector3 pos, Quaternion rot)
		{
			return this.Spawn (pos, rot).GetComponent<T> ();
		}

		public void Despawn (GameObject obj)
		{
			if (!obj.activeSelf) {
				return;
			}

			obj.SetActive (false);
			this._active.Remove (obj);
			this._inactive.Enqueue (obj);
		}

		private int _nextId = 1;

		public Queue<GameObject> _inactive;
		public List<GameObject> _active;

		public readonly HashSet<int> MemberIDs;

		private readonly GameObject _prefab;
	}
}
