using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace sever2
{
    class TransferFiles
    {
        public static int SendData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }

            return total;
        }

        public static byte[] ReceiveData(Socket s, int size, DateTime now)
        {
            int total = 0;
            int dataleft = size;
            byte[] data = new byte[size];
            int recv;
            //设置超时为１分钟
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 60 * 1000);
            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, SocketFlags.None);
                //Phone.now = DateTime.Now;
                if (recv == 0)
                {
                    data = null;
                    break;
                }

                total += recv;
                dataleft -= recv;
            }
            recv = s.Receive(data, total, dataleft, SocketFlags.None);

            return data;
        }

        public static int SendVarData(Socket s, byte[] data)
        {
            //if (s == null) return 0;
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;
            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);

            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }

            return total;
        }

        public static byte[] ReceiveVarData(Socket s)
        {
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];
            //设置超时为１分钟
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 60 * 1000);
            recv = s.Receive(datasize, 0, 4, SocketFlags.None);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];
            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, SocketFlags.None);
                //Phone.now = DateTime.Now;
                if (recv == 0)
                {
                    data = null;
                    break;
                }
                total += recv;
                dataleft -= recv;
            }
            return data;
        }
    }
}
