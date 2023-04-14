using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entity;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SQLitePCL;

namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }        

    [HttpPost("register")]//api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){
        if( await UserExist(registerDto.Username)) return BadRequest("User has already taken");
        using var hmac = new HMACSHA512();

        var user = new AppUser{
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return  new UserDto{
        Username = user.UserName,
        Token = _tokenService.CreateToken(user)
       };
     }

     [HttpPost("login")]
     public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
     {
        var user = await _context.Users.SingleOrDefaultAsync(user=> user.UserName.ToLower() == loginDto.Username.ToLower());
       if(user == null) return Unauthorized();
       using var hmac = new HMACSHA512(user.PasswordSalt);

       var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

       for(int i=0;i<ComputeHash.Length;i++){
            if(ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password!");
       }
       return new UserDto{
        Username = user.UserName,
        Token = _tokenService.CreateToken(user)
       };
     }

     private async Task<bool> UserExist(string username)
     {
        return await _context.Users.AnyAsync(user=>user.UserName.ToLower() == username.ToLower());
     }

  }
}