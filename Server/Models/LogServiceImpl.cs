// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class LogServiceImpl : LogService.LogServiceBase
    {
        private ServerContext DBcontext;
        
        public LogServiceImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override Task<Confirmation> LogInformation(LogInfo request, ServerCallContext context)
        {
            try
            {
                Logger logger = new Logger()
                {
                    Msg = request.Msg,
                    LevelLog = 1,
                };

                DBcontext.Loggers.Add(logger);

                DBcontext.SaveChanges();

                return Task.FromResult(new Confirmation()
                {
                    Id = 1,
                });
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                    writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log informativo na base de dados.\n Code Msg: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log informativo na base de dados.\n Code Msg: {ex.Message}");
                }
            }

            return Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override Task<Confirmation> LogWarning(LogInfo request, ServerCallContext context)
        {
            try
            {
                Logger logger = new Logger()
                {
                    Msg = request.Msg,
                    LevelLog = 2,
                };

                DBcontext.Loggers.Add(logger);

                DBcontext.SaveChanges();

                return Task.FromResult(new Confirmation()
                {
                    Id = 1,
                });
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                    writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log warning na base de dados.\n Code Msg: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log warning na base de dados.\n Code Msg: {ex.Message}");
                }
            }

            return Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override Task<Confirmation> LogError(LogInfo request, ServerCallContext context)
        {
            try
            {
                Logger logger = new Logger()
                {
                    Msg = request.Msg,
                    LevelLog = 3,
                };

                DBcontext.Loggers.Add(logger);

                DBcontext.SaveChanges();

                return Task.FromResult(new Confirmation()
                {
                    Id = 1,
                });
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                    writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados.\n Code Msg: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados.\n Code Msg: {ex.Message}");
                }
            }

            return Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override Task<Confirmation> LogCritical(LogInfo request, ServerCallContext context)
        {
            try
            {
                Logger logger = new Logger()
                {
                    Msg = request.Msg,
                    LevelLog = 4,
                };

                DBcontext.Loggers.Add(logger);

                DBcontext.SaveChanges();

                return Task.FromResult(new Confirmation()
                {
                    Id = 1,
                });
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                    writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log critical na base de dados.\n Code Msg: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                {
                    Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                    writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log critical na base de dados.\n Code Msg: {ex.Message}");
                }
            }

            return Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task getAllLogs(UserConnected request, IServerStreamWriter<LogInfo> responseStream, ServerCallContext context)
        {
            try
            {
                var loggers = DBcontext.Loggers;
                LogInfo logInfo;

                foreach (var log in loggers)
                {
                    logInfo = new LogInfo()
                    {
                        Id = log.Id,
                        DataTime = log.DataTime.Ticks,
                        Msg = log.Msg,
                        LevelLog = log.LevelLog
                    };


                    await responseStream.WriteAsync(logInfo);
                }
            }
            catch (Exception ex)
            {
                await LogError(new LogInfo()
                        {
                            Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao obter os logs.\nCode Msg: {ex.Message}",
                            LevelLog = 3
                        }, context);
            }
        }
    }
}