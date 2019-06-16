namespace AwesomeGitHub.Models
{
    using System;

    public class ConnectivityException : Exception
    {
        public ConnectivityException() : base("Opps, without internet conection")
        {

        }
    }
}