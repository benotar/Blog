﻿using Blog.Application.Interfaces.DbContext;
using Blog.Application.Interfaces.Repository;
using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Persistence.Repository;

public class UserRepository : IUserRepository
{
    private readonly IDbContext _dbContext;

    public UserRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AnyByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }
}