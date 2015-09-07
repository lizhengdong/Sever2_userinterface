using System;
using System.Collections.Generic;
using System.Text;
//lzd
namespace sever2
{
    class EncryptIPAdd
    {
        //加密IP地址
        public string encrypt(String IPAdd)
        {

            //将IP地址拆分
            string[] lines = new string[4];
            string s = ".";


            lines = IPAdd.Split(s.ToCharArray(), 4);

            string IPNum1 = NumToStr(lines[0].ToString());//将IP地址的每个数字转化成字符串
            string IPNum2 = NumToStr(lines[1].ToString());
            string IPNum3 = NumToStr(lines[2].ToString());
            string IPNum4 = NumToStr(lines[3].ToString());
            //将转化后的字符串拼接，IP地址中的“.”用z来代替

            string encryptedIPAdd = IPNum1 + "z" + IPNum2 + "z" + IPNum3 + "z" + IPNum4;
            return encryptedIPAdd;

        }

        //解密IP地址
        public string decrypt(String encryptedIPAdd)
        {
            //将加密后的IP地址按照字母z分解
            string[] lines = new string[4];
            string z = "z";
            lines = encryptedIPAdd.Split(z.ToCharArray(), 4);
            string IPNum1 = Convert.ToString(StrToNum(lines[0]));//将每个字符串转化成IP地址
            string IPNum2 = Convert.ToString(StrToNum(lines[1]));

            string IPNum3 = Convert.ToString(StrToNum(lines[2]));
            string IPNum4 = Convert.ToString(StrToNum(lines[3]));
            //将ip地址的每一段数字和“.”进行拼接得到结果
            string decryptedIPAdd = IPNum1 + "." + IPNum2 + "." + IPNum3 + "." + IPNum4;
            return decryptedIPAdd;
        }
        public string NumToStr(String Num)
        {
            //数字转字符串
            int d = Convert.ToInt32(Num);
            int d1 = (d / 100) % 10;
            int d2 = (d / 10) % 10;
            int d3 = d % 10;
            char[] c = new char[3];
            c[0] = NumToChar(d1);
            c[1] = NumToChar(d2);
            c[2] = NumToChar(d3);
            string resultStr = new string(c);
            return resultStr;
        }
        public char NumToChar(int aNum)
        {
            char a = Convert.ToChar(aNum + 97);
            //数字转字符
            return a;
        }
        public int StrToNum(String aStr)
        {
            //字符串转数字
            char[] c = new char[3];
            c = aStr.ToCharArray();
            int[] d = new int[3];
            for (int i = 0; i < 3; i++)
            {
                d[i] = Convert.ToInt32(c[i]) - 97;
            }

            int aResultNum = d[0] * 100 + d[1] * 10 + d[2];
            return aResultNum;
        }
    }
}
