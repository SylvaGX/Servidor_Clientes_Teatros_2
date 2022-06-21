using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class LogServiceClient
    {
        readonly Channel _channel;
        readonly LogService.LogServiceClient _client;

        public LogServiceClient(Channel channel, LogService.LogServiceClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<Confirmation> LogInformation(LogInfo logInfo)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.LogInformation(logInfo);
                if (confirmation.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'RpcException': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
            catch (Exception ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }
        public async Task<Confirmation> LogWarning(LogInfo logInfo)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.LogWarning(logInfo);
                if (confirmation.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'RpcException': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
            catch (Exception ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<Confirmation> LogError(LogInfo logInfo)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.LogError(logInfo);
                if (confirmation.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'RpcException': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
            catch (Exception ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<Confirmation> LogCritical(LogInfo logInfo)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.LogCritical(logInfo);
                if (confirmation.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'RpcException': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
            catch (Exception ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<IEnumerable<LogInfo>> GetAllLogs(UserConnected userConnected)
        {
            try
            {
                List<LogInfo> logs = new();
                using (var call = _client.getAllLogs(userConnected))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        logs.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(logs.AsEnumerable());
            }
            catch (RpcException ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'RpcException': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new List<LogInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao conectar à base de dados.\n");
                }
                return await Task.FromResult(new List<LogInfo>().AsEnumerable());
            }
        }
    }
}
