using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;
using System.Threading;

namespace TCP_client
{
    internal class Controller
    {
        Socket sock;


        public Action<string> reseive;

        public void CloseSock()
        {
            sock.Shutdown(SocketShutdown.Both); // Блокируем передачу и получение данных для объекта Socket.
            sock.Close(); // закрываем сокет

        }

        public void Connect()
        {
            // соединяемся с удаленным устройством
            try
            {
                IPAddress ipAddr = IPAddress.Loopback;//IPAddress.Parse(/*ip_address.Text*/Dns.GetHostName());
                                                      // textBox1.Text = ipAddr.ToString();
                                                      // устанавливаем удаленную конечную точку для сокета
                                                      // уникальный адрес для обслуживания TCP/IP определяется комбинацией IP-адреса хоста с номером порта обслуживания
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr /* IP-адрес */, 49152 /* порт */);

                // создаем потоковый сокет
                sock = new Socket(AddressFamily.InterNetwork /*схема адресации*/, SocketType.Stream /*тип сокета*/, ProtocolType.Tcp /*протокол*/);
                /* Значение InterNetwork указывает на то, что при подключении объекта Socket к конечной точке предполагается использование IPv4-адреса.
                  SocketType.Stream поддерживает надежные двусторонние байтовые потоки в режиме с установлением подключения, без дублирования данных и 
                  без сохранения границ данных. Объект Socket этого типа взаимодействует с одним узлом и требует предварительного установления подключения 
                  к удаленному узлу перед началом обмена данными. Тип Stream использует протокол Tcp и схему адресации AddressFamily.
                */

                // соединяем сокет с удаленной конечной точкой
                sock.Connect(ipEndPoint);
                byte[] msg = Encoding.Default.GetBytes(Dns.GetHostName() /* имя узла локального компьютера */);// конвертируем строку, содержащую имя хоста, в массив байтов
                int bytesSent = sock.Send(msg); // отправляем серверу сообщение через сокет
                MessageBox.Show("Клиент " + Dns.GetHostName() + " установил соединение с " + sock.RemoteEndPoint.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент: " + ex.Message);
            }
        }



        public void Send(object mas)
        {
            try
            {
                string theMessage = mas.ToString(); // получим текст сообщения, введенный в текстовое поле
                byte[] msg = Encoding.Default.GetBytes(theMessage); // конвертируем строку, содержащую сообщение, в массив байтов
                Thread.Sleep(10000);
                int bytesSent = sock.Send(msg); // отправляем серверу сообщение через сокет
                
                if (theMessage.IndexOf("<end>") > -1) // если клиент отправил эту команду, то принимаем сообщение от сервера
                {
                    byte[] bytes = new byte[1024];
                    int bytesRec = sock.Receive(bytes); // принимаем данные, переданные сервером. Если данных нет, поток блокируется
                    MessageBox.Show("Сервер (" + sock.RemoteEndPoint.ToString() + ") ответил: " + Encoding.Default.GetString(bytes, 0, bytesRec) /*конвертируем массив байтов в строку*/);
                    sock.Shutdown(SocketShutdown.Both); // Блокируем передачу и получение данных для объекта Socket.
                    sock.Close(); // закрываем сокет
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент: " + ex.Message);
            }
        }


        public void Recieve()
        {
            try
            {

                string client = null;
                string data = null;
                byte[] bytes = new byte[1024];
                // Получим от клиента DNS-имя хоста.
                // Метод Receive получает данные от сокета и заполняет массив байтов, переданный в качестве аргумента
                //int bytesRec = sock.Receive(bytes); // Возвращает фактически считанное число байтов
                //client = Encoding.Default.GetString(bytes, 0, bytesRec); // конвертируем массив байтов в строку
                ///client += "(" + handler.RemoteEndPoint.ToString() + ")";////Возвращает удаленную конечную точку.
                //sock.ReceiveTimeout=5000;
                int bytesRec = sock.Receive(bytes); // принимаем данные, переданные клиентом. Если данных нет, поток блокируется
                if (bytesRec == 0)
                {
                    sock.Shutdown(SocketShutdown.Both); // Блокируем передачу и получение данных для объекта Socket.
                    sock.Close(); // закрываем сокет
                    //return "Данніх нет";
                }


                data = Encoding.Default.GetString(bytes, 0, bytesRec); // конвертируем массив байтов в строку     

                reseive(data);///  event to form
                //return data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент: " + ex.Message);
                //return null;
            }
        }

    }
}
