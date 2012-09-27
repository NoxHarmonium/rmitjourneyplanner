﻿
namespace JRPCServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.IO;
    using System.Threading;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using RmitJourneyPlanner.CoreLibraries.Types;

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

            if (args.Length == 0)
            {

                var server = new Server();

                var parameters = new[] { Directory.GetCurrentDirectory() + "\\..\\" };
                parameters[0] = parameters[0].Replace("\\", "/");
                System.Configuration.ConfigurationManager.AppSettings["MonoServerRootDir"] = parameters[0];
                //System.Configuration.ConfigurationManager.AppSettings["MonoServerPort"] = new Random().Next(8080,8280).ToString();
				//System.Diagnostics.Process p = System.Diagnostics.Process.Start("./test.sh");// + String.Join(" ", new string[] { "--verbose", "--root", parameters[0] }));
				//p.WaitForExit();
                server.RealMain(new string[] { "--verbose", "--port", new Random().Next(8080, 8280).ToString() }, true, null, false);

                
            }
            else
            if (args[0] == "--collate")
            {
                //S:\Recovery\1 NTFS\Users\Sean\Dropbox\Study\RMITAssignments\Honours\JRPCServer\JSONStore\Journeys
                var journeys = Directory.GetDirectories(args[1]);
                foreach (var journey in journeys)
                {
                    var runs = Directory.GetDirectories(journey);


                    double[,] jvalues = new double[200, 15];
                    IEnumerable<object> names = null;

                    int runCount = 0;
                    bool success = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Processing journey: {0}", journey.Split(new[] { '\\' }).Last());


                    foreach (var run in runs)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\tProcessing run: {0}", run.Split(new []{'\\'}).Last());
                        runCount++;
                        
                        double[,] values = new double[200, 15];
                        for (int i = 0; i < 200; i++)
                        {
                            if (!File.Exists(run + "\\iteration." + i + ".json"))
                            {
                                Console.WriteLine("\tWarning: Iteration doesnt exist. Skipping run.");
                                success = false;
                                runCount--;
                                break;
                                
                            }
                            else
                            {
                                
                                JsonObject iteration =
                                JsonConvert.Import(
                                    File.ReadAllText(run + "\\iteration." + i + ".json"))
                                as JsonObject;

                                values[i, 0] = i;
                                values[i, 1] = Convert.ToDouble(iteration["hypervolume"]);
                                values[i, 2] = Convert.ToDouble(iteration["cardinality"]);
                                var f = new Fitness[100];

                                var pop = (JsonArray)iteration["population"];
                                int ps = -1;

                                for (int j = 0; j < 100; j++)
                                {

                                    var p = (JsonObject)((JsonObject)((JsonObject)pop[j])["Critter"])["Fitness"];
                                    ps = p.Count;
                                    for (int k = 0; k < p.Count; k++)
                                    {

                                        var name = p.Names.OfType<object>().ElementAt(k).ToString();
                                        names = p.Names.OfType<object>();
                                        var value = (p[name]);
                                        checked
                                        {
                                            values[i, k + 3] += Convert.ToDouble(value);
                                        }
                                    }

                                }

                                for (int j = 0; j < ps; j++)
                                {
                                    values[i, j + 3] /= 100.0;
                                }
                            }
                            

                        }

                        if (success)
                        {
                            for (int i = 0; i < jvalues.GetLength(0); i++)
                            {
                                for (int j = 0; j < jvalues.GetLength(1); j++)
                                {
                                    jvalues[i, j] += values[i, j];
                                }

                            }
                        }



                    }

                    Console.ForegroundColor = ConsoleColor.Green;

                    using (var writer = new StreamWriter(journey + "\\result2.csv"))
                    {
                        writer.Write("iteration,hypervolume,cardinality,");
                        foreach (var name in names)
                        {
                            writer.Write(name + ",");
                        }
                        writer.WriteLine();
                        if (runCount == 0)
                        {
                            Console.WriteLine("No intact runs for this journey!");
                            File.WriteAllText(journey + "\\fail.fail","FAIL");
                            continue;
                            

                        }
                        
                        for (int i = 0; i < jvalues.GetLength(0); i++)
                        {
                            for (int j = 0; j < jvalues.GetLength(1); j++)
                            {
                                jvalues[i, j] /= (double)runCount;
                                writer.Write(jvalues[i,j] + ", ");
                            }
                            writer.WriteLine();

                        }

                        Console.WriteLine("Wrote {0} runs!", runCount);
                    }

                }

            }
            else
            {
                Console.WriteLine("Invalid Param");
            }


            Console.ReadLine();
        }

    }
}
