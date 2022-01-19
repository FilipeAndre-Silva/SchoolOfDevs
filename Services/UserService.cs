using Microsoft.EntityFrameworkCore;
using SchoolOfDevs.Entities;
using SchoolOfDevs.Exceptions;
using SchoolOfDevs.Helpers;
using SchoolOfDevs.Services.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace SchoolOfDevs.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Create(User user)
        {
            if(!user.Password.Equals(user.ConfirmPassword))
            {
                throw new BadRequestException($"Password does not match ConfirmPassword");
            }

            User userBb = await _context.Users.AsNoTracking()
                                              .SingleOrDefaultAsync(u => u.UserName == user.UserName);

            if(userBb is not null)
            {
                throw new BadRequestException($"UserName {user.UserName} already exist.");
            }

            user.Password = BC.HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> GetAll() => await _context.Users.ToListAsync();

        public async Task<User> GetById(int id)
        {
            User userDb = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

            if(userDb is null)
            {
                throw new KeyNotFoundException($"User {id} not found");
            }

            return userDb;
        }

        public async Task Update(User userIn, int id)
        {
            if(userIn.Id != id)
            {
                throw new BadRequestException($"Route id differs User id");
            }

            if(!userIn.Password.Equals(userIn.ConfirmPassword))
            {
                throw new BadHttpRequestException($"Password does not match ConfirmPassword");
            }
            
            User userDb = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);

            if(userDb is null)
            {
                throw new KeyNotFoundException($"User {id} not found");
            }
            else if(!BC.Verify(userIn.CurrentPassword, userDb.Password))
            {
                throw new BadRequestException($"Incorrect Password");
            }

            userIn.CreatedAt = userDb.CreatedAt;
            userIn.Password = BC.HashPassword(userIn.Password);

            _context.Entry(userIn).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            User userDb = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

            if(userDb is null)
            {
                throw new KeyNotFoundException($"User {id} not found");
            }

            _context.Users.Remove(userDb);
            await _context.SaveChangesAsync();
        }
    }
}