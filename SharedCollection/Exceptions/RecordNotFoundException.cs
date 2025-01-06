namespace SharedCollection.Exceptions
{
    public class RecordNotFoundException<T> : Exception
          where T : notnull
    {
        public RecordNotFoundException(string name, T key)
            : base(String.Format("Запись в таблице \"{0}\" с идентификатором ({1}) не найдена.", name, key.ToString())) { }
    }
}
