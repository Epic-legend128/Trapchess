<<<<<<< Updated upstream
/* using System.Collections;
=======
using System;
using System.Collections;
>>>>>>> Stashed changes
using System.Collections.Generic;
using Unity.Collections;
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
<<<<<<< Updated upstream
    private List<NetworkConnection> Connections;
=======
    private NativeList<NetworkConnection> Connections;
>>>>>>> Stashed changes
    private bool ServerActive = false;
    private const float Delay = 20f;
    private float LastDelay;
    public System.Action<string> Drop;
    // [SerializeField] int PORT = 8000;

    public void __init__(ushort PORT)
    {
        NetDrive = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = PORT;

        if (NetDrive.Bind(endPoint) != 0)
        {
            Debug.Log($"[SERVER ERROR] Unable to reach port {endPoint.Port}");
            return;
        }
        else
        {
            NetDrive.Listen();
            Debug.Log($"[SERVER] Listening to port {endPoint.Port}");
        }

        Connections = new NativeList<NetworkConnection>(2, Allocator.Persistent); // Max amount is 2
        ServerActive = true;
    }
<<<<<<< Updated upstream
}*/
=======
    public void StopServing()
    {
        if (ServerActive == true)
        {
            NetDrive.Dispose();
            Connections.Dispose();
            ServerActive = false;
        }
    }
    public void OnDestroy()
    {
        StopServing();
    }
    public void Update()
    {
        if (ServerActive == false)
        {
            return;
        }
        // ServerStatusSend();

        NetDrive.ScheduleUpdate().Complete();
        DestroyBadServer();
        GetConnection();
        UpdateMsgPipe();
    }
    private void DestroyBadServer()
    {
        for (int i = 0; i < Connections.Length; i++)
        {
            if (!Connections[i].IsCreated)
            {
                Connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }
    private void GetConnection()
    {
        NetworkConnection NetConnection;
        while ((NetConnection = NetDrive.Accept()) != default(NetworkConnection))
        {
            Connections.Add(NetConnection);
        }
    }
    private void UpdateMsgPipe()
    {
        DataStreamReader stream;
        for (int i = 0; i < Connections.Length; i++)
        {
            NetworkEvent.Type command;
            while ((command = NetDrive.PopEventForConnection(Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (command == NetworkEvent.Type.Data)
                {
                    // NetUtility.OneData(stream, Connections[i], this);
                }
                else if (command == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("[SERVER] Client disconnection");
                    Connections[i] = default(NetworkConnection);
                    Drop?.Invoke();
                    StopServing();
                }
            }
        }
    }
    public void SendToClient(NetworkConnection currentConnection, NetMessage message)
    {
        DataStreamWriter writer;
        NetDrive.BeginSend(currentConnection, out writer);
        message.Serialize(ref writer);
        driver.EndSend(writer);
    }
    public void BroadCast(NetMessage message)
    {
        for (int i = 0; i < Connections.Length; i++)
        {
            if (Connections[i].IsCreated)
            {
                // Debug.Log($"[SERVER] Sending {message.Code} to {Connections[i].InternalId}");
                SendToClient(Connections[i], msg);
            }
        }
    }

}
>>>>>>> Stashed changes
