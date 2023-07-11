using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpClient
{
	public delegate void d_Receive(string message, int length);
	public event d_Receive Receive;
	private Socket socket_send;
	private Socket socket_receive;
	private string IP;
	private int Port;
	private Thread thread_connect;
	private Thread thread_receive;

	public TcpClient(string ip, int port, AddressFamily family = AddressFamily.InterNetwork, SocketType type = SocketType.Stream, ProtocolType protocol = ProtocolType.Tcp)
	{
		IP = ip;
		Port = port;
		socket_send = new Socket(family, type, protocol);
		socket_receive = new Socket(family, type, protocol);
	}

	public void StartConnect()
	{
		thread_connect = new Thread(Thread_Connect);
		thread_connect.Start();
	}

	public void StopConnect()
	{
		socket_send.Close();
		socket_receive.Close();
	}

	public bool IsConnected
    {
        get
        {
            if (socket_send == null) return false;
            else return socket_send.Connected;
        }
    }

	public void Send(string msg)
	{
		if (IsConnected) socket_send.Send(Encoding.UTF8.GetBytes(msg));
	}

	private void Thread_Connect()
	{
		try
		{
            socket_send.Connect(IPAddress.Parse(IP), Port + 1);
			socket_receive.Connect(IPAddress.Parse(IP), Port);

            Debug.Log("Connected!");

            thread_receive = new Thread(Thread_Receive);
			thread_receive.Start();
		}
		catch (Exception) { }
	}

	private void Thread_Receive()
	{
		while (true)
		{
			try
			{
                byte[] tmp = new byte[2048];
                int length = socket_receive.Receive(tmp);

                string msg = Encoding.UTF8.GetString(tmp);
                msg.Remove(length);

                Receive(msg, length);
            }
			catch (Exception)
			{
				break;
			}
		}
	}
}