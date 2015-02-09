using UnityEngine;
using System.Collections;

public static class ExtensionMethods 
{
    public static void SetLayerRecursively(this GameObject obj, string layer) 
    {
        obj.SetLayerRecursively(LayerMask.NameToLayer(layer));
    }

	public static void SetLayerRecursively(this GameObject obj, int layer) 
	{
        obj.layer = layer;
 
        foreach (Transform child in obj.transform) 
        {
            child.gameObject.SetLayerRecursively(layer);
        }
    }

    public static GameObject Clone(this GameObject obj, Vector3 position = default(Vector3))
    {
        return GameObject.Instantiate(obj, position, Quaternion.identity) as GameObject;
    }

    public static Vector3 Add(this Vector3 vec, float x, float y, float z) 
    {
        vec.x += x;
        vec.y += y;
        vec.z += z;
        return vec;
    }
}
