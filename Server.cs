using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using TMPro;

public class Server : MonoBehaviour
{
    [SerializeField] RobotController RobotController;
    [SerializeField] DetectionTest TagDetectManager;
    [SerializeField] TextMeshProUGUI CommandInfo;

    public int TagID = -1;

    public float _Linear, _Angular, QX, QY, QZ, QW;

    public _Quaternion quat;
    
    private ServerThread st;
    public ServerJson _serverJson;
    private bool isSend;//儲存是否發送訊息完畢

    public struct ServerJson
    {
        public int ID;
        public float Linear;
        public float Angular;

        public ServerJson(int id, float linear, float angular)
        {
            ID = id;
            Linear = linear;
            Angular = angular;
        }
    }

    public struct _Quaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public _Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = x;
        }
    }

    private void Start()
    {
        //開始連線，設定使用網路、串流、TCP
        st = new ServerThread(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, "192.168.46.216", 8000);
        st.Listen();//讓Server socket開始監聽連線
        st.StartConnect();//開啟Server socket
        isSend = true;
    }

    private void Update()
    {

        if (st.receiveMessage != null)
        {
            Debug.Log("Client:" + st.receiveMessage);
            st.receiveMessage = null;
        }
        if (isSend == true)
            StartCoroutine(delaySend());//延遲發送訊息

        _serverJson = new ServerJson(TagID, _Linear, _Angular);
        quat = new _Quaternion(QX, QY, QZ, QW);

        st.Receive();
    }

    private IEnumerator delaySend()
    {
        isSend = false;
        yield return new WaitForSeconds(0.1f);//延遲1秒後才發送

        TagID = TagDetectManager.Tags;
        _Linear =  RobotController._joystick.Vertical;
        _Angular = RobotController._joystick.Horizontal;

        var SJsn = JsonUtility.ToJson(_serverJson);
        var QJsn = JsonUtility.ToJson(quat);

        QX = TagDetectManager.TagRotation.x;
        QY = TagDetectManager.TagRotation.y;
        QZ = TagDetectManager.TagRotation.z;
        QW = TagDetectManager.TagRotation.w;

        st.Send(SJsn);
        st.Send(QJsn);

        Debug.Log(SJsn + "////, " + QJsn);

        CommandInfo.text = "Tag ID: " + TagDetectManager.Tags + "\nTagRotation: " + TagDetectManager.TagRotation + "\nLinear: " + _Linear + "\nAngular: " + _Angular;
        isSend = true;
    }

    // public void QuaternionConverter()
    // {
    //     [A,-D,C,B,
    //     D,A,-B,C,
    //     -C,B,A,-D,
    //     -B,-C,-B,A]
    // }

    private void OnApplicationQuit()//應用程式結束時自動關閉連線
    {
        st.StopConnect();
    }
}