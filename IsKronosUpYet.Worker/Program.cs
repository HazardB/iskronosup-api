using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace IsKronosUpYet.Worker
{
    public class Program
    {
        // Configuration variables:
        private static string _customAuthorisationHeader = "x-worker-auth";
        private static string _customAuthorisationSecret = "!CHANGE-THIS!";
        private static string baseApiUri = "http://localhost:59946";

  
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static List<Server> cachedServers;
        
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }

        static async void MainAsync(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();

            log.Info("-> Starting IsKronosUpYetWorker...");
            var servers = await RetrieveServers();

            log.Info($"--> Received {servers.Count()} servers.");
            foreach (var server in servers)
            {
                log.Info("---> Server: " + server);
            }

            cachedServers = servers;
            log.Info("-> Starting Observable loop for discovered servers.");

            Observable.Interval(TimeSpan.FromSeconds(15))
                .Select(_ => Unit.Default)
                .StartWith(Unit.Default)
                .ObserveOn(new NewThreadScheduler())
                .Subscribe(async _ => await CheckServers());

            Console.ReadKey();
        }
        
        private static async Task CheckServers()
        {
            foreach (var server in cachedServers)
            {
                log.Debug($"Checking {server.Name} - ({server.IP}:{server.Port})...");
                if (CheckServer(server.IP, server.Port, 2000))
                {
                    await SaveUpdate(new ServerStatusUpdate() { id = server.Id, status = true }, server.Name);
                    continue;
                }

                await SaveUpdate(new ServerStatusUpdate() { id = server.Id, status = false }, server.Name);
            }
        }

        private static async Task<List<Server>> RetrieveServers()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseApiUri);
                client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = await client.GetAsync(baseApiUri + "/api/servers");
                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        var servers = JsonConvert.DeserializeObject<List<Server>>(responseJson);
                        return servers;
                    }
                    else
                    {
                        log.Error("Encountered failure status code from API: " + response.StatusCode);
                        log.Error("Full response: " + response);
                        return new List<Server>();
                    }
                }
                catch (Exception e)
                {
                    log.Error("Encountered failure from API: " + e.Message);
                    log.Error("Full exception: " + e);
                    return new List<Server>();
                }
            }
        }

        private static async Task SaveUpdate(ServerStatusUpdate update, string name)
        {
            var json = JsonConvert.SerializeObject(update);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseApiUri);
                client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Change the following for your extremely basic authorisation to work
                // TODO: Remove this and use a more sophisticated authorisation method (See StatusController.cs)
                client.DefaultRequestHeaders.Add(_customAuthorisationHeader, _customAuthorisationSecret);

                try
                {
                    var response = await client.PostAsync(baseApiUri + "/api/status", new StringContent(json, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        log.Info($"--> Saved update to {name}: {update}");

                    }
                    else
                    {
                        log.Error("Encountered failure status code from API: " + response.StatusCode);
                        log.Error("Full response: " + response);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Encountered failure from API: " + e.Message);
                    log.Error("Full exception: " + e);
                }
            }
        }
 
        private static bool CheckServer(string host, int port, int timeout)
        {
            using (var tcp = new TcpClient())
            {
                var ar = tcp.BeginConnect(host, port, null, null);
                using (ar.AsyncWaitHandle)
                {
                    if (ar.AsyncWaitHandle.WaitOne(timeout, false))
                    {
                        try
                        {
                            tcp.EndConnect(ar);
                            log.Debug($"Successfully connected to {host}:{port}");
                            return true;
                        }
                        catch (Exception e)
                        {
                            log.Debug("EndConnect threw an exception - refused?" + e.StackTrace);
                            return false;
                        }
                    }
                    else
                    {
                        log.Info($"Timeout after {timeout}ms to {host}:{port}");

                        return false;
                    }
                }
            }
        }

       
    }
}
