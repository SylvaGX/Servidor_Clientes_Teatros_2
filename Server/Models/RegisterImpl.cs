// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class RegisterImpl : Register.RegisterBase
    {
        private ServerContext DBcontext;
        
        public RegisterImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override Task<UserConnected> RegisterManager(ManagerRegister request, ServerCallContext context)
        {
            try
            {
                if (DBcontext.Users.FirstOrDefault(user => user.Mail.Equals(request.Email)) == null)
                {

                    var user = new User()
                    {
                        Name = request.Name,
                        Mail = request.Email,
                        Pass = request.Password,
                        Type = "2",
                        IdLocalization = request.IdLocalization,
                    };

                    DBcontext.Users.Add(user);

                    DBcontext.SaveChanges();

                    user = DBcontext.Users.FirstOrDefault(user => user.Mail.Equals(request.Email));

                    if (user != null)
                    {
                        return Task.FromResult(new UserConnected()
                        {
                            Id = user.Id,
                            Type = user.Type,
                        });
                    }

                    return Task.FromResult(new UserConnected()
                    {
                        Id = -1,
                        Type = "",
                    });
                }
            }
            catch (DbUpdateException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'DbUpdateException': [{DateTime.Now}] - Error - Erro de atualizar a base de dados.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao adicionar o manager. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar o manager.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return Task.FromResult(new UserConnected()
            {
                Id = -1,
                Type = "",
            });
        }

        public override Task<UserConnected> RegisterUser(UserRegister request, ServerCallContext context)
        {
            try
            {
                if (DBcontext.Users.FirstOrDefault(user => user.Mail.Equals(request.Email)) == null)
                {

                    var user = new User()
                    {
                        Name = request.Name,
                        Mail = request.Email,
                        Pass = request.Password,
                        Type = "1",
                        IdLocalization = request.IdLocalization,
                    };

                    DBcontext.Users.Add(user);

                    DBcontext.SaveChanges();

                    user = DBcontext.Users.FirstOrDefault(user => user.Mail.Equals(request.Email));

                    if (user != null)
                    {
                        return Task.FromResult(new UserConnected()
                        {
                            Id = user.Id,
                            Type = user.Type,
                        });
                    }

                    return Task.FromResult(new UserConnected()
                    {
                        Id = -1,
                        Type = "",
                    });
                }
            }
            catch (DbUpdateException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'DbUpdateException': [{DateTime.Now}] - Error - Erro de atualizar a base de dados.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao adicionar o utilizador. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar o utilizador.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return Task.FromResult(new UserConnected()
            {
                Id = -1,
                Type = "",
            });
        }
    }
}