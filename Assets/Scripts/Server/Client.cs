using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }

    public NetworkDriver NetDrive;
    private NetworkConnection SingularConnection;
    private bool ServerActive = false;
    public Action Drop;

    public void __init__(string IP_ADDRESS, ushort PORT)
    {   
        //Debug.Log(IP_ADDRESS);
        NetDrive = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(IP_ADDRESS, PORT);
        SingularConnection = NetDrive.Connect(endPoints);
        ServerActive = true;
        EventListener();
    }

    public void StopServing()
    {
        if (ServerActive == true)
        {
            EventDestroyer();
            NetDrive.Dispose();
            ServerActive = false;
            SingularConnection = default(NetworkConnection);
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
        NetDrive.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMsgPipe();
    }

    private void CheckAlive()
    {
        if (!SingularConnection.IsCreated && isActive)
        {
            Debug.Log("[SERVER ERROR] Connection Lost ;(");
            Drop?.Invoke();
            StopServing();
        }
    }

    private void UpdateMsgPipe()
    {
        DataStreamReader stream;
        NetworkEvent.Type command;
        while ((command = SingularConnection.PopEvent(NetDrive, out stream)) != NetworkEvent.Type.Empty) ;
        {
            if (command == NetworkEvent.Type.Connect)
            {
                SendToServer(new NetWelcome());
            }
            else if (command == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(stream, default(NetworkConnection));
            }
            else if (command == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("[SERVER] Client has disconnected");
                SingularConnection = default(NetworkConnection);
                ConnectionDrop?.Invoke();
                StopServing();
            }
        }
    }

    public void SendToServer(NetworkMessage message)
    {
        DataStreamWriter writer;
        NetDrive.BeginSend(SingularConnection, out writer);
        // message.Serialize(ref writer);
        driver.EndSend(writer);
    }

    private void EventListener()
    {
        // NetUtility.KeepConnectionStable += OnKeepAlive;
    }

    private void EventDestroyer()
    {
        // NetUtility.KeepConnectionStable -= OnKeepAlive;
    }

    private void OnKeepAlive(NetworkMessage message)
    {
        SendToServer(message);
    }
}

/*
Fotis, just if you are confused, when calling a function I also check if it returns null : 

    <functionName?.Invoke()>
                ^^^
    This is used to check if it is null, or the driver would stop communicating.
*/
