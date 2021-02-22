using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;


public class NetworkManager : MonoBehaviour {
    public static NetworkManager Instance;
    public uint seed;
    public bool connected;
    public bool isServer;

    private Socket handler;
    private Socket sender;

    public void Start() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }

        connected = false;
        isServer = false;
        DontDestroyOnLoad(this);
    }

    public void StartServer() {
        IPHostEntry host = Dns.GetHostEntry("127.0.0.1");
        IPAddress ipAddress = host.AddressList[0];
        Debug.Log("IP Address: " + ipAddress.ToString());
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11234); //Running on port 11234

        try {
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);

            listener.Listen(1);

            Debug.Log("Waiting for a connection...");
            //TODO: Close the socket when clicking off of Host screen
            //listener.BeginAccept(new AsyncCallback(OnSocketAccept), listener);
            handler = listener.Accept();
            Debug.Log("Connection made!");
            byte[] dataBytes = new byte[100];
            string data = null;
            int bytesRec = handler.Receive(dataBytes);
            data += Encoding.ASCII.GetString(dataBytes, 0, bytesRec);
            Debug.Log("Read: " + data);

            seed = uint.Parse(data.Substring(6));
            //Shut down socket and close connection
            //handler.Shutdown(SocketShutdown.Both);
            //handler.Close();
            Debug.Log("Seed is: " + seed);
            connected = true;
            isServer = true;
            //LOAD SCENE
            Menu.LoadScene(1);
        } catch (Exception e) {
            Debug.Log("ERROR: " + e.ToString());
        }
    }

    public void JoinServer(string ip) {
        //replace 127.0.0.1 with an actual ip
        IPHostEntry host = Dns.GetHostEntry(ip);
        IPAddress ipAddress = host.AddressList[0];
        Debug.Log("IP Address: " + ipAddress.ToString());
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11234); //Running on port 11234

        sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try {
            sender.Connect(remoteEP);

            Debug.Log("Connected!");
            //TODO: fix this jank ass code
            seed = (uint) UnityEngine.Random.Range(0, UInt32.MaxValue);
            byte[] msg = Encoding.ASCII.GetBytes("Seed: " + seed.ToString());

            int bytesSent = sender.Send(msg);
            connected = true;
            isServer = false;
            //LOAD SCENE
            Menu.LoadScene(1);
        } catch (Exception e) {
            Debug.Log("ERROR: " + e.ToString());
        }
    }

    public void SendString(string message) {
        if(connected) {
            byte[] dataBytes = new byte[100];
            if (isServer) {
                handler.Send(Encoding.ASCII.GetBytes(message));
            } else {
                sender.Send(Encoding.ASCII.GetBytes(message));
            }
        } else {
            Debug.Log("ERROR: Trying to send message without being connected");
        }
    }

    public string ReceiveString() {
        if (connected) {
            byte[] dataBytes = new byte[100];
            string message;
            if (isServer) {
                int bytesRec = handler.Receive(dataBytes);
                message = Encoding.ASCII.GetString(dataBytes, 0, bytesRec);
            } else {
                int bytesRec = sender.Receive(dataBytes);
                message = Encoding.ASCII.GetString(dataBytes, 0, bytesRec);
            }
            return message;
        } else {
            Debug.Log("ERROR: Trying to send message without being connected");
            return "ERROR";
        }
    }

    public void CloseSockets() {
        if(connected) {
            if(isServer) {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            } else {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
        }
        connected = false;
    }

    public void ListenForKill() {
        while (connected) {
            string message = ReceiveString();
            Debug.Log("Message Received: " + message);
            if (message.Substring(0, 5) == "KILL:") {
                GameManager.Instance.RemoveTarget(uint.Parse(message.Substring(5)));
            } else if (message.Substring(0,5) == "BOOM:") {
                GameManager.Instance.BlowUpEverything();
            }
        }
    }
}
