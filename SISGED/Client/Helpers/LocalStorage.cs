namespace SISGED.Client.Helpers
{
    public class LocalStorage
    {
        public LocalStorage(string key, object content)
        {
            Key = key;
            Content = content;
        }

        public string Key { get; set; } = default!;
        public object Content { get; set; } = default!;
    }
}
