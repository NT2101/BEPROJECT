using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.DTO.Request;
using Project.Interfaces;
using Project.Models;



namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {

        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public AccountsController(IAccountRepository accountRepository, IMapper mapper, DataContext context)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]

        public IActionResult GetAccounts()
        {
            var accounts = _mapper.Map<List<AccountDTO>>(_accountRepository.GetAccounts());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(accounts);
        }

        [HttpGet("{ID}")]
        public IActionResult GetAccount(int ID)
        {
            if (!_accountRepository.AccountExists(ID))
            {
                return NotFound();
            }

            var account = _accountRepository.GetAccount(ID);

            if (account == null)
            {
                return NotFound();
            }

            var accountDTO = _mapper.Map<AccountDTO>(account);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(accountDTO);
        }



        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id, [FromBody] AccountUpdateRequest updatedAccount)
        {
            if (updatedAccount == null)
                return BadRequest(ModelState);

            

            var existingAccount = _accountRepository.GetAccount(id);

            if (existingAccount == null)
                return NotFound();

            // Update properties from DTO to existing entity
            _mapper.Map(updatedAccount, existingAccount);

            try
            {
                if (!_accountRepository.UpdateAccount(existingAccount))
                {
                    ModelState.AddModelError("", "Lỗi xảy ra");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log and handle exception appropriately
                Console.WriteLine($"Error updating account: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpPost]
        public IActionResult CreateAccount([FromBody] AccountCreationRequest newAccountRequest)
        {
            if (newAccountRequest == null)
                return BadRequest("Invalid account data.");

            // Map DTO to Account model
            var newAccount = new Account
            {
                Name = newAccountRequest.Name,
                Password = newAccountRequest.Password,
                Status = 1, // Default status
                RoleID = 3 , // Default roleID
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CreatedUser = "API",
                ModifiedUser = "API"

            };

            try
            {
                // Create the new account
                if (!_accountRepository.CreateAccount(newAccount))
                {
                    ModelState.AddModelError("", "Error creating account");
                    return StatusCode(500, ModelState);
                }

                return CreatedAtAction(nameof(GetAccount), new { id = newAccount.ID }, newAccount);
            }
            catch (Exception ex)
            {
                // Log and handle exception appropriately
                Console.WriteLine($"Error creating account: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpDelete]
        public IActionResult DeleteAccount(int id)
        {
            if (!_accountRepository.AccountExists(id))
            {
                return NotFound();
            }

            var AccountToDelete = _accountRepository.GetAccount(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_accountRepository.DeleteAccount(AccountToDelete))
            {
                ModelState.AddModelError("", "Không xóa được tài khoản");
            }

            return NoContent();
        }


    }
}
