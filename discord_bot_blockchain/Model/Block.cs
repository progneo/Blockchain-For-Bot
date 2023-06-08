namespace discord_bot_blockchain.Model;

public class Block
{
    public long Index { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    public long Nonce { get; set; }
    public List<Transaction> TransactionList { get; set; }
}