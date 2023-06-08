using Blockchain.Model;

namespace Blockchain.Miner;

public class BlockMiner
{
    private const int Difficulty = 4;

    private readonly TransactionPool _transactionPool;

    public List<Block> Blockchain { get; set; }

    public BlockMiner(TransactionPool transactionPool)
    {
        Blockchain = new List<Block>();
        _transactionPool = transactionPool;
        CreateGenesisBlock();
    }

    private void CreateGenesisBlock()
    {
        Blockchain.Add(new Block(DateTime.Now, "Genesis Block", new List<Transaction>()));
    }

    private Block GetLatestBlock()
    {
        return Blockchain[^1];
    }

    public void MineBlock()
    {
        var latestBlock = GetLatestBlock();
        var newBlock = new Block(DateTime.Now, latestBlock.Hash, _transactionPool.TakeAll());
        newBlock.MineBlock(Difficulty);
        Blockchain.Add(newBlock);
    }

    public bool IsValidChain()
    {
        for (var i = 1; i < Blockchain.Count; i++)
        {
            var currentBlock = Blockchain[i];
            var previousBlock = Blockchain[i - 1];

            if (currentBlock.Hash != currentBlock.CalculateHash())
                return false;

            if (currentBlock.PreviousHash != previousBlock.Hash)
                return false;
        }

        return true;
    }

    public User GetUserById(int id)
    {
        var user = new User();
        foreach (var transaction in from block in Blockchain
                 from transaction in block.TransactionList
                 where transaction.SenderId == id
                 select transaction)
        {
            switch (transaction.Attribute)
            {
                case "Balance":
                    user.Balance += transaction.Amount;
                    break;
                case "Level":
                    user.Level += transaction.Amount;
                    break;
                case "Experience":
                    user.Experience += transaction.Amount;
                    break;
            }
        }

        user.Id = id;
        return user;
    }
}