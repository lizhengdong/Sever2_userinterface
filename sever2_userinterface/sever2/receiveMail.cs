//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Net.Sockets;
//using System.Net;
//using System.Security.Cryptography;
//using System.IO;
//using jmail;
//namespace sever2
//{
//    class receiveMail
//    {
//        //接收邮件
//        public string ReceiveByJmail(String MailAdd, String Passwd)
//        {
//            string[] mailInfo = MailAdd.Split(new char[] { '@' });
//            //string mailName = MailAdd.Trim();
//            string mailName = mailInfo[0].Trim();
//            string mailPasswd = Passwd.Trim(); //密码
//            string mailServer = mailInfo[1].Trim();
//            //在sina.com前面加上pop3.
//            mailServer = "pop3." + mailServer;
//            //建立接收邮件对象
//            jmail.POP3Class popMail = new POP3Class();

//            //建立邮件信息接口
//            jmail.Message mailMessage;

//            string mailHeader = "服务器未发送IP改变的信息";
//            try
//            {
//                popMail.Connect(mailName, mailPasswd, mailServer, 110);

//                //如果收到邮件
//                try
//                {
//                    if (popMail.Count > 0)
//                    {
//                        try
//                        {

//                            //根据取得到的数量一次去的每封邮件
//                            for (int i = 1; i <= popMail.Count; i++)
//                            {
//                                //取得每一条邮件信息
//                                mailMessage = popMail.Messages[i];
//                                //设置邮件的编码方式
//                                mailMessage.Charset = "GB2312";
//                                //是否将信头编码成ISO-8859-1字符集
//                                mailMessage.ISOEncodeHeaders = false;
//                                try
//                                {
//                                    string mailHead = null;
//                                    mailHead = mailMessage.Subject;
//                                    //邮件主题，这里存放IP地址
//                                    try
//                                    {
//                                        if (IsIP(mailHead))
//                                        {
//                                            mailHeader = mailHead;
//                                        }
//                                    }
//                                    catch (Exception e)
//                                    {
//                                        mailHeader = e.Message.ToString();
//                                        mailHeader = "测IP" + mailHeader;
//                                        return mailHeader;
//                                    }
//                                }
//                                catch (Exception e)
//                                {
//                                    mailHeader = e.Message.ToString();
//                                    mailHeader = "收主题" + mailHeader;
//                                    return mailHeader;
//                                }

//                            }

//                            /*
//                            //将第一个邮件标题赋值给变量
//                            //取得每一条邮件信息
//                            mailMessage = popMail.Messages[1];
//                            //设置邮件的编码方式
//                            mailMessage.Charset = "GB2312";
//                            //是否将信头编码成ISO-8859-1字符集
//                            mailMessage.ISOEncodeHeaders = false;
//                            mailHeader = mailMessage.Subject;
//                            */
//                        }
//                        catch (Exception e)
//                        {
//                            mailHeader = e.Message.ToString();
//                            mailHeader = "for那" + mailHeader;
//                            return mailHeader;
//                        }
//                    }
//                    else
//                    {
//                        //没有新邮件
//                        mailHeader = "IP没改变";
//                        return mailHeader;
//                    }
//                }
//                catch (Exception e)
//                {
//                    mailHeader = e.Message.ToString();
//                    //mailHeader = "读取邮件出错";
//                    return mailHeader;
//                }
//            }
//            catch
//            {
//                //连接不上邮箱
//                mailHeader = "连不上邮箱";
//                return mailHeader;
//            }
//            //popMail.DeleteMessages();
//            popMail.Disconnect();
//            return mailHeader;
//        }

//        public bool IsIP(string ip)
//        {
//            bool b = true;
//            string[] lines = new string[4];
//            string s = ".";


//            lines = ip.Split(s.ToCharArray(), 4);//分隔字符串 

//            for (int i = 0; i < 4; i++)
//            {
//                try
//                {
//                    //int ipNum = Convert.ToInt32(lines[i]);
//                    int ipNum = -1;
//                    if (int.TryParse(lines[i], out ipNum) == false)
//                    {
//                        //判断是否可以转换为整型
//                        b = false;
//                        return b;
//                    }
//                    if (ipNum >= 255 || ipNum < 0)
//                    {
//                        b = false;
//                        return b;
//                    }
//                }
//                catch
//                {
//                    //如果转换出错，说明不是数字
//                    b = false;
//                    return b;
//                }

//            }
//            return b;
//        }
//    }
//}
