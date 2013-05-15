namespace AzureTraining.Core
{
    public enum TransliterationType
    {
        Gost,
        ISO
    }

    public interface ITransliterationService
    {
        string Front(string text);
        
        string Front(string text, TransliterationType type);

        string Back(string text);

        string Back(string text, TransliterationType type);
        
    }
}
