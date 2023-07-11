using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Mime;
using Newtonsoft.Json;


namespace HTTP_POST_SERVER
{
    public class user
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public user()
        {

        }

    }
    public class data_user_log
    {
        public string user { get; set; }
        public string password { get; set; }
        public data_user_log()
        {

        }
    }

    public class respons
    {
        public bool def { get; set; }
        public int id { get; set; }
        public respons() { }
    }


    internal class Server
    {
        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient)StateInfo);
        }

        public static void Main(string[] args)
        {
            new Server(80);
        }
        TcpListener Listener; // Объект, принимающий TCP-клиентов

        // Запуск сервера
        public Server(int Port)
        {
            // Создаем "слушателя" для указанного порта
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start(); // Запускаем его

            // В бесконечном цикле
            while (true)
            {
                // Принимаем нового клиента
                TcpClient Client = Listener.AcceptTcpClient();
                // Создаем поток
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(Client);
            }
        }
        ~Server()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }
    }
    class Client
    {
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client)
        {
            // Объявим строку, в которой будет хранится запрос клиента
            string Request = "";
            // Буфер для хранения принятых от клиента данных
            byte[] Buffer = new byte[1024];
            // Переменная для хранения количества байт, принятых от клиента
            int Count;
            // Читаем из потока клиента до тех пор, пока от него поступают данные
            while ((Count = Client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                // Преобразуем эти данные в строку и добавим ее к переменной Request
                Request += Encoding.ASCII.GetString(Buffer, 0, Count);
                // Запрос должен обрываться последовательностью \r\n\r\n
                // Либо обрываем прием данных сами, если длина строки Request превышает 4 килобайта
                // Нам не нужно получать данные из POST-запроса (и т. п.), а обычный запрос
                // по идее не должен быть больше 4 килобайт
                if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
                {
                    break;
                }
            }
            Match ReqMatch = Regex.Match(Request, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            Console.WriteLine(ReqMatch);

            if (ReqMatch == Match.Empty)
            {
                // Передаем клиенту ошибку 400 - неверный запрос
                SendError(Client, 400);
                return;
            }

            if (Request.Contains("POST"))
            {
                string RequestUri_ = ReqMatch.Groups[1].Value;
                if (RequestUri_ == "/new_user")
                {
                    try
                    {

                        StreamReader stream = new StreamReader("html/get_id.txt");
                        string lastid = stream.ReadLine();
                        stream.Dispose();
                        StreamWriter sw = new StreamWriter("html/get_id.txt", false);
                        sw.WriteLine(Convert.ToInt16(lastid) + 1);
                        sw.Dispose();
                        string json = Request.Split('\n')[Request.Split('\n').Length - 1];
                        StreamWriter sw2 = new StreamWriter("html/dataBase.txt", true);
                        sw2.WriteLine(json);
                        sw2.Dispose();
                        byte[] Req = Encoding.UTF8.GetBytes("sucsess");
                        string Headers2 = "HTTP/1.1 200 OK\nContent-Type: text" + "\nContent-Length: " + Req.Length + "\n\n";
                        byte[] BufHed = Encoding.UTF8.GetBytes(Headers2);
                        Client.GetStream().Write(BufHed, 0, BufHed.Length);
                        Client.GetStream().Write(Req, 0, Req.Length);
                        Client.Close();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return;
                    }
                }
                else if (RequestUri_ == "/Newusername")
                {

                    StreamReader streamReader = new StreamReader("html/dataBase.txt");
                    string all = streamReader.ReadToEnd();
                    streamReader.Close();
                    string[] alls = all.Split('\n');
                    List<user> users = new List<user>();
                    foreach (string el in alls)
                    {
                        if (el.Contains("{"))
                        {
                            users.Add(JsonConvert.DeserializeObject<user>(el));
                        }
                    }
                    string name = Request.Split('\n')[Request.Split('\n').Length - 1];

                    foreach (user user in users)
                    {
                        if (user.username == name)
                        {
                            byte[] Req_ = Encoding.UTF8.GetBytes("0");
                            string Headers2_ = "HTTP/1.1 200 OK\nContent-Type: text" + "\nContent-Length: " + Req_.Length + "\n\n";
                            byte[] BufHed_ = Encoding.UTF8.GetBytes(Headers2_);
                            Client.GetStream().Write(BufHed_, 0, BufHed_.Length);
                            Client.GetStream().Write(Req_, 0, Req_.Length);
                            Client.Close();
                            return;
                        }


                    }
                    byte[] Req = Encoding.UTF8.GetBytes("1");
                    string Headers2 = "HTTP/1.1 200 OK\nContent-Type: text" + "\nContent-Length: " + Req.Length + "\n\n";
                    byte[] BufHed = Encoding.UTF8.GetBytes(Headers2);
                    Client.GetStream().Write(BufHed, 0, BufHed.Length);
                    Client.GetStream().Write(Req, 0, Req.Length);
                    Client.Close();
                    return;
                }
                else if (RequestUri_ == "/user_log")
                {
                    StreamReader streamReader = new StreamReader("html/dataBase.txt");
                    string all = streamReader.ReadToEnd();
                    streamReader.Close();
                    string[] alls = all.Split('\n');
                    List<user> users = new List<user>();
                    foreach (string el in alls)
                    {
                        if (el.Contains("{"))
                        {
                            users.Add(JsonConvert.DeserializeObject<user>(el));
                        }
                    }
                    string data = Request.Split('\n')[Request.Split('\n').Length - 1];
                    data_user_log data_User = JsonConvert.DeserializeObject<data_user_log>(data);
                    foreach (user user in users)
                    {
                        if (user.username == data_User.user && user.password == data_User.password)
                        {
                            respons respons = new respons();
                            respons.def = true;
                            respons.id = user.id;
                            byte[] Req = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(respons));
                            string Headers2 = "HTTP/1.1 200 OK\nContent-Type: text" + "\nContent-Length: " + Req.Length + "\n\n";
                            byte[] BufHed = Encoding.UTF8.GetBytes(Headers2);
                            Client.GetStream().Write(BufHed, 0, BufHed.Length);
                            Client.GetStream().Write(Req, 0, Req.Length);
                            Client.Close();
                            return;
                        }

                    }
                    respons respons2 = new respons();
                    respons2.def = false;
                    respons2.id = -1;

                    byte[] Req_ = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(respons2));
                    string Headers2_ = "HTTP/1.1 200 OK\nContent-Type: text" + "\nContent-Length: " + Req_.Length + "\n\n";
                    byte[] BufHed_ = Encoding.UTF8.GetBytes(Headers2_);
                    Client.GetStream().Write(BufHed_, 0, BufHed_.Length);
                    Client.GetStream().Write(Req_, 0, Req_.Length);
                    Client.Close();
                    return;

                }
                else if (RequestUri_ == "/username")
                {
                    string id = Request.Split('\n')[Request.Split('\n').Length - 1];
                    StreamReader streamReader = new StreamReader("html/dataBase.txt");
                    string all = streamReader.ReadToEnd();
                    streamReader.Close();
                    string[] alls = all.Split('\n');
                    List<user> users = new List<user>();
                    foreach (string el in alls)
                    {
                        if (el.Contains("{"))
                        {
                            users.Add(JsonConvert.DeserializeObject<user>(el));
                        }
                    }
                    foreach (user user in users)
                    {
                        if (user.id == int.Parse(id))
                        {
                            string Headers2 = "HTTP/1.1 200 OK\nContent-Type: text\nContent-Length" + Encoding.UTF8.GetBytes(user.username).Length.ToString() + "\n\n";
                            byte[] b = Encoding.UTF8.GetBytes(user.username);
                            byte[] b2 = Encoding.UTF8.GetBytes(Headers2);
                            Client.GetStream().Write(b2, 0, b2.Length);
                            Client.GetStream().Write(b, 0, b.Length);
                            Client.Close();
                            return;
                        }
                    }
                }
                else if (RequestUri_ == "/createPost")
                {
                    StreamWriter stream = new StreamWriter("html/posts.txt", true);
                    string json = Request.Split('\n')[Request.Split('\n').Length - 1];
                    stream.WriteLine(json);
                    Client.Close();
                    stream.Close();
                    return;
                }


            }
            string RequestUri = ReqMatch.Groups[1].Value;
            Console.WriteLine(RequestUri);
            if (RequestUri.IndexOf("..") >= 0)
            {
                SendError(Client, 400);
                return;
            }
            if (RequestUri == "/")
            {
                RequestUri += "index.html";
            }
            string FilePath = "html/" + RequestUri;
            if (!File.Exists(FilePath))
            {
                if (!File.Exists(FilePath + ".html"))
                {
                    SendError(Client, 404);
                    return;
                }
                else
                {
                    FilePath += ".html";
                }
            }
            string ContentType = "";
            string Extension = "";
            try
            {
                Extension = RequestUri.Substring(RequestUri.LastIndexOf('.'));
                switch (Extension)
                {
                    case ".htm":
                    case ".html":
                        ContentType = "text/html";
                        break;
                    case ".css":
                        ContentType = "text/stylesheet";
                        break;
                    case ".js":
                        ContentType = "text/javascript";
                        break;
                    case ".jpg":
                        ContentType = "image/jpeg";
                        break;
                    case ".jpeg":
                    case ".png":
                    case ".gif":
                        ContentType = "image/" + Extension.Substring(1);
                        break;
                    default:
                        if (Extension.Length > 1)
                        {
                            ContentType = "application/" + Extension.Substring(1);
                        }
                        else
                        {
                            ContentType = "application/unknown";
                        }
                        break;
                }
            }
            catch
            {
                ContentType = "text/html";

            }

            StreamReader sr;
            try
            {
                sr = new StreamReader(FilePath);
            }
            catch (Exception)
            {
                // Если случилась ошибка, посылаем клиенту ошибку 500
                SendError(Client, 500);
                return;
            }
            if (ContentType.Contains("image/"))
            {
                sr.Close();
                FileStream fs;
                try
                {
                    fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch
                {
                    SendError(Client, 500);
                    return;
                }
                while (fs.Position < fs.Length)
                {
                    Count = fs.Read(Buffer, 0, Buffer.Length);
                    Client.GetStream().Write(Buffer, 0, Count);
                }
                fs.Close();
                Client.Close();
                return;

            }
            string content = sr.ReadToEnd();
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            string Headers = "HTTP/1.1 200 OK\nContent-Type: " + ContentType + "\nContent-Length: " + buffer.Length + "\n\n";
            byte[] HeadersBuffer = Encoding.UTF8.GetBytes(Headers);



            Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);
            Client.GetStream().Write(buffer, 0, buffer.Length);
            sr.Close();
            Client.Close();


        }
        private void SendError(TcpClient Client, int Code)
        {
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1
            string CodeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            // Код простой HTML-странички
            string Html = "<html><body><h1>" + CodeStr + "</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 " + CodeStr + "\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            Client.Close();
        }
    }
}
