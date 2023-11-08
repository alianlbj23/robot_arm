using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Unity.Robotics.UrdfImporter.Control;

public class robotControl : MonoBehaviour
{
    // Start is called before the first frame update

    string topicName_subscribe = "/unity2Ros"; 
    
    private WebSocket socket;
    private string rosbridgeServerUrl = "ws://localhost:9090";

    public Transform base_link;
    public Transform link1;
    public Transform link2;
    public Transform link3;

    public Transform grap_left_1;
    public Transform grap_left_2;

    public Transform grap_right_1;
    public Transform grap_right_2;
    float grapAngle=0;
    [System.Serializable]
    public class RobotNewsMessage
    {
        public string op;
        public string topic;
        public MessageData msg;
    }

    [System.Serializable]
    public class MessageData
    {
        public LayoutData layout;
        public float[] data;
    }
    [System.Serializable]
    public class LayoutData
    {
    public int[] dim;
    public int data_offset;
    }
    void Start()
    {        
        socket = new WebSocket(rosbridgeServerUrl);
        socket.OnOpen += (sender, e) =>
        {
            SubscribeToTopic(topicName_subscribe);
        };
        socket.OnMessage += OnWebSocketMessage;
        socket.Connect();
        
    }
    private void SubscribeToTopic(string topic)
    {
        string subscribeMessage = "{\"op\":\"subscribe\",\"id\":\"1\",\"topic\":\"" + topic + "\",\"type\":\"std_msgs/msg/Float32MultiArray\"}";
        // string subscribeMessage = "{\"op\":\"subscribe\",\"id\":\"1\",\"topic\":\"" + topic + "\",\"type\":\"std_msgs/msg/String\"}";
        socket.Send(subscribeMessage);
    }
    float[] data;
    private void OnWebSocketMessage(object sender, MessageEventArgs e){
        string jsonString = e.Data;
        RobotNewsMessage message = JsonUtility.FromJson<RobotNewsMessage>(jsonString);
        data = message.msg.data;
        
        
        // link1.localRotation = Quaternion.Euler(0, 0, 45); 
        // link2.localRotation = Quaternion.Euler(0, 0, 45); 
        // link3.localRotation = Quaternion.Euler(0, 0, 90); 
        // grapAngle = 70;
        // grap(grapAngle, -grapAngle);

    }

    
    

    // Update is called once per frame
    void Update()
    {
        Debug.Log(data[0]);
        base_link.localRotation = Quaternion.Euler(0, data[0], 0); 
        link1.localRotation = Quaternion.Euler(0, 0, data[1]); 
        link2.localRotation = Quaternion.Euler(0, 0, data[2]); 
        link3.localRotation = Quaternion.Euler(0, data[3], 0); 
        grapAngle = data[4];
        grap(grapAngle, -grapAngle);
    }

    void grap(float left, float right){
        grap_left_1.localRotation = Quaternion.Euler(0, 0, left);
        grap_left_2.localRotation = Quaternion.Euler(0, 0, left);

        grap_right_1.localRotation = Quaternion.Euler(0, 0, right);
        grap_right_2.localRotation = Quaternion.Euler(0, 0, right);
    }

    
}
