using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MedianService
{
    class ServerClient
    {
        private TcpClient tcpClient;

        private NetworkStream stream;

        private static List<int> numbers = new List<int>(2018);

        private static int count = 0;

        public void connect()
        {
            try
            {
                tcpClient = new TcpClient();

                tcpClient.Connect("88.212.241.115", 2012);

                stream = tcpClient.GetStream();

                tcpClient.Client.ReceiveTimeout = 300;
            }
            catch (Exception)
            {
                connectException();
            }
        }

        private void connectException()
        {
            try
            {
                Thread.Sleep(2000);

                tcpClient.Close();

                tcpClient.Connect("88.212.241.115", 2012);

                stream = tcpClient.GetStream();

                tcpClient.Client.ReceiveTimeout = 300;
            }
            catch(Exception)
            {
                connectException();
            }

        }

        public void getNumbers(string index)
        {
            try
            {
                byte[] dataWrite = System.Text.Encoding.GetEncoding("KOI8-R").GetBytes(index);

                stream.Write(dataWrite, 0, dataWrite.Length);

                byte[] dataRead = new byte[5000];

                StringBuilder result = new StringBuilder();

                Regex regex = new Regex(@"[\w\|\W]*\n");

                MatchCollection matches;

                do
                {
                    int bytes = stream.Read(dataRead, 0, dataRead.Length);

                    result.Append(Encoding.GetEncoding("KOI8-R").GetString(dataRead, 0, bytes));

                    matches = regex.Matches(result.ToString());

                    Thread.Sleep(2000);

                } while (matches.Count == 0);

                Console.WriteLine(index + matches[0]);

                numbers.Add(Convert.ToInt32(Regex.Match(matches[0].ToString(), @"\d+").Value));

                stream.Close();

                tcpClient.Close();

                count++;

                if (count == 2018)
                    getMedian();
            }
            catch (Exception)
            {
                connectException();

                getNumbers(index);
            }
        }

        private void getMedian()
        {
            numbers.Sort();

            var middle = numbers.Count / 2;
          
            Console.WriteLine((numbers[middle] + numbers[middle - 1]) * 0.5);

            Program.taskQueue.Dispose();

            Console.ReadKey();
        }
    }
}
