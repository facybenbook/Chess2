﻿using Flow;

namespace App.Agent
{
    using Registry;

    /// <summary>
    /// AgentBase for all agents. Provides a custom logger and an ITransient implementation
    /// to be used with Flow library.
    /// </summary>
    public class AgentLogger
        : ITransient
        , ILogger
    {
        public event TransientHandler Completed;

        public bool Active { get; private set; }
        public IKernel Kernel { get; set; }
        public IFactory Factory => Kernel.Factory;
        public IFactory New => Kernel.Factory;
        public INode Root => Kernel.Root;
        public string Name { get; set; }
        [Inject] public IBoardAgent Board { get; set; }
        [Inject] public IArbiterAgent Arbiter { get; set; }
        public string LogPrefix { get { return _log.LogPrefix; } set { _log.LogPrefix = value; }}
        public object LogSubject { get { return _log.LogSubject; } set { _log.LogSubject = value; } }
        public bool ShowSource { get; set; }
        public bool ShowStack { get; set; }
        public int Verbosity { get { return _log.Verbosity; } set { _log.Verbosity = value; }}

        public ITransient Named(string name)
        {
            Name = name;
            return this;
        }

        public void Complete()
        {
            if (!Active)
                return;
            Completed?.Invoke(this);
            Active = false;
        }

        public void Info(string fmt, params object[] args)
        {
            _log.Info(fmt, args);
        }

        public void Warn(string fmt, params object[] args)
        {
            _log.Warn(fmt, args);
        }

        public void Error(string fmt, params object[] args)
        {
            _log.Error(fmt, args);
        }

        public void Verbose(int level, string fmt, params object[] args)
        {
            _log.Verbose(level, fmt, args);
        }

        protected AgentLogger()
        {
            _log.LogSubject = this;
            _log.LogPrefix = "Agent";
        }

        protected void TransientCompleted()
        {
            Completed?.Invoke(this);
        }

        protected readonly LoggerFacade<Flow.Impl.Logger> _log = new LoggerFacade<Flow.Impl.Logger>("Agent");
    }

    public abstract class AgentLogger<TModel>
        : AgentLogger
        where TModel : Model.IModel
    {
        public Model.IModel BaseModel { get; protected set; }
        public TModel Model { get; protected set; }
    }
}
