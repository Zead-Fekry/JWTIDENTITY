﻿using JWT.Models;

namespace JWT.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(GetTokenModel model);

    }
}
