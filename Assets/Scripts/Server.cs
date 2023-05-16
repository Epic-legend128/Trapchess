/* using UnityEngine;
using System.Collections;
using WebSocketSharp;
 
public class Node : MonoBehaviour {
 
    void Start () {
        using (var ws = new WebSocket ("localhost:8080")) {
            ws.OnMessage += (sender, e) =>
                Debug.Log (e.Data);
           
            ws.Connect ();
            ws.Send ("message");
        }
    }
 
}
}

*/