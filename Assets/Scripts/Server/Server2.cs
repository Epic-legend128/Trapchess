/* using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class Server2 : MonoBehaviour
{
    public static Server2 Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }
    public NetworkDriver NetDrive;
    private List<NetworkConnection> Connections;
    private bool ServerActive = false;
    private const float Delay = 20f;
    private float LastDelay;
    public System.Action<string> Drop;
    // [SerializeField] int PORT = 8000;
    public void Init(ushort PORT)
    {
        NetDrive = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        NetDrive.Port = PORT;
        NetDrive.Bind(endPoint);
    }
}*/