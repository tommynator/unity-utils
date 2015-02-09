using UnityEngine;
using System.Collections;

public class UnityRuntime : MonoSingleton<UnityRuntime>
{
    public event System.Action OnGui = delegate { };
    public event System.Action OnGizmos = delegate { };
    public event System.Action OnUpdate = delegate { };
    public event System.Action OnFixedUpdate = delegate { };
    public event System.Action OnQuit = delegate { };
    public event System.Action OnWake = delegate { };

	void OnGUI ()
    {
        OnGui();
	}

    void OnDrawGizmos()
    {
        if(Camera.current == Camera.main)
            OnGizmos();
    }

    void Update()
    {
        OnUpdate();
    }

	void FixedUpdate()
	{
		OnFixedUpdate();
	}

    void OnApplicationQuit()
    {
        OnQuit();
    }

    void OnApplicationFocus(bool focusStatus) 
    {
        Debug.Log("OnApplicationFocus: " + focusStatus);
    }

    private void OnApplicationPause(bool isPaused)
    {
        Debug.Log("OnApplicationPause: " + isPaused);

        if(!isPaused)
            OnWake();
    }
}
