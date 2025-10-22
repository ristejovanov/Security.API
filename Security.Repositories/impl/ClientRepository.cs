using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Security.Data.EF.Infrastructure;
using Security.Domain;
using Security.Repositories.interfaces;
using Security.Shared;

namespace Security.Repositories.impl
{
    public class ClientRepository :IClientRepository
    {
        private readonly AppDbContext _dbContext;

        public ClientRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// Retrieves all clients from the database.
        /// </summary>
        public async Task<IEnumerable<Client>> GetAll()
        {
            try
            {
                return await _dbContext.Clients.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Unexpected error while retrieving clients.", ex);
            }
        }

        public async Task<Client?> GetById(Guid id)
            => await _dbContext.Clients.AsNoTracking().FirstOrDefaultAsync(u => u.ClientId == id);

        public async Task<bool> ClientNameExists(string clientName)
            => await _dbContext.Clients.AnyAsync(u => u.ClientName == clientName);

        public async Task Add(Client client)
        {
            try
            {
                _dbContext.Clients.Add(client);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
            {
                throw new RepositoryException($"SQL Error {sql.Number} while inserting client '{client.ClientName}'", ex);
            }
        }

        public async Task<bool> Update(Client client)
        {
            try
            {
                _dbContext.Clients.Update(client);
                var changed = await _dbContext.SaveChangesAsync();
                return changed > 0;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
            {
                throw new RepositoryException($"SQL Error {sql.Number} while updating client '{client.ClientName}'", ex);
            }
        }


    }
}
