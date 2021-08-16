namespace IdentityDemo.Model
{
    public class KeyValue<T>
    {
        public KeyValue()
        {
        }
        public string Key { get; set; }
        public T Value { get; set; }
    }
}
