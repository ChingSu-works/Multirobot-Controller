using System;
using UnityEngine;
using TMPro;

public class ClientMain : MonoBehaviour
{
	
	[SerializeField] RobotController RobotController;
    [SerializeField] DetectionTest TagDetectManager;
    [SerializeField] TextMeshProUGUI CommandInfo;
	int TagID = -1;
	float _Linear, _Angular, QX, QY, QZ, QW;
    public _Quaternion quat; 
	public TcpClient client; ClientJson _ClientJson;
	private bool isRece = true; //判斷是否已經收到訊息，避免重複發送導致資料損壞

	public struct ClientJson
    {
        public int ID;
        public float Linear,Angular;

        public ClientJson(int id, float linear, float angular)
        {
            ID = id;
            Linear = linear;
            Angular = angular;
        }
    }

	public struct _Quaternion
    {
        public float X,Y,Z,W;
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
		client = new TcpClient("192.168.46.215", 5000);
		client.Receive += ReceiveMessage; //當收到訊息時，自動呼叫指定的方法(該方法將由另一個執行緒自動呼叫，因此請勿在該方法內使用Unity的物件)
		client.StartConnect();
	}

	private void Update()
	{
		CMDSync();

		_ClientJson = new ClientJson(TagID, _Linear, _Angular);

		var client2Jsn = JsonUtility.ToJson( _ClientJson );

		if (client.IsConnected && isRece)
		{
			client.Send(client2Jsn);
			isRece = false;
		}
	}

	public void CMDSync()
	{
		TagID = TagDetectManager.Tags;
		_Linear = RobotController._joystick.Vertical;
		_Angular = RobotController._joystick.Horizontal;
	}

	private void ReceiveMessage(string msg, int length)
	{
        Debug.Log("收到的訊息長度：" + length + ", 訊息內容：" + msg);
        isRece = true;
	}

	private void OnApplicationQuit()
	{
		client.Receive -= ReceiveMessage;
		client.StopConnect();
	}
}