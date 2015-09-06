using System;

namespace TetraSticks.ViewModel
{
    public interface IDispatcher
    {
        void Invoke(Delegate method, params object[] args);
    }
}
