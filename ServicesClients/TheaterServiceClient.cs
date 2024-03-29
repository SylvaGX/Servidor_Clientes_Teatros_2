﻿using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class TheaterServiceClient
    {
        readonly Channel _channel;
        readonly TheaterService.TheaterServiceClient _client;

        public TheaterServiceClient(Channel channel, TheaterService.TheaterServiceClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<Confirmation> AddTheater(TheaterInfo theaterInfo)
        {
            try
            {
                Confirmation confirmation = _client.AddTheater(theaterInfo);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao adicionar o teatro. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1,
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar o teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1,
                });
            }
        }

        public async Task<Confirmation> UpdateTheater(TheaterInfo theaterInfo)
        {
            try
            {
                Confirmation confirmation = _client.UpdateTheater(theaterInfo);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao atualizar o teatro. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1,
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao atualizar o teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1,
                });
            }
        }

        public async Task<TheaterInfo> GetTheater(TheaterInfo theaterInfo)
        {
            try
            {
                TheaterInfo theater = _client.GetTheater(theaterInfo);

                return await Task.FromResult(theater);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber o teatro. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new TheaterInfo()
                {
                    Id = -1,
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber o teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new TheaterInfo()
                {
                    Id = -1,
                });
            }
        }

        public async Task<Confirmation> ChangeState(TheaterInfoState theaterInfoState)
        {
            try
            {
                Confirmation confirmation = _client.ChangeState(theaterInfoState);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao mudar o estado do teatro. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1,
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao mudar o estado do teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1,
                });
            }
        }

        public async Task<IEnumerable<TheaterInfo>> GetTheaters(UserConnected userConnected)
        {
            try
            {
                List<TheaterInfo> theaters = new();
                using (var call = _client.GetTheaters(userConnected))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        theaters.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(theaters.AsEnumerable());
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber os teatros. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<TheaterInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber os teatros.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<TheaterInfo>().AsEnumerable());
            }
        }
    }
}
