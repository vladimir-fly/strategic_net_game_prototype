using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace SNGPClient
{
    public class App : MonoBehaviour
    {
        public Playground Playground;

        public string resp = "nothing";

        void Start()
        {
            try
            {
                var client = new TcpClient();
                client.Connect(IPAddress.Loopback, 8888);

                Debug.Log("connected.");

                byte[] data = new byte[256];
                StringBuilder response = new StringBuilder();
                NetworkStream stream = client.GetStream();

                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (stream.DataAvailable);

                resp = response.ToString();

                Debug.Log(response.ToString());

                // Закрываем потоки
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Debug.Log(string.Format("SocketException: {0}", e));
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Exception: {0}", e.Message));
            }

            Debug.Log("Запрос завершен...");
        }

        void OnGUI()
        {
            Debug.Log("ongui: " + resp);
            GUI.Label(new Rect(10, 10, 100, 20), resp);
        }

        void Update()
        {

        }


    }
}