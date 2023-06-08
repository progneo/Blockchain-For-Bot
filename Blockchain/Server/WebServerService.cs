using System.Text.Json;
using Blockchain.Miner;
using Blockchain.Model;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Microsoft.Extensions.Hosting;

namespace Blockchain.Server;

public class WebServerService : BackgroundService
{
    private readonly BlockMiner _blockMiner;
    private readonly TransactionPool _transactionPool;

    private readonly WebServer _server;
    private readonly string _url;
        
    public WebServerService(TransactionPool transactionPool, BlockMiner blockMiner)
    {
        _url = $"http://localhost:{5000}/";

        _server = CreateWebServer(_url);
        _transactionPool = transactionPool;
        _blockMiner = blockMiner;
    }
        

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"http server available at {_url}api");
        await _server.RunAsync(stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _server.Dispose();
        Console.WriteLine("http server stopped");
        return Task.CompletedTask;
    }
        
    private WebServer CreateWebServer(string url)
    {
        var server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
            .WithLocalSessionManager()
            .WithWebApi("/api", m => m.WithController(() => new Controller(_blockMiner, _transactionPool)))
            .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

        return server;
    }

    private sealed class Controller : WebApiController
    {
        private readonly BlockMiner _blockMiner;
        private readonly TransactionPool _transactionPool;

        public Controller(BlockMiner blockMiner, TransactionPool transactionPool)
        {
            _blockMiner = blockMiner;
            _transactionPool = transactionPool;
        }

        //GET http://localhost:port/api/blocks
        [Route(HttpVerbs.Get, "/blocks")]
        public string GetAllBlocks() => JsonSerializer.Serialize(_blockMiner.Blockchain);

        //GET http://localhost:port/api/blocks/index/{index?}
        [Route(HttpVerbs.Get, "/blocks/index/{index?}")]
        public string GetAllBlocks(int index)
        {
            Block? block = null;
            if (index < _blockMiner.Blockchain.Count)
                block = _blockMiner.Blockchain[index];
            return JsonSerializer.Serialize(block);
        }

        //GET http://localhost:port/api/blocks/latest
        [Route(HttpVerbs.Get, "/blocks/latest")]
        public string GetLatestBlocks()
        {
            var block = _blockMiner.Blockchain.LastOrDefault();
            return JsonSerializer.Serialize(block);
        }

        //GET http://localhost:port/api/validate
        [Route(HttpVerbs.Get, "/validate")]
        public string IsValidChain()
        {
            var isValid = _blockMiner.IsValidChain();
            return JsonSerializer.Serialize(isValid);
        }

        //Post http://localhost:port/api/add
        [Route(HttpVerbs.Post, "/add")]
        public string AddTransaction()
        {
            var availableAttributes = new List<string> {"Balance", "Level", "Experience"};
            var data = HttpContext.GetRequestDataAsync<Transaction>();
            
            if (!availableAttributes.Any(data.Result.Attribute.Contains)) return "wrong attribute";
            
            _transactionPool.AddRaw(data.Result);
            _blockMiner.MineBlock();

            return "success";
        }
        
        //GET http://localhost:port/api/users/{userId?}
        [Route(HttpVerbs.Get, "/users/{userId?}")]
        public string GetUserById(int userId)
        {
            var user = _blockMiner.GetUserById(userId);
            return JsonSerializer.Serialize(user);
        }
    }
}