using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Model;

public class Block
{
    public DateTime Timestamp { get; set; }
    public string PreviousHash { get; set; }
    public string Hash { get; set; }
    public int Nonce { get; set; }
    public List<Transaction> TransactionList { get; set; }

    public Block(DateTime timestamp, string previousHash, List<Transaction> transactionList)
    {
        Timestamp = timestamp;
        PreviousHash = previousHash;
        TransactionList = transactionList;
        Nonce = 0;
        Hash = CalculateHash();
    }

    public string CalculateHash()
    {
        using var sha256 = SHA256.Create();
        var data = Timestamp + PreviousHash + TransactionList + Nonce;
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }

    public void MineBlock(int difficulty)
    {
        var target = new string('0', difficulty);
        while (Hash[..difficulty] != target)
        {
            Nonce++;
            Hash = CalculateHash();
        }
        Console.WriteLine("Block mined: " + Hash);
    }
}