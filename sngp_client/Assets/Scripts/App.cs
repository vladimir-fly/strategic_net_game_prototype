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
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Loopback, 8888);

                    Debug.Log("connected.");

                    var data = new byte[256];
                    var response = new StringBuilder();
                    using (var stream = client.GetStream())
                    {
                        while (stream.DataAvailable)
                        {
                            var bytes = stream.Read(data, 0, data.Length);
                            response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        }

                        resp = response.ToString();
                        Debug.Log(response.ToString());
                    }
                }
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
            GUI.Label(new Rect(10, 10, 100, 20), resp);
        }

        void Update()
        {

        }
    }
}