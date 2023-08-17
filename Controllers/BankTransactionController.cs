using System.Security.Claims;
using BankAPI.Data.DTOs;
using BankAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers;

[Authorize(Policy = "ClientOnly")]
[ApiController]
[Route("api/[controller]")]
public class BankTransactionController : ControllerBase
{
    private readonly BankTransactionService bankTransactionService;
    private readonly TransactionTypeService transactionTypeService;
    private readonly AccountService accountService;

    public BankTransactionController(BankTransactionService bankTransactionService,
                                     TransactionTypeService transactionTypeService,
                                     AccountService accountService)
    {
        this.bankTransactionService = bankTransactionService;
        this.transactionTypeService = transactionTypeService;
        this.accountService = accountService;
    }
    
    [HttpGet("getall")]
    public async Task<IEnumerable<AccountDtoOut>> GetAll()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        return await bankTransactionService.GetAll(name, email);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDtoOut>> GetById(int id)
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var account = await bankTransactionService.GetDtoById(name, email, id);

        if (account is null)
            return AccountNotFound(id);

        return account;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(BankTransactionDto transaction)
    {
        string validationResult = await ValidateTransaction(transaction);

        if (!validationResult.Equals("Valid"))
            return BadRequest(new { message = validationResult });

        var newBankTransaction = await bankTransactionService.Create(transaction);

        return CreatedAtAction(nameof(GetById), new { id = newBankTransaction.Id }, newBankTransaction);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var client = await bankTransactionService.GetClientById(name,email);
        var accountToDelete = await accountService.GetById(id);

        if (accountToDelete is null)
            return AccountNotFound(id);
        else if (accountToDelete.ClientId != client.Id)
                return BadRequest(new { message = $"El ID({client.Id}) del cliente logueado no coincide con el ID({accountToDelete.ClientId}) del cliente en la cuenta ({accountToDelete.Id})." });
        else if (accountToDelete.Balance > 0)
                return BadRequest(new { message = $"La cuenta {accountToDelete.Id} aun dispone de dinero {accountToDelete.Balance}" });
        else
        {
            await bankTransactionService.Delete(accountToDelete);
            return Ok();
        }
    }

    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound(new { message = $"La cuenta con ID = {id} no existe."});
    }

    public async Task<string> ValidateTransaction(BankTransactionDto transaction)
    {
        string result = "Valid";
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var client = await bankTransactionService.GetClientById(name,email);
        var account = await accountService.GetById(transaction.AccountId);
        var transactionType = await transactionTypeService.GetById(transaction.TransactionType);
        var externalAccount = await accountService.GetById(transaction.ExternalAccount);

        if(account is null)
            result = $"La cuenta con ID = {transaction.AccountId} no existe.";
        else if (client.Id != account.ClientId)
            result = $"El ID({client.Id}) del cliente logueado no coincide con el ID({account.ClientId}) del cliente en la cuenta ({account.Id}).";
        else if (transactionType is null)
                result = $"El tipo de transaccion {transaction.TransactionType} no existe.";
        else if (transactionType.Id.Equals(3))
                result = "No es posible realizar depositos via transferencia.";
        else if (transactionType.Id.Equals(4))
        {
            if (transaction.ExternalAccount.Equals(null))
                result = "Debe asociar una cuenta externa o propia para hacer un retiro via transferencia.";
            else if (externalAccount is null)
                    result = $"La cuenta externa asociada {transaction.ExternalAccount} no existe.";
        }
        else if (!transaction.ExternalAccount.Equals(null))
                result = "No es posible asociar una cuenta externa para hacer una transaccion en efectivo.";
        else if (transaction.Amount <= 0)
                result = "La cantidad de la transaccion debe ser un numero positivo";
        else if (!transaction.TransactionType.Equals(1) && transaction.Amount > account.Balance)
                result = "No es posible hacer el retiro debido a que la cantidad supera el saldo de la cuenta.";

        return result;
    }
}