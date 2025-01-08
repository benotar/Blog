﻿using Blog.Application.Interfaces.DbContext;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Models;
using Blog.Domain.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Blog.Persistence.Repository;

public class UserRepository : IUserRepository
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserRepository(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.Email == email)
            .ProjectToType<UserModel>()
            .FirstOrDefaultAsync(cancellationToken);
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