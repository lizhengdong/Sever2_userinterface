using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace sever2
{
    class FileControl
    {
          /// <summary> 
         /// 删除指定目录的所有文件和子目录 
         /// </summary> 
         /// <param name="targetDir">操作目录</param> 
         /// <param name="delSubDir">如果为true,包含对子目录的操作</param> 


         public static void DeleteFiles(string targetDir,bool delSubDir)   



         { 
              foreach(string fileName in Directory.GetFiles(targetDir)) 
              {    

                   File.SetAttributes(fileName,FileAttributes.Normal); 
                   File.Delete(fileName); 
              }             

              if(delSubDir) 
              { 
                   DirectoryInfo dir = new DirectoryInfo(targetDir);
                   foreach(DirectoryInfo subDi in  dir.GetDirectories()) 

                   { 
                       DeleteFiles(subDi.FullName,true); 
                       subDi.Delete(); 
                   }             
              }
         } 

       }

   
}

 