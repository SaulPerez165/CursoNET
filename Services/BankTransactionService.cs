using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class BankTransactionService
{
    private readonly BankContext _context;
    public BankTransactionService(BankContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetClientById(string? name, string? email)
    {
        return await _context.Clients.SingleOrDefaultAsync(c => c.Name == name && c.Email == email);
    }

    public async Task<Account?> GetAccountById(int id, Client? client)
    {
        return await _context.Accounts.SingleOrDefaultAsync(a => a.Id == id && a.ClientId == client.Id);
    }

    public async Task<IEnumerable<AccountDtoOut>> GetAll(string? name, string? email)
    {
        var client = await GetClientById(name,email);
        return await _context.Accounts.
            Where(a => a.ClientId == client.Id).
            Select(a => new AccountDtoOut
            {
                Id = a.Id,
                AccountName = a.AccountTypeNavigation.Name,
                ClientName = a.Client != null ? a.Client.Name : "",
                Balance = a.Balance,
                RegDate = a.RegDate
            }).ToListAsync();
    }

    public async Task<AccountDtoOut?> GetDtoById(string? name, string? email, int id)
    {
        var client = await GetClientById(name,email);
        var account = await GetAccountById(id,client);

        if (account is null)
            return null;
        
        return await _context.Accounts.
            Where(a => a.Id == account.Id).
            Select(a => new AccountDtoOut
            {
                Id = a.Id,
                AccountName = a.AccountTypeNavigation.Name,
                ClientName = a.Client != null ? a.Client.Name : "",
                Balance = a.Balance,
                RegDate = a.RegDate
            }).SingleOrDefaultAsync();
    }

    public async Task<BankTransaction> Create(BankTransactionDto transaction)
    {
        var account = await _context.Accounts.FindAsync(transaction.AccountId);

        if(transaction.TransactionType.Equals(1))
            account.Balance += transaction.Amount;
        else
            account.Balance -= transaction.Amount;

        var newBankTransaction = new BankTransaction
        {
            AccountId = transaction.AccountId,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            ExternalAccount = transaction.ExternalAccount
        };

        _context.BankTransactions.Add(newBankTransaction);
        await _context.SaveChangesAsync();

        return newBankTransaction;
    }

    public async Task Delete(Account accountToDelete)
    {
        await _context.BankTransactions.
            Where(t => t.AccountId == accountToDelete.Id).
            ForEachAsync(t => t.AccountId = null);

        _context.Accounts.Remove(accountToDelete);
        await _context.SaveChangesAsync();
    }
}