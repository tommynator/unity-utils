using UnityEngine;
using System.Collections;
using System.IO;

public class PhotoBooth : MonoBehaviour
{
    public static string Snapshots = "";
    public static string GetFileName(int id)
    {
        return Snapshots + "/SavedScreen_" + id + ".png";
    }

    void Awake()
    {
        Snapshots = Application.persistentDataPath + "/snapshots";

        camera.targetTexture = new RenderTexture(512, 512, 16, RenderTextureFormat.Default);

        EventSystem.AddListener<int>("Take Snapshot", TakeSnapshot);

        Directory.CreateDirectory(Snapshots);
    }

    void TakeSnapshot(int id)
    { 
        var activeTarget = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;
        camera.Render();

        Texture2D tex = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        tex.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);

        // invert alpha channel
        var pix = tex.GetPixels();
        for(int i = 0; i < pix.Length; i++)
            pix[i].a = 1 - pix[i].a;
        tex.SetPixels(pix);

        tex.Apply();

        RenderTexture.active = activeTarget;
            
        // Encode texture into PNG 
        var bytes = tex.EncodeToPNG(); 
        Destroy (tex);
            
        // For testing purposes, also write to a file in the project folder 
        File.WriteAllBytes(GetFileName(id), bytes);
	}
}
