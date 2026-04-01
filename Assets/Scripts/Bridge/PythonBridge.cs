using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

[System.Serializable]
public class BridgeAction
{
    public int action; // Discrete(3)
}

[System.Serializable]
public class BridgeObservation
{
    public int infected_count;
    public int hospital_occupancy;
    public float remaining_budget;
    public int peak_infection;
    public float step_reward; // Can be passed back for debugging
    public bool done;
}

public class PythonBridge : MonoBehaviour
{
    public SimulationManager manager;
    private TcpListener listener;
    private Thread listenerThread;
    
    private TcpClient connectedClient;
    private NetworkStream stream;
    
    // Concurrency queues
    private Queue<int> pendingActions = new Queue<int>();

    private void Start()
    {
        if(manager == null) manager = FindAnyObjectByType<SimulationManager>();
        
        listenerThread = new Thread(StartServer);
        listenerThread.IsBackground = true;
        listenerThread.Start();
        
        manager.OnTick += HandleTick;
    }
    
    private void StartServer()
    {
        try
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9876);
            listener.Start();
            Debug.Log("Python Bridge listening on port 9876");

            while (true)
            {
// Only accept a connection if none is active
                if(connectedClient == null || !connectedClient.Connected)
                {
                    connectedClient = listener.AcceptTcpClient();
                    stream = connectedClient.GetStream();
                    Debug.Log("Python Agent Connected!");
                    
                    Thread clientThread = new Thread(ListenForActions);
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                Thread.Sleep(100);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log($"TcpListener error: {e}");
        }
    }
    
    private void ListenForActions()
    {
        byte[] buffer = new byte[1024];
        while (connectedClient != null && connectedClient.Connected)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    BridgeAction act = JsonUtility.FromJson<BridgeAction>(data);
                    
                    if(act != null)
                    {
                        lock(pendingActions)
                        {
                            pendingActions.Enqueue(act.action);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"Socket Closed: {e}");
                break;
            }
        }
    }
    
    private void HandleTick()
    {
        if (manager == null) return;
        
        // Execute pending actions from RL agent
        lock(pendingActions)
        {
            while(pendingActions.Count > 0)
            {
                int actionId = pendingActions.Dequeue();
                manager.ExecuteAction(actionId);
            }
        }
        
        // Send state back to RL
        if(connectedClient != null && stream != null && connectedClient.Connected)
        {
            BridgeObservation obs = new BridgeObservation
            {
                infected_count = manager.countI,
                hospital_occupancy = manager.currentHospitalOccupancy,
                remaining_budget = manager.remainingBudget,
                peak_infection = manager.peakInfectious,
                done = manager.stepCount > 1000 || manager.countI == 0 // Example termination
            };
            
            string json = JsonUtility.ToJson(obs);
            byte[] buf = Encoding.UTF8.GetBytes(json + "\n");
            try 
            {
                stream.Write(buf, 0, buf.Length);
            }
            catch(System.Exception) {}
        }
    }
    
    private void OnDestroy()
    {
        if(listener != null) listener.Stop();
        if(connectedClient != null) connectedClient.Close();
    }
}
