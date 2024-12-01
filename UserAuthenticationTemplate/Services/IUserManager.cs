﻿using Microsoft.AspNetCore.Identity;

namespace UserAuthenticationTemplate.Services
{
    public interface IUserManager<TUser>
    {
        Task<IdentityResult> CreateAsync(TUser user, string password);
        Task<bool> CheckPasswordAsync(TUser user, string password);

        Task<TUser?> FindByIdAsync(string id);
        Task<TUser?> FindByEmailAsync(string email);
        Task<TUser?> FindByNameAsync(string username);
    }
}