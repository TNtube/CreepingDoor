using System;

namespace Anvil.EditorEmojis
{
    internal abstract class Presenter : IDisposable
    {
        protected readonly SymbolsModel Model;
        protected readonly SymbolsView View;

        protected Presenter(SymbolsModel model, SymbolsView view)
        {
            Model = model;
            View = view;
        }

        internal virtual void OnOpen()
        {

        }

        public abstract void Dispose();
    }
}