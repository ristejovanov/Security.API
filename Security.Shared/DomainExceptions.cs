using System;

namespace Security.Shared
{

    public class NotFoundException : Exception { public NotFoundException(string m) : base(m) { } }
    public class ConflictException : Exception { public ConflictException(string m) : base(m) { } }
    public class ValidationException : Exception { public ValidationException(string m) : base(m) { } }
    public class RepositoryException : Exception { public RepositoryException(string m, Exception? inner = null) : base(m, inner) { } }

}
