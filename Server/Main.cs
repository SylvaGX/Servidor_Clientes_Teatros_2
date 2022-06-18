// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using gRPCProto;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                const int Port = 45300;

                var context = new ServerContext();

                Grpc.Core.Server server = new Grpc.Core.Server
                {
                    Services = {
                    Login.BindService(new LoginImpl(context)),
                    UserService.BindService(new UserServiceImpl(context)),
                    Register.BindService(new RegisterImpl(context)),
                    LocalizationService.BindService(new LocalizationServiceImpl(context)),
                    TheaterService.BindService(new TheaterServiceImpl(context)),
                    ShowService.BindService(new ShowServiceImpl(context)),
                    SessionService.BindService(new SessionServiceImpl(context)),
                    CartService.BindService(new CartServiceImpl(context)),
                    CompraService.BindService(new CompraServiceImpl(context)),
                },
                    Ports = { new ServerPort("10.144.10.2", Port, ServerCredentials.Insecure) }
                };
                server.Start();

                Console.WriteLine("RouteGuide server listening on port " + Port);
                Console.WriteLine("Press any key to stop the server...");
                Console.ReadKey();

                server.ShutdownAsync().Wait();
            }
            catch (AggregateException ex)
            {
                try
                {
                    var context = new ServerContext();
                    Logger logger = new Logger()
                    {
                        Msg = $"'AggregateException': [{DateTime.Now}] - Error - Erro ao inicializar o servidor. AggregateException.\n Code Msg: {ex.Message}",
                        LevelLog = 3,
                    };

                    context.Loggers.Add(logger);

                    context.SaveChanges();
                }
                catch (DbUpdateException ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                        writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. DbUpdateException.\n Code Msg: {ex2.Message}");
                    }
                }
                catch (Exception ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                        writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. Exception.\n Code Msg: {ex2.Message}");
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                try
                {
                    var context = new ServerContext();
                    Logger logger = new Logger()
                    {
                        Msg = $"'ObjectDisposedException': [{DateTime.Now}] - Error - Erro ao inicializar o servidor. ObjectDisposedException.\n Code Msg: {ex.Message}",
                        LevelLog = 3,
                    };

                    context.Loggers.Add(logger);

                    context.SaveChanges();
                }
                catch (DbUpdateException ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                        writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. DbUpdateException.\n Code Msg: {ex2.Message}");
                    }
                }
                catch (Exception ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                        writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. Exception.\n Code Msg: {ex2.Message}");
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                try
                {
                    var context = new ServerContext();
                    Logger logger = new Logger()
                    {
                        Msg = $"'InvalidOperationException': [{DateTime.Now}] - Error - Erro ao inicializar o servidor. InvalidOperationException.\n Code Msg: {ex.Message}",
                        LevelLog = 3,
                    };

                    context.Loggers.Add(logger);

                    context.SaveChanges();
                }
                catch (DbUpdateException ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                        writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. DbUpdateException.\n Code Msg: {ex2.Message}");
                    }
                }
                catch (Exception ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                        writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. Exception.\n Code Msg: {ex2.Message}");
                    }
                }
            }
            catch(IOException ex)
            {
                try
                {
                    var context = new ServerContext();
                    Logger logger = new Logger()
                    {
                        Msg = $"'IOException': [{DateTime.Now}] - Error - Erro ao inicializar o servidor. IOException.\n Code Msg: {ex.Message}",
                        LevelLog = 3,
                    };

                    context.Loggers.Add(logger);

                    context.SaveChanges();
                }
                catch (DbUpdateException ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                        writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. DbUpdateException.\n Code Msg: {ex2.Message}");
                    }
                }
                catch (Exception ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                        writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. Exception.\n Code Msg: {ex2.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var context = new ServerContext();
                    Logger logger = new Logger()
                    {
                        Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao inicializar o servidor. Exception.\n Code Msg: {ex.Message}",
                        LevelLog = 3,
                    };

                    context.Loggers.Add(logger);

                    context.SaveChanges();
                }
                catch (DbUpdateException ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error DbUpdateException, please check file LogOfLog.txt");
                        writer.WriteLine($"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. DbUpdateException.\n Code Msg: {ex2.Message}");
                    }
                }
                catch (Exception ex2)
                {
                    using (StreamWriter writer = File.AppendText("LogOfLog.txt"))
                    {
                        Console.WriteLine("Error Exception, please check file LogOfLog.txt");
                        writer.WriteLine($"'Exception': [{DateTime.Now}] - Error - Erro ao inserir o log error na base de dados. Exception.\n Code Msg: {ex2.Message}");
                    }
                }
            }
        }
    }
}
