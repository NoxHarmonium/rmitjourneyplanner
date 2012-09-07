
namespace JRPCServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.IO;
    using System.Threading;

    using Mono.WebServer.XSP;

    /// <summary>
    /// Main entry point of the program.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("RMIT Journey Planner Server");
            Console.WriteLine("Starting...");

            var server = new Server();
           
            var parameters = new [] {Directory.GetCurrentDirectory() + "\\..\\"};
            parameters[0] = parameters[0].Replace("\\", "/");
            System.Configuration.ConfigurationManager.AppSettings["MonoServerRootDir"] = parameters[0];
             
            server.RealMain(new string[] {"--verbose"}, true, null, false);



            Console.ReadLine();
        }

    }
}
