using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Security.Data.EF.Infrastructure;
using Security.Domain;
using Security.Repositories.interfaces;
using Security.Shared;

namespace Security.Repositories.impl
{
    public class UserRepository :IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext db)
        {
            _dbContext = db;
        }


        /// <inheritdoc />
        public async Task<bool> UserNameExists(string userName)
            => await _dbContext.Users.AnyAsync(u => u.UserName == userName);

        /// <inheritdoc />
        public async Task<bool> EmailExists(string email)
            => await _dbContext.Users.AnyAsync(u => u.Email == email);

        /// <inheritdoc />
        public async Task Add(User user)
        {
            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
            {
                throw new RepositoryException($"SQL Error {sql.Number} while inserting user '{user.Email}'", ex);
            }
        }

        /// <inheritdoc />
        public async Task<User?> GetById(Guid id)
            => await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        /// <inheritdoc />
        public async Task<bool> Update(User user)
        {
            try
            {
                _dbContext.Users.Update(user);
                var changed = await _dbContext.SaveChangesAsync();
                return changed > 0;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
            {
                throw new RepositoryException($"SQL Error {sql.Number} while updating user '{user.Email}'", ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> Delete(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user is null) return false;
            try
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
            {
                throw new RepositoryException($"SQL Error {sql.Number} while deleting user '{id}'", ex);
            }
        }
    }
}
