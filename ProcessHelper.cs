using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaStarter
{
    public class ProcessHelper
    {

        public static Process CreateProcess(string command, DataReceivedEventHandler handler, string path = "")
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = false;//不显示程序窗口
                p.OutputDataReceived += handler;
                p.Start();//启动程序


                if (!string.IsNullOrEmpty(path))
                {
                    p.StandardInput.WriteLine(@"cd \");
                    string root = path.Substring(0, 2);
                    p.StandardInput.WriteLine($"{root}");
                    p.StandardInput.WriteLine($"cd {path} &");
                }

                string strCMD = command;
                //向cmd窗口发送输入信息
                p.StandardInput.WriteLine(strCMD);

                p.StandardInput.AutoFlush = true;

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                //等待程序执行完退出进程
                return p;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static Process CreateProcess(string path, DataReceivedEventHandler handler)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = false;//不显示程序窗口
                p.OutputDataReceived += handler;
                p.Start();//启动程序

                if (!string.IsNullOrEmpty(path))
                {
                    p.StandardInput.WriteLine(@"cd \");
                    string root = path.Substring(0, 2);
                    p.StandardInput.WriteLine($"{root}");
                    p.StandardInput.WriteLine($"cd {path} &");
                }

                p.StandardInput.AutoFlush = true;

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                //等待程序执行完退出进程
                return p;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
