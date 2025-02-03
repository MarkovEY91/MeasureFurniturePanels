using System;

namespace MeasureFurniturePanels
{
    public class CustomWarningException : Exception
    {
        public CustomWarningException(string message): base(message)
        {
            
        }
    }
}