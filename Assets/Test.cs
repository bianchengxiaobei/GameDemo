using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        FileAccessManager.ZipFile(Application.dataPath + "/Plugin/network.dll", Application.dataPath + "/Plugin/");
	}
}
