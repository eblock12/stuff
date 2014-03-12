using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    [global::System.Serializable]
    public class GameException : Exception
    {
        public GameException() { }
        public GameException(string message) : base(message) { }
        public GameException(string message, Exception inner) : base(message, inner) { }
        protected GameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [global::System.Serializable]
    public class TaskException : GameException
    {
        public TaskException() { }
        public TaskException(string message) : base(message) { }
        public TaskException(string message, Exception inner) : base(message, inner) { }
        protected TaskException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    
    [global::System.Serializable]
    public class SdlException : GameException
    {
        public SdlException() { }
        public SdlException(string message) : base(message) { }
        public SdlException(string message, Exception inner) : base(message, inner) { }
        protected SdlException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    
    [global::System.Serializable]
    public class InitializationException : GameException
    {
        public InitializationException() { }
        public InitializationException(string message) : base(message) { }
        public InitializationException(string message, Exception inner) : base(message, inner) { }
        protected InitializationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    
    [global::System.Serializable]
    public class RendererInitializationException : InitializationException
    {
        public RendererInitializationException() { }
        public RendererInitializationException(string message) : base(message) { }
        public RendererInitializationException(string message, Exception inner) : base(message, inner) { }
        protected RendererInitializationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    
    [global::System.Serializable]
    public class DataLoadException : GameException
    {
        public DataLoadException() { }
        public DataLoadException(string message) : base(message) { }
        public DataLoadException(string message, Exception inner) : base(message, inner) { }
        protected DataLoadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
