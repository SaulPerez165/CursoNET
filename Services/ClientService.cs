using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;

namespace BankAPI.Services;

public class ClientService
{
    private readonly BankContext _context;
    private readonly Encoder encoder;
    public ClientService (BankContext context)
    {
        _context = context;
        encoder = new Encoder();
    }

    public async Task<IEnumerable<Client>> GetAll()
    {
        return await _context.Clients.Select(c => new Client
        {
            Id = c.Id,
            Name = c.Name,
            PhoneNumber = c.PhoneNumber,
            Email = c.Email,
            Pwd = encoder.Desencrypt(c.Pwd)
        }).ToListAsync();
    }

    public async Task<Client?> GetById(int id)
    {
        return await _context.Clients.
            Where(c => c.Id == id).
            Select(c => new Client
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email,
                Pwd = encoder.Desencrypt(c.Pwd)
            }).SingleOrDefaultAsync();
    }

    public async Task<Client> Create(Client newClient)
    {
        newClient.Pwd = encoder.Encrypt(newClient.Pwd);
        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync();

        return newClient;
    }

    public async Task Update(int id, Client client)
    {
        var existingClient = await GetById(id);

        if (existingClient is not null)
        {
            existingClient.Name = client.Name;
            existingClient.PhoneNumber = client.PhoneNumber;
            existingClient.Email = client.Email;
            existingClient.Pwd = encoder.Encrypt(client.Pwd);

            await _context.SaveChangesAsync();
        }
    }

    public async Task Delete(int id)
    {
        var clientToDelete = await GetById(id);

        if (clientToDelete is not null)
        {
            _context.Clients.Remove(clientToDelete);
            await _context.SaveChangesAsync();
        }
    }
}