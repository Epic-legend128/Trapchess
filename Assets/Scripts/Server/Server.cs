using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
        
    private void Start() {
        StartCoroutine(GetWebData("http://localhost:8080/account/", "?rUsername=Joh&rPassword=Abc123"));
        // Even though unstable we create a reference where the model can connect to on a url
    }

        IEnumerator GetWebData(string addr, string ID)
    {
        /*
        string addr : url or address
        ID : query
        */
        UnityWebRequest req = UnityWebRequest.Get(addr + ID);
        yield return req.SendWebRequest(); 

        if (req.result != UnityWebRequest.Result.Success)
            Debug.LogError("[ERROR] -> Something went wrong >_ " + req.error);
        else
            Debug.Log("[STARTED] -> " + req.downloadHandler.text + " is the output of the operation.");
    }
}

// Hey fotis, here in case you forget :

// 1. Run $ node server.js on the correct directory
// 2. Run Game from Unity
// 3. Look at both node and unity console logging messages