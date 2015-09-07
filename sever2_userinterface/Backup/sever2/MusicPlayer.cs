using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace sever2
{
    class MusicPlayer
    {
        
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand,
        string strReturn, int iReturnLength, IntPtr hwndCallback);


        public MusicPlayer()
        {
        }

        //播放器状态
        public string Status
        {
            get
            {
                string sCommand = "status MediaFile mode";
                string ret = "";
                mciSendString(sCommand, ret, 10, IntPtr.Zero);
                return ret.Trim();
            }
        }

        //打开音乐文件
        private void Open(string sFileName)
        {
            string sCommand = "open \"" + sFileName + "\" type mpegvideo alias MediaFile";
            //   MediaFile是选择播放文件类型 
            string ret = null;
            mciSendString(sCommand, ret, 0, IntPtr.Zero);
        }
        
        //播放音乐
        public void Play(string filename)
        {
            try
            {
                Open(filename);
                string sCommand = "play MediaFile";
                string ret = null;

                mciSendString(sCommand, ret, 0, IntPtr.Zero);
            }
            catch
            {
 
            }
        }
        
        //停止播放音乐
        public void Stop()
        {
            string sCommand = "close MediaFile";
            string ret = null;
            mciSendString(sCommand, ret, 0, IntPtr.Zero);
        }
        
        //暂停音乐
        public void Pause()
        {
            string sCommand = "pause MediaFile";
            string ret = null;
            mciSendString(sCommand, ret, 0, IntPtr.Zero);
        }

    }
    }
