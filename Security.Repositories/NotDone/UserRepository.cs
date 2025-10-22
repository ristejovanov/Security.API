// -- todo -- I didn't find the time to make it like this.
// -- todo -- More precise with error messages and exception problem because of the base class
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Security.Domain;
//using Security.Repositories.interfaces;
//using Security.Shared;

//namespace Security.Repositories.impl
//{
//    public class UserRepository : RepositoryBase<User>, IUserRepository
//    {
//        public UserRepository(DbContext context, ILogger<UserRepository> logger)
//            : base(context, logger)
//        {
//        }

//        public async Task Add(User user)
//        {
//            await ExecuteSafeAsync(async () =>
//            {
//                _dbContext.Set<User>().Add(user);
//                await _dbContext.SaveChangesAsync();
//            }, nameof(Add), user.Id.ToString());
//        }

//        public async Task<bool> Update(User user)
//        {
//            return await ExecuteSafeAsync(async () =>
//            {
//                _dbContext.Set<User>().Update(user);
//                return await _dbContext.SaveChangesAsync() > 0;
//            }, nameof(Update), user.Id.ToString());
//        }

//        public async Task<User?> GetById(Guid id)
//        {
//            try
//            {
//                return await _dbContext.Set<User>().FindAsync(id);
//            }
//            catch (Exception ex)
//            {
//                throw HandleRepositoryException(ex, nameof(GetById), nameof(User), id.ToString());
//            }
//        }

//        public async Task<bool> Delete(Guid id)
//        {
//            await ExecuteSafeAsync(async () =>
//            {
//                var user = await _dbContext.Set<User>().FindAsync(id);
//                if (user == null)
//                    throw new RepositoryException($"[{nameof(User)}] Delete failed. Entity with ID {id} not found.");

//                _dbContext.Set<User>().Remove(user);
//                await _dbContext.SaveChangesAsync();
//            }, nameof(Delete), id.ToString());

//            return true;
//        }
//    }
//}