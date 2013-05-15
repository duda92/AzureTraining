namespace AzureTraining.Core.Interfaces
{
    public interface IPaginationService
    {
        string GetDocumentPage(string documentContent, int page);

        string Paginate(string input); 
        
        int GetPagesCount(string content);
    }
}
