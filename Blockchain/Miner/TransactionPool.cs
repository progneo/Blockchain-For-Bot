using Blockchain.Model;

namespace Blockchain.Miner;

public class TransactionPool
{
    private readonly List<Transaction> _rawTransactionList;
    private readonly object _lockObj;

    public TransactionPool()
    {
        _lockObj = new object();
        _rawTransactionList = new List<Transaction>();
    }

    public int TransactionsCount()
    {
        lock (_lockObj)
        {
            return _rawTransactionList.Count;
        }
    }

    public void AddRaw(Transaction transaction)
    {
        lock (_lockObj)
        {
            _rawTransactionList.Add(transaction);
        }
    }

    public List<Transaction> TakeAll()
    {
        lock (_lockObj)
        {
            var all = _rawTransactionList.ToList();
            _rawTransactionList.Clear();
            return all;
        }
    }
}