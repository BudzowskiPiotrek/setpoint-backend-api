namespace SetPoint.API.Common
{
    public class ApiResponse
    {
        #region Fields
        public object? Result { get; set; }

        // Flag para saber si hubo fallo sin mirar el código
        public bool WithError { get; set; }

        // El código HTTP (200, 400, 404, 500...)
        public int StatusCode { get; set; }

        // Mensaje descriptivo para el desarrollador o el usuario
        public string Message { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public ApiResponse(object? result = null, string message = "Success", int statusCode = 200)
        {
            Result = result;
            WithError = false;
            StatusCode = statusCode;
            Message = message;
        }

        // Método estático para errores (Rigor de ingeniería)
        public static ApiResponse Error(string message, int statusCode = 400)
        {
            return new ApiResponse
            {
                Result = null,
                WithError = true,
                StatusCode = statusCode,
                Message = message
            };
        }

        public ApiResponse() { }

        #endregion
    }
}

