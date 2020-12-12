using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using Mirror.Discovery;

[DisallowMultipleComponent]
//[AddComponentMenu("Network/NetworkManagerHUD")]
[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(NetworkDiscovery))]
[HelpURL("https://mirror-networking.com/docs/Articles/Components/NetworkManagerHUD.html")]
public class ExtendedHud : MonoBehaviour
{
    NetworkManager manager;

    /// <summary>
    /// Whether to show the default control HUD at runtime.
    /// </summary>
    public bool showGUI = true;

    /// <summary>
    /// The horizontal offset in pixels to draw the HUD runtime GUI at.
    /// </summary>
    public int offsetX;

    /// <summary>
    /// The vertical offset in pixels to draw the HUD runtime GUI at.
    /// </summary>
    public int offsetY;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    void OnGUI()
    {
        if (!showGUI)
            return;

        GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        // client ready
        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                ClientScene.Ready(NetworkClient.connection);

                if (ClientScene.localPlayer == null)
                {
                    ClientScene.AddPlayer(NetworkClient.connection);
                }
            }
        }

        StopButtons();

        GUILayout.EndArea();

        if (NetworkManager.singleton == null)
            return;

        if (NetworkServer.active || NetworkClient.active)
            return;

        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
            DrawGUI();
    }

    void StartButtons()
    {
        if (!NetworkClient.active)
        {
            // Server + Client
            /*
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (GUILayout.Button("Host (Server + Client)"))
                {
                    manager.StartHost();
                }
            }
            */

            // Client + IP
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Client"))
            {
                manager.StartClient();
            }
            manager.networkAddress = GUILayout.TextField(manager.networkAddress);
            GUILayout.EndHorizontal();

            // Server Only
            /*
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // cant be a server in webgl build
                GUILayout.Box("(  WebGL cannot be server  )");
            }
            else
            {
                if (GUILayout.Button("Server Only")) manager.StartServer();
            }
            */
        }
        else
        {
            // Connecting
            GUILayout.Label("Connecting to " + manager.networkAddress + "..");
            if (GUILayout.Button("Cancel Connection Attempt"))
            {
                manager.StopClient();
            }
        }
    }

    void StatusLabels()
    {
        // server / client status message
        if (NetworkServer.active)
        {
            GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
        }
        if (NetworkClient.isConnected)
        {
            GUILayout.Label("Client: address=" + manager.networkAddress);
        }
    }

    void StopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Host"))
            {
                manager.StopHost();
            }
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client"))
            {
                manager.StopClient();
            }
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            if (GUILayout.Button("Stop Server"))
            {
                manager.StopServer();
            }
        }
    }



    //--------------------------------

    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    Vector2 scrollViewPos = Vector2.zero;

    public NetworkDiscovery networkDiscovery;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (networkDiscovery == null)
            {
                networkDiscovery = GetComponent<NetworkDiscovery>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
                UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
            }
        }
#endif
    /*
    void OnGUI()
    {
        if (NetworkManager.singleton == null)
            return;

        if (NetworkServer.active || NetworkClient.active)
            return;

        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
            DrawGUI();
    }
    */

    void DrawGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Find Servers"))
        {
            discoveredServers.Clear();
            networkDiscovery.StartDiscovery();
        }

        // LAN Host
        if (GUILayout.Button("Start Host"))
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartHost();
            networkDiscovery.AdvertiseServer();
        }

        // Dedicated server
        /*
        if (GUILayout.Button("Start Server"))
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartServer();

            networkDiscovery.AdvertiseServer();
        }
        */

        GUILayout.EndHorizontal();

        // show list of found server

        GUILayout.Label($"Discovered Servers [{discoveredServers.Count}]:");

        // servers
        scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);

        foreach (ServerResponse info in discoveredServers.Values)
            if (GUILayout.Button(info.EndPoint.Address.ToString()))
                Connect(info);

        GUILayout.EndScrollView();
    }

    void Connect(ServerResponse info)
    {
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
    }

}
