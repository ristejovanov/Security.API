using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Security.Shared;

namespace Security.Repositories.NotDone
{
    /// <summary>
    /// Base repository providing consistent exception handling, logging, and diagnostics
    /// for all data-access operations.
    /// </summary>
    public abstract class RepositoryBase<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly ILogger _logger;

        protected RepositoryBase(DbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Executes a database operation and wraps expected exceptions in RepositoryException.
        /// Automatically logs failure context with operation name and entity type.
        /// </summary>
        protected async Task ExecuteSafeAsync(Func<Task> action, string operation, string identifier = "")
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                throw HandleRepositoryException(ex, operation, typeof(T).Name, identifier);
            }
        }

        /// <summary>
        /// Unified repository exception handler that provides meaningful diagnostics and preserves stack trace.
        /// </summary>
        private RepositoryException HandleRepositoryException(Exception ex, string operation, string entity, string identifier)
        {
            var context = string.IsNullOrWhiteSpace(identifier)
                ? $"[{entity}] {operation} failed."
                : $"[{entity}] {operation} failed for ID: {identifier}.";

            switch (ex)
            {
                case InvalidOperationException:
                    _logger.LogError(ex, "{Context} Entity already tracked by DbContext.", context);
                    return new RepositoryException($"{context} The entity is already tracked by the context.", ex);

                case DbUpdateException { InnerException: SqlException sql }:
                    _logger.LogError(ex, "{Context} SQL Error {ErrorNumber}.", context, sql.Number);
                    return new RepositoryException($"{context} SQL Error {sql.Number}: {sql.Message}", ex);

                case DbUpdateException:
                    _logger.LogError(ex, "{Context} Database update failed.", context);
                    return new RepositoryException($"{context} Database update failed.", ex);

                case SqlException sqlEx:
                    _logger.LogError(ex, "{Context} SQL Error {ErrorNumber}.", context, sqlEx.Number);
                    return new RepositoryException($"{context} SQL Error {sqlEx.Number}: {sqlEx.Message}", ex);

                default:
                    _logger.LogError(ex, "{Context} Unexpected error occurred.", context);
                    return new RepositoryException($"{context} Unexpected error: {ex.Message}", ex);
            }
        }
    }
}
