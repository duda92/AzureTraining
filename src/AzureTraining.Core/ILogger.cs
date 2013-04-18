using System;
using System.Linq;

namespace AzureTraining.Core
{
    public interface ILogger
    {
        void UserLoggedIn(string login);

        void UserLoggedOut(string login);

        void UserRegistered(string login);

        void DocumentUploaded(string documentName, string login);

        void DocumentChangedPolicy(string owner, string documentName, bool isShared);

        void DocumentProcessingFinished(string owner, string documentName);
    }
}
