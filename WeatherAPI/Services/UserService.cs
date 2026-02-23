using JwtAuthApi.Models;

namespace JwtAuthApi.Services;

public interface IUserService
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
}

public class InMemoryUserService : IUserService
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
        return Task.FromResult(user);
    }
}
