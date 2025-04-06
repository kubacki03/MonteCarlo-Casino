namespace MonteCarlo.NET.HealthCheck
{
   
    public class ExceptionTracker
    {
        private Exception? _lastException;
        private readonly object _lock = new();

        public void SetException(Exception exception)
        {
            lock (_lock)
            {
                _lastException = exception;
            }
        }

        public Exception? GetLastException()
        {
            lock (_lock)
            {
                return _lastException;
            }
        }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionTracker _exceptionTracker;

        public ExceptionHandlingMiddleware(RequestDelegate next, ExceptionTracker exceptionTracker)
        {
            _next = next;
            _exceptionTracker = exceptionTracker;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
              
                _exceptionTracker.SetException(ex);
                
                throw; 

            }
           
        }
    }

}
