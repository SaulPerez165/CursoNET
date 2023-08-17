using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;

namespace BankAPI.Services;

public class LoginService
{
    private readonly BankContext _context;
    private readonly Encoder encoder;
    public LoginService(BankContext context)
    {
        _context = context;
        encoder = new Encoder();
    }

    public async Task<Administrator?> GetAdmin(AdminDto admin)
    {
        string encryptedPwd = encoder.Encrypt(admin.Pwd);
        return await _context.Administrators.
                    SingleOrDefaultAsync(x => x.Email == admin.Email && x.Pwd == encryptedPwd);
    }

    public async Task<Client?> GetClient(ClientDtoLogin clientDtoLogin)
    {
        string encryptedPwd = encoder.Encrypt(clientDtoLogin.Pwd);
        return await _context.Clients.
                    SingleOrDefaultAsync(x => x.Email == clientDtoLogin.Email && x.Pwd == encryptedPwd);
    }
}