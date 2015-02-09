using UnityEngine;
using System.Collections;

public class DestroyBehaviour : MonoBehaviour 
{
	[SerializeField] int seconds;

	// Use this for initialization
	void Start () 
	{
		GameObject.Destroy(gameObject, seconds);
	}
}