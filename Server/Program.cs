using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Collections;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    class Program
    {
        private static string filename = String.Empty;
        
        static void Main(string[] args)
        {
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("                    RMIT Journey Planner                   ");
            Console.WriteLine("                          Server                           ");
            Console.WriteLine("-----------------------------------------------------------");
            
            if (args.Length == 1)
            {
                filename = args[0];
            }

            bool error = !RunServer(false);

            if (error)
            {
                Console.WriteLine("Exiting due to errors.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static bool RunServer(bool network)
        {
            
            //TODO: Move proxy into set command
            /*
             * UNCOMMENT FOR RMIT
            Console.WriteLine("Setting proxy settings...");
            ConnectionInfo.Proxy = new WebProxy(
                "http://aproxy.rmit.edu.au:8080", false, null, new NetworkCredential("s3229159", "MuchosRowlies1"));
            */ 

            Console.WriteLine("Loading CoreLibraries Assembly...");
            Assembly coreLibraries;
            try
            {
                try
                {
                    coreLibraries = Assembly.Load("RmitJourneyPlanner.CoreLibraries");
                }
                catch (Exception)
                {
                    coreLibraries = Assembly.LoadFile(Directory.GetCurrentDirectory() + "/RmitJourneyPlanner.CoreLibraries.dll");
                }
                
                
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error loading the CoreLibraries assembly. Please check if 'RmitJourneyPlanner.CoreLibraries.dll' is present in the program directory.\nError Message: {0}", e.Message);
                return false;
            }

            string attributes = coreLibraries.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false).Cast<AssemblyFileVersionAttribute>().Aggregate("", (current, attribute) => current + attribute.Version);

            Console.WriteLine("Version: {0}",String.Join("\n",attributes));
            Console.WriteLine("Hooking into logging interface...");
            
            Logger.LogEvent += LoggerOnLogEvent;
            Logger.ProgressEvent += new ProgressEventHandler(Logger_ProgressEvent);

            /*
            Type[] types = coreLibraries.GetTypes();

            
            Console.WriteLine("Loading point data providers:");
            Type[] pdpTypes = getTypesImplimenting("IPointDataProvider", types);
            List<IPointDataProvider> pointDataProviders = new List<IPointDataProvider>();
            foreach (Type type in pdpTypes)
            {
                Console.WriteLine("--> {0} ", type.Name);
                IPointDataProvider pdp = type.InvokeMember("", BindingFlags.CreateInstance, null, null, null, null) as IPointDataProvider;
                if (pdp == null)
                {
                    Console.WriteLine("Error initializing the data provider.");
                }
                else
                {
                    pointDataProviders.Add(pdp);
                }
            }
            Console.WriteLine("Loading network data providers:");
            Type[] ndpTypes = getTypesImplimenting("INetworkDataProvider", types);
            List<INetworkDataProvider> networkDataProviders = new List<INetworkDataProvider>();
            foreach (Type type in ndpTypes)
            {
                Console.WriteLine("--> {0} ", type.Name);
                INetworkDataProvider ndp = type.InvokeMember("", BindingFlags.CreateInstance, null, null, null, null) as INetworkDataProvider;
                if (ndp == null)
                {
                    Console.WriteLine("Error initializing the data provider.");
                }
                else
                {
                    networkDataProviders.Add(ndp);
                }

            }
            */
            var properties = new EvolutionaryProperties();
        
            EvolutionaryRoutePlanner planner = null;
            bool run = true;
            StreamWriter resultWriter = null;
            string resultPath = "";
            Queue<string> commands = new Queue<string>();
            var results = new List<Result>();
            var loopCommands = new Queue<string>();
            var loopStack = new Stack<KeyValuePair<Queue<string>, int>>();
            int resultIndex = 0;
            //bool loop = false;

            Socket socket = null;
            
            if (network)
            {
                Console.WriteLine("Listening for socket connection.");
                TcpListener listener = new TcpListener(IPAddress.Loopback,3000);
                listener.Start();
                socket = listener.AcceptSocket();
                
            }

            while (run)
            {
                Console.Write("> ");

                string input = String.Empty;
                
                if (network)
                {
                    Byte[] buffer = new byte[1024];
                    socket.Receive(buffer);
                    BinaryReader reader = new BinaryReader(new MemoryStream(buffer));
                    input = reader.ReadString();
                }
                else
                {
                    if (filename == String.Empty)
                    {
                        input = Console.ReadLine().Trim();
                    }
                    else
                    {
                        input = "run " + filename;

                    }
                    
                }



                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex(@"[ ]{2,}", options);

                input = regex.Replace(input, @" ");

                

                foreach (var value in input.Split(';'))
                {
                    commands.Enqueue(value.Replace(";","").Trim());
                }
                bool echo = false;
                while (commands.Count > 0)
                {
                    string command = commands.Dequeue();
                    if (echo == false)
                    {
                        echo = true;
                    }
                    else
                    {
                        Console.WriteLine(command.Trim());
                    }
                    if (command.Length == 0)
                    {
                        continue;
                    }
                    if (command.ToLower() == "quit" || command.ToLower() == "exit")
                    {
                        run = false;
                        return true;
                    }

                    try
                    {

                        if (command.Contains("randomNode()"))
                        {
                            
                        command = command.Replace(
                            "randomNode()",
                            ((MetlinkDataProvider)properties.NetworkDataProviders[0]).GetRandomNodeId().ToString(
                                CultureInfo.InvariantCulture));
                        }
                    string[] segments =  splitWithQuotes(command);
                        switch (segments[0].ToLower())
                        {
                            case "//":
                                break;

                            case "":
                                break;

                            case "help":
                                if (segments[1].ToLower() == "set")
                                {
                                    if (segments.Length == 2)
                                    {
                                        Console.WriteLine("   Help: set [parameter] [value]. - Sets a property.");
                                        Console.WriteLine(
                                            "   Here is a list of available properties. Type help set <property> for help on an individual property.");
                                        foreach (PropertyInfo p in typeof(EvolutionaryProperties).GetProperties())
                                        {
                                            if (p.CanWrite)
                                            {
                                                if (p.PropertyType.Name == "List`1")
                                                {
                                                    Type pType = p.PropertyType;

                                                    PropertyInfo pCount = pType.GetProperty("Count");
                                                    object o = p.GetValue(properties, null);
                                                    object count = pCount.GetValue(o, null);

                                                    Console.WriteLine(
                                                        "   {0} ({1}) = {2} ",
                                                        p.Name,
                                                        String.Format("List<{0}>", pType.Name),
                                                        count);
                                                }
                                                else
                                                {
                                                    string name = p.PropertyType.Name;
                                                    if (name.Contains("."))
                                                    {
                                                        name = name.Remove(
                                                            0, name.LastIndexOf(".", System.StringComparison.Ordinal));
                                                    }

                                                    Console.WriteLine(
                                                        "   {0} ({1}) = {2} ",
                                                        p.Name,
                                                        name,
                                                        p.GetValue(properties, null));
                                                }
                                            }
                                        }
                                    }
                                }
                                break;

                            case "loop":
                                string s;
                                int nestLevel = 0;
                                loopCommands.Clear();
                                while ((s = commands.Dequeue().ToLower()) != "end loop" || nestLevel > 0)
                                {
                                    string[] split = s.Split(' ');
                                    if (split.Length > 1 && split[0].Trim() == "loop")
                                    {
                                        nestLevel++;
                                    }
                                    if (s == "end loop")
                                    {
                                        nestLevel--;
                                    }
                                    loopCommands.Enqueue(s);
                                }
                                var loopedCommands = new Queue<string>();
                               
                                for (int i = 0; i < int.Parse(segments[1]); i ++ )
                                {
                                     loopedCommands.Enqueue("// Loop " + i );
                                    foreach (var lcommand in loopCommands)
                                    {
                                        loopedCommands.Enqueue(lcommand);
                                    }
                                    
                                }
                                foreach (var mcommand in commands)
                                {
                                    loopedCommands.Enqueue(mcommand);
                                }

                                commands = loopedCommands;
                                break;

                            

                            case "set":
                                if (segments[2].ToLower() == "new")
                                {
                                    ///Console.WriteLine("Creating new instance...");
                                    PropertyInfo info = typeof(EvolutionaryProperties).GetProperty(segments[1]);
                                    if (info == null)
                                    {
                                        throw new Exception("That property does not exist.");
                                    }

                                    string typeName = segments[3];
                                    Type type = searchForType(typeName, coreLibraries);
                                    object o = null;
                                    if (segments.Length == 4)
                                    {
                                        try
                                        {
                                            o = type.InvokeMember("", BindingFlags.CreateInstance, null, null, null);

                                        }
                                        catch( MissingMethodException )
                                        {
                                            o = type.InvokeMember(
                                             "", BindingFlags.CreateInstance, null, null, new object[] { properties });
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Log(typeof(Program), "Warning: Exception on instance creation: " + e.Message);
                                            if (e.InnerException != null)
                                            {
                                                Logger.Log(
                                                    typeof(Program),
                                                    "-------> Inner exception: " + e.InnerException.Message);

                                                if (e.InnerException.InnerException != null)
                                                {
                                                    Logger.Log(
                                                        typeof(Program),
                                                        "-------> ------->Inner inner exception: " + e.InnerException.InnerException.Message);
                                                }
                                            }
                                        }

                                       
                                    }
                                    else
                                    {
                                        int noParam = segments.Length - 4;
                                        bool created = false;
                                        foreach (var constructor in type.GetConstructors())
                                        {
                                            //int count = 0;

                                            ParameterInfo[] infos = constructor.GetParameters();
                                            if (infos.Length == noParam)
                                            {
                                                var p = new List<object>();
                                                
                                                for (int z = 4; z < segments.Length; z++)
                                                {
                                                    if (segments[z].ToLower() == "metlinkprovider")
                                                    {
                                                        p.Add(properties.NetworkDataProviders[0]);
                                                    }
                                                    else
                                                    {
                                                        p.Add(Convert.ChangeType(segments[z], infos[z - 4].ParameterType));
                                                        
                                                        
                                                    }
                                                }

                                                o = type.InvokeMember(
                                                    "", BindingFlags.CreateInstance, null, null, p.ToArray());
                                                created = true;
                                            }
                                        }
                                        if (!created)
                                        {
                                            throw new Exception("Error executing constructor / Unable to find a constuctor with those values.");
                                        }
                                    }

                                    //PropertyInfo info = typeof(EvolutionaryProperties).GetProperty(segments[1]);
                                    if (info.PropertyType.Name == "List`1")
                                    {
                                        Type listType = info.PropertyType;

                                        object list = info.GetValue(
                                            properties, BindingFlags.GetProperty, null, null, null);
                                        MethodInfo methodInfo = listType.GetMethod("Add");
                                        methodInfo.Invoke(
                                            list, BindingFlags.InvokeMethod, null, new object[] { o }, null);
                                    }
                                    else
                                    {
                                        typeof(EvolutionaryProperties).GetProperty(segments[1]).SetValue(
                                            properties, o, BindingFlags.SetProperty, null, null, null);
                                    }
                                }
                                else if (segments[1].ToLower() == "resultoutput")
                                {
                                    if (File.Exists(segments[2]))
                                    {
                                        Console.WriteLine("   Warning: File exists and will be overwritten.");
                                    }
                                    resultWriter = new StreamWriter(segments[2], false);
                                    resultPath = segments[2];
                                }
                                else
                                {
                                    string propertyName = segments[1];
                                    string value = segments[2];
                                  
                                    PropertyInfo info = typeof(EvolutionaryProperties).GetProperty(propertyName);
                                    if (info.PropertyType.IsValueType)
                                    {
                                        object o = Convert.ChangeType(value, info.PropertyType);
                                        typeof(EvolutionaryProperties).GetProperty(propertyName).SetValue(
                                            properties, o, BindingFlags.SetProperty, null, null, null);
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            "   Only value types (int, string) can be used with this property. Try the new keyword to create an instance.");
                                    }
                                }

                                break;

                            case "run":
                                try
                                {
                                    using (var reader = new StreamReader(segments[1]))
                                    {
                                        var newQueue = new Queue<string>();
                                        string script = reader.ReadToEnd();
                                        foreach (var c in script.Split(';'))
                                        {
                                            newQueue.Enqueue(c.Replace(";", "").Trim());
                                        }
                                        foreach (var c in commands)
                                        {
                                            newQueue.Enqueue(c);
                                        }

                                        commands = newQueue;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Error running script: {0}", e.Message);
                                }

                                break;

                            case "dump":
                                if (segments[1].ToLower() == "population")
                                {
                                    resultWriter.WriteLine("\n[Population Dump]");
                                    foreach (var critter in planner.Result.Population)
                                    {
                                        string line = critter.Route.Aggregate(
                                            "", (current, node) => current + (node.Id + ","));
                                        resultWriter.WriteLine(line);
                                    }
                                    resultWriter.WriteLine("[End Population Dump]");
                                }
                                if (segments[1].ToLower() == "averageresults")
                                {
                                    int n = Int32.Parse(segments[2]);
                                    
                                    for (int i = 0; i < n; i++)
                                    {
                                        Result average = default(Result);
                                        for (int j = 0; j < results.Count / n; j++)
                                        {
                                            average.AverageFitness += results[i + (j*n)].AverageFitness;
                                            average.DiversityMetric += results[i + (j*n)].DiversityMetric;
                                            average.MinimumFitness += results[i + (j*n)].MinimumFitness;
                                            average.Totaltime += results[i + (j*n)].Totaltime;

                                        }
                                        average.AverageFitness /= (results.Count / (double) n);
                                        average.DiversityMetric /= (results.Count / (double)n);
                                        average.MinimumFitness /= (results.Count / (double) n);
                                        average.Totaltime = new TimeSpan(average.Totaltime.Ticks / (results.Count / n));
                                        if (resultWriter != null)
                                        {
                                                resultWriter.WriteLine(average.ToString());
                                        }
                                    }
                                    results.Clear();
                                    

                                }
                                if (segments[1].ToLower() == "htmlmap")
                                {
                                    Tools.SavePopulation(planner.Population.GetRange(0,1),0,properties,resultPath+ ".html");
                                }

                                if (segments[1].ToLower() == "saveline")
                                {
                                    if (resultWriter.BaseStream.CanWrite)
                                    {
                                        resultWriter.Close();
                                    }
                                    StreamReader reader = new StreamReader(resultPath);
                                    string text = reader.ReadToEnd().Trim();
                                    reader.Close();
                                    string[] lines = text.Split('\n');
                                    
                                    
                                    try
                                    {
                                        if (resultIndex < lines.Length)
                                        {
                                            if (resultIndex < results.Count)
                                            {
                                                lines[resultIndex] = lines[resultIndex].Trim() + ","
                                                                     + results[resultIndex].AverageFitness;
                                            }
                                            else
                                            {
                                                lines[resultIndex] = lines[resultIndex].Trim() + ","
                                                                     + "-1";
                                            }
                                            resultWriter = new StreamWriter(resultPath, false);
                                            string st = String.Join("\n", lines);
                                            resultWriter.Write(st);
                                        }
                                        else
                                        {
                                            resultWriter = new StreamWriter(resultPath, true);
                                            if (resultIndex < results.Count)
                                            {
                                                resultWriter.WriteLine(results[resultIndex].AverageFitness);
                                            }
                                            else
                                            {
                                                resultWriter.WriteLine("-1");
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        
                                        Console.WriteLine("Error writing things...");
                                    }
                                    
                                }
                                

                                break;

                            case "start":
                                //try
                                try
                                {
                                    planner = new EvolutionaryRoutePlanner(properties);
                                    planner.Start();
                                    results.Clear();
                                    results.Add(planner.Result);
                                   
                                }
                                catch (Exception e)
                                {
                                    var r = new Result { AverageFitness = new Fitness() };
                                    results.Add(r);
                                    Console.WriteLine("   Error starting travel planner:\n   {0}\n   {1}", e.Message, e.StackTrace);
                                }
                                
                                resultIndex = 0;

                                /*
                                if (resultWriter != null)
                                {
                                    resultWriter.WriteLine(planner.Result.ToString());
                                }
                                 */
                                /*
                                catch (Exception e)
                                {
                                    Console.WriteLine("   Error starting travel planner:\n   {0}\n   {1}", e.Message,e.StackTrace);
                                }
                                */

                                break;

                            case "disable":
                                if (segments[1] == "log")
                                {
                                    Logger.LogEvent -= LoggerOnLogEvent;
                                }
                                break;

                            case "enable":
                                if (segments[1] == "log")
                                {
                                    Logger.LogEvent += LoggerOnLogEvent;
                                }

                                break;

                            case "step":
                                try
                                {
                                    int iterations = 1;
                                    if (segments.Length > 1)
                                    {
                                        int i;
                                        if (Int32.TryParse(
                                            segments[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out i))
                                        {
                                            iterations = i;
                                        }
                                    }

                                    for (int i = 0; i < iterations; i++)
                                    {
                                        planner.SolveStep();
                                        /*
                                        if (resultWriter != null)
                                        {
                                            resultWriter.WriteLine(planner.Result.ToString());
                                        }
                                         * */
                                        results.Add(planner.Result);
                                    }
                                    
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("   Error stepping travel planner:\n   {0}\n   {1}", e.Message, e.StackTrace);
                                    var r = new Result { AverageFitness = new Fitness()};
                                    results.Add(r);
                                }

                                resultIndex++;

                                break;
                            default:
                                throw new Exception("   Command not recognised.");
                        }
                    }
                    catch(TargetInvocationException e)
                    {
                        Console.WriteLine("   There was an error parsing your command.");
                        Console.WriteLine("      There was an error invoking the new instance.\n      {0}", e.InnerException.Message); 

                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine(
                            "   There was an error parsing your command. Type help for a list of commands.");
                        //commands.Clear();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(
                            "   There was an error parsing your command. \n {0} \nType help for a list of commands.",
                            e.Message);
                        if (e.InnerException != null)
                        {
                            Console.WriteLine("    Inner Exception Message: " + e.InnerException.Message);
                        }
                        //commands.Clear();

                    }
                        

                }

            }

            
            return true;
        }

        static string[] splitWithQuotes(string s)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w./-]+))(\s)*)", options);
            var result = (from Match m in regex.Matches(s)
                          where m.Groups["token"].Success
                          select m.Groups["token"].Value).ToList();
            return result.ToArray();


        }

        static void Logger_ProgressEvent(object sender, int progress)
        {
            Console.SetCursorPosition(0,Console.CursorTop);
            Console.Write("   [{0}]: --> {1} %", sender.GetType().Name, progress);
        }

        

        private static void LoggerOnLogEvent(object sender, string message)
        {
            Console.WriteLine("   [{0}]: --> {1}",sender.GetType().Name,message);
        }

        static Type searchForType(string typeName,Assembly a)
        {
            foreach (Type type in a.GetTypes())
            {
                if (type.Name.ToLower() ==typeName.ToLower())
                {
                    return type;
                }
            }
            throw new Exception(String.Format("Type not found: {0}", typeName));

        }

        static Type[] getTypesImplimenting(string interfaceName, Type[] types)
        {
            List<Type> typesImplimenting = new List<Type>();
            foreach (Type type in types)
            {
                bool impliments = false;
                Type[] interfaces = type.GetInterfaces();
                foreach (Type inf in interfaces)
                {
                    if (inf.Name == interfaceName)
                    {
                        impliments = true;
                        break;
                    }
                }
                if (impliments)
                {
                    typesImplimenting.Add(type);
                }
            }
            return typesImplimenting.ToArray();


        }
    }
}
