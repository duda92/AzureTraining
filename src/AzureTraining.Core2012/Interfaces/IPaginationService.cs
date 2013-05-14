using System;
using System.Linq;

namespace AzureTraining.Core
{
    public interface IPaginationService
    {
        string GetDocumentPage(string documentContent, int page);

        string Paginate(string input); 
        
        int GetPagesCount(string content);
    }
}
