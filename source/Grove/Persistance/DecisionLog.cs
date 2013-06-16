﻿namespace Grove.Persistance
{
  using System.IO;
  using System.Runtime.Serialization;
  using System.Runtime.Serialization.Formatters.Binary;
  using Gameplay;

  public class DecisionLog
  {
    private readonly BinaryFormatter _formatter;
    private MemoryStream _stream;

    public DecisionLog(Game game, MemoryStream savedDecisions)
    {
      _stream = savedDecisions ?? new MemoryStream();

      _formatter = SaveLoadHelper.CreateFormatter();

      _formatter.Context = new StreamingContext(
        StreamingContextStates.All,
        new SerializationContext {Game = game});
    }

    public bool IsAtTheEnd { get { return _stream.Position == _stream.Length; } }

    public void SetStream(MemoryStream stream)
    {
      _stream = stream;
    }

    public void SaveResult(object result)
    {
      _formatter.Serialize(_stream, result);
    }

    public object LoadResult()
    {
      return _formatter.Deserialize(_stream);
    }

    public void DiscardUnloadedResults()
    {
      _stream.SetLength(_stream.Position);      
    }

    public void WriteTo(Stream stream)
    {
      _stream.Position = 0;
      _stream.CopyTo(stream);
    }
  }
}