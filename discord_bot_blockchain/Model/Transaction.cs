namespace discord_bot_blockchain.Model;

public class Transaction
{
    public int SenderId { get; set; }
    public string Attribute { get; set; }
    public int Amount { get; set; }
}